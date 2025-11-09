# ADR-0001: 매치메이킹 시스템 아키텍처 결정

## Status
Accepted

## Context
게임 서버에서 플레이어를 매칭하여 게임 세션을 생성하는 핵심 기능이 필요합니다.  
실시간 게임 특성상 빠른 매칭(응답시간), 공정한 매칭(MMR/등급 기반), 확장성(동시 매칭 수)을 고려해야 합니다.

다음 결정 사항들이 필요합니다:
- 매칭 큐 관리 방식 (메모리 vs Redis vs DB)
- 매칭 알고리즘 (즉시 매칭 vs 주기적 스캔)
- 도메인 모델 구조 (Aggregate Root, 엔티티, 값 객체)
- 매칭 성공 알림 방식 (SignalR vs Polling)

## Decision

### 1. 큐 관리: 메모리 기반 시작 (MVP), Redis 확장 가능 구조
- MVP: `InMemoryMatchmakingRepository` (ConcurrentDictionary 기반)
- 확장: Redis Sorted Set 기반으로 전환 가능한 Port 인터페이스 설계
- 이유: 빠른 개발, 단순성, 이후 확장 용이

### 2. 매칭 알고리즘: 백그라운드 서비스 기반 주기적 스캔
- `MatchmakingBackgroundService` (IHostedService) 구현
- 주기: 1초마다 큐 스캔 및 매칭 시도
- 이유: 확장 가능, 확률적 매칭 로직 구현 용이

### 3. 도메인 모델: DDD 패턴 적용
- `MatchmakingQueue` (Aggregate Root) - 큐 상태 관리
- `Match` (엔티티) - 매칭 결과
- `PlayerMatchRequest` (엔티티) - 플레이어 요청
- `MMR` (값 객체, `readonly record struct`) - 레이팅 값
  - EF Core 8 복합 형식(Complex Type)으로 지원
  - 불변성 보장을 위해 `readonly` 키워드 필수
- `MatchmakingDomainService` - 매칭 로직 (MMR 기반)

### 4. 알림 방식: SignalR Hub 기반
- `MatchmakingHub` 구현
- 매칭 성공 시 그룹 브로드캐스트
- 이유: 실시간성, 확장성

### 5. 레이어 구조: API → App → Domain → Infra
- API: HTTP 엔드포인트 + SignalR Hub
- App: UseCase + Port 인터페이스
- Domain: 순수 비즈니스 로직
- Infra: Repository 구현체

## Consequences

### 장점
- 메모리 기반으로 빠른 MVP 구현 가능
- Port 인터페이스로 Redis 전환 용이
- DDD 패턴으로 도메인 로직 명확화
- SignalR으로 실시간 알림 제공

### 단점/제약
- 메모리 기반은 서버 재시작 시 큐 손실 (확장 단계에서 Redis로 전환 필요)
- 백그라운드 서비스는 주기적 스캔으로 약간의 지연 가능 (1초 이내)
- `record struct` 사용 시 기술적 제약:
  - EF Core 8 이상 필요 (복합 형식 지원)
  - `readonly record struct` 사용 필수 (불변성 보장)
  - null 허용 시 `MMR?` 형식 사용 필요
  - 복합 형식은 소유 엔터티와 함께 저장되므로 별도 테이블 생성 안 됨

## 참고
- 아키텍처: [docs/ARCHITECTURE.md](../ARCHITECTURE.md)
- 컨벤션: [docs/CONVENTIONS.md](../CONVENTIONS.md)
- 관련 이슈: #13
- 후속 작업: #16 (Domain), #17 (Application), #18 (API), #19 (Infrastructure)

