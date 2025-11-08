# ADR-0003: 매치 세션 라이프사이클 및 MMR 시스템 아키텍처 결정

## Status
Accepted

## Context
매칭 성공 후 게임 세션이 시작되고, 종료 시 승/패 결과에 따라 MMR이 증감됩니다.  
MMR 시스템은 게임 밸런스를 유지하는 핵심 요소이며, 공정한 매칭을 위해 필수적입니다.

다음 결정 사항들이 필요합니다:
- MMR 계산 알고리즘 (Elo vs 커스텀)
- 매치 세션 상태 관리 방식 (메모리 vs DB)
- 매치 종료 시 MMR 업데이트 트리거 방식 (동기 vs 비동기)
- 매치 결과 저장 전략

## Decision

### 1. MMR 계산 알고리즘: Elo 레이팅 시스템 채택
- 공식: `New MMR = Old MMR + K * (Actual Score - Expected Score)`
- Expected Score: `1 / (1 + 10^((Opponent MMR - Player MMR) / 400))`
- K 값: 32 (기본값, 조정 가능)
- 팀 매칭: 팀 평균 MMR 기반 계산
- 이유: 업계 표준, 검증된 알고리즘, 구현 단순성

### 2. 매치 세션 상태 관리: DB 기반 (EF Core)
- `MatchSession` 엔티티로 영속화
- 상태 전이: `Matched → Starting → InProgress → Finished`
- 이유: 세션 이력 추적, 재시작 시 복구 가능

### 3. MMR 업데이트 트리거: 매치 종료 UseCase에서 동기 호출 (MVP)
- `EndMatchUseCase` 완료 후 `UpdatePlayerMMRUseCase` 호출
- 확장: 도메인 이벤트 기반 비동기 처리 가능 구조
- 이유: MVP 단순성, 일관성 보장, 이후 확장 용이

### 4. 도메인 모델: DDD 패턴 적용
- `MatchSession` (Aggregate Root)
  - ID, MatchId, Status, StartedAt, EndedAt, Players, GameMode
  - `Start()`, `End()` 메서드로 상태 전이
- `MatchResult` (엔티티)
  - MatchId, PlayerResults, Winner, EndedAt
- `PlayerResult` (값 객체, `record`)
  - PlayerId, IsWinner, Score (게임별 통계)
- `MMRCalculator` (Domain Service)
  - `CalculateNewMMR()` 메서드
- `PlayerMMR` (엔티티)
  - PlayerId, CurrentMMR, UpdatedAt

### 5. 레이어 구조: API → App → Domain → Infra
- API: HTTP 엔드포인트 (`POST /api/fps/matches/{matchId}/start`, `/end`)
- App: UseCase + Port 인터페이스
- Domain: 상태 전이 규칙, MMR 계산 로직
- Infra: EF Core Repository

### 6. 추상화 원칙: MVP 단계에서 불필요한 추상화 최소화
- **원칙**: MVP 단계에서는 실제 필요성이 명확해질 때까지 추상화를 최소화
- **적용**:
  - MMR 업데이트를 동기 호출로 처리 (도메인 이벤트 기반 비동기 처리 제외)
  - 이유: MVP에서는 일관성 보장이 우선이며, 비동기 처리의 복잡도 대비 이점이 불명확
  - 확장 시점: 실제로 비동기 처리의 필요성이 명확해질 때 도메인 이벤트 기반 구조로 전환
- **장점**: 개발 속도 향상, 복잡도 감소, 일관성 보장
- **단점**: 향후 비동기 처리 전환 시 리팩토링 필요 (하지만 실제 필요 시점에 도입하는 것이 더 효율적)

## Consequences

### 장점
- Elo 알고리즘으로 공정한 MMR 계산
- DB 기반으로 세션 이력 추적 가능
- 동기 처리로 일관성 보장
- 확장 가능한 구조 (이벤트 기반 전환 가능)
- MVP 단계에서 불필요한 추상화 최소화로 개발 속도 향상

### 단점/제약
- 동기 MMR 업데이트로 매치 종료 응답 지연 가능 (소폭)
- DB 의존성 증가 (메모리 대비)
- MVP 단계에서 추상화 최소화로 인한 향후 리팩토링 가능성 (실제 필요 시점에 도입하는 것이 더 효율적)

## 참고
- 매치메이킹 설계: [ADR-0001](./0001-matchmaking-design.md)
- 아키텍처: [docs/ARCHITECTURE.md](../ARCHITECTURE.md)
- 컨벤션: [docs/CONVENTIONS.md](../CONVENTIONS.md)
- 관련 이슈: #15
- 후속 작업: #23 (매치 세션 관리), #24 (MMR 계산 및 업데이트), #25 (API), #26 (Infrastructure)

