# ADR-0002: 실시간 채팅 시스템 아키텍처 결정

## Status
Accepted

## Context
게임 내/로비에서 실시간 채팅 기능이 필요합니다.  
SignalR 기반으로 WebSocket 연결을 활용하며, 채팅방/채널 관리, 메시지 전송, 접속자 관리 등을 다룹니다.

다음 결정 사항들이 필요합니다:
- 실시간 통신 방식 (SignalR vs WebSocket 직접)
- 채팅방 관리 방식 (메모리 vs DB)
- 메시지 영속성 전략 (메모리 vs DB vs Hybrid)
- 메시지 전송 흐름 (Hub → Application → Domain)

## Decision

### 1. 실시간 통신: ASP.NET Core SignalR 사용
- `ChatHub` 클래스 구현
- 그룹 기반 방 관리 (`Groups.AddToGroupAsync`)
- 이유: ASP.NET Core 네이티브, 연결 관리 자동화, 확장성

### 2. 채팅방 관리: 메모리 기반 시작 (MVP), DB 확장 가능
- MVP: `InMemoryChatRepository` (ConcurrentDictionary 기반)
- 확장: EF Core 기반으로 전환 가능한 Port 인터페이스 설계
- 이유: 빠른 개발, 단순성, 이후 확장 용이

### 3. 메시지 영속성: Hybrid 전략 (MVP는 메모리, 확장 시 DB)
- MVP: 메모리 기반 (최근 N개 메시지만 유지)
- 확장: EF Core로 이력 저장 (선택)
- 이유: 실시간성 우선, 이력은 선택적

### 4. 도메인 모델: DDD 패턴 적용
- `ChatRoom` (Aggregate Root) - 채팅방 상태 관리
- `ChatMessage` (엔티티) - 메시지 내용 및 메타데이터
- `ChatUser` (값 객체, `record`) - 사용자 정보
  - EF Core 복합 형식 또는 Owned Entity Type으로 지원 가능
- `ChatDomainService` (Domain Service) - 메시지 검증, 금지어 필터 등

### 5. 레이어 구조: SignalR Hub → Application → Domain → Infra
- Hub: WebSocket 연결 관리, 메시지 수신/브로드캐스트
- App: UseCase + Port 인터페이스
- Domain: 순수 비즈니스 로직 (메시지 검증, 금지어 필터 등)
- Infra: Repository 구현체

### 6. 메시지 전송 흐름
```
클라이언트 → Hub.SendMessage() → SendMessageUseCase → Domain 검증 → Repository 저장 → Notifier → Hub 그룹 브로드캐스트
```

## Consequences

### 장점
- SignalR으로 WebSocket 연결 관리 자동화
- 메모리 기반으로 빠른 MVP 구현
- Port 인터페이스로 DB 전환 용이
- 그룹 기반으로 효율적인 브로드캐스트
- Hybrid 전략으로 실시간성과 이력 관리 균형

### 단점/제약
- 메모리 기반은 서버 재시작 시 메시지 손실 (확장 단계에서 DB 전환 필요)
- 메시지 영속성은 MVP 단계에서 제한적 (최근 N개만 유지)
- SignalR 연결 수에 따른 서버 리소스 사용 증가

## 참고
- 아키텍처: [docs/ARCHITECTURE.md](../ARCHITECTURE.md)
- 컨벤션: [docs/CONVENTIONS.md](../CONVENTIONS.md)
- 관련 이슈: #14
- 후속 작업: #22 (Domain 및 Application), #21 (SignalR Hub 및 Infrastructure)

