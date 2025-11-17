# Changelog

이 프로젝트의 모든 주요 변경사항은 이 파일에 기록됩니다.

형식은 [Keep a Changelog](https://keepachangelog.com/en/1.0.0/)를 기반으로 하며,
이 프로젝트는 [Semantic Versioning](https://semver.org/spec/v2.0.0.html)을 준수합니다.

## [0.3.0] - 2025-11-18

### Added

* 매치메이킹 시스템 전체 구현 (#16, #17, #18, #19, #37, #39, #40, #41, #42)
  * Domain 레이어: `MatchmakingQueue` Aggregate Root, `Match`, `PlayerMatchRequest` 엔티티, `MMR` 값 객체, `MatchmakingDomainService` 구현
  * Application 레이어: `JoinMatchmakingQueueUseCase`, `CancelMatchmakingUseCase`, Port 인터페이스 (`IMatchmakingRepository`, `IMatchmakingNotifier`)
  * Infrastructure 레이어: `InMemoryMatchmakingRepository`, `SignalRMatchmakingNotifier`, `MatchmakingBackgroundService` 구현
  * API 레이어: `POST /api/fps/matchmaking/join`, `DELETE /api/fps/matchmaking/cancel` 엔드포인트, `MatchmakingHub` SignalR Hub 구현
  * MMR 기반 매칭 로직 (허용 범위 ±100)
  * 실시간 매칭 알림 (SignalR)
  * 단위 테스트 작성 (38개 테스트, 모두 통과)
* 매치메이킹 시스템 세부 설계 문서 추가 (#36)
  * `docs/design/matchmaking-detailed-design.md` - 시퀀스 다이어그램 및 상세 설계
  * `docs/LOCAL_TEST_SIGNALR.md` - SignalR 로컬 테스트 가이드
* 프로젝트 타겟 프레임워크 조정
  * Domain, Application, Infrastructure: .NET 8.0
  * API: .NET 9.0 (OpenAPI 기능 활용)
  * 테스트 프로젝트: .NET 9.0

### Changed

* 릴리스 Merge commit 메시지 컨벤션 추가 (#35)
  * `docs/RELEASE.md`에 Merge commit 메시지 규칙 및 예시 추가
* 컨벤션 FAQ 업데이트 (#38)
  * 이슈-PR 연결 및 자동 Close 규칙 추가
  * DRAFT PR 사용 규칙 추가

## [0.2.0] - 2025-11-08

### Added

* Architecture Decision Records (ADR) for core game server systems
  * `docs/adr/0001-matchmaking-design.md` - 매치메이킹 시스템 아키텍처 결정 (#13, #29)
    * 큐 관리: 메모리 기반 시작 (MVP), Redis 확장 가능 구조
    * 매칭 알고리즘: 백그라운드 서비스 기반 주기적 스캔
    * 도메인 모델: MatchmakingQueue, Match, PlayerMatchRequest, MMR
    * 알림 방식: SignalR Hub 기반
  * `docs/adr/0002-realtime-chat-design.md` - 실시간 채팅 시스템 아키텍처 결정 (#14, #32)
    * 실시간 통신: ASP.NET Core SignalR 사용
    * 채팅방 관리: 메모리 기반 시작 (MVP), DB 확장 가능
    * 메시지 영속성: Hybrid 전략 (MVP는 메모리, 확장 시 DB)
    * 도메인 모델: ChatRoom, ChatMessage, ChatUser
  * `docs/adr/0003-match-session-mmr-design.md` - 매치 세션 라이프사이클 및 MMR 시스템 아키텍처 결정 (#15, #31)
    * MMR 계산 알고리즘: Elo 레이팅 시스템 채택
    * 매치 세션 상태 관리: DB 기반 (EF Core)
    * MMR 업데이트 트리거: 매치 종료 UseCase에서 동기 호출 (MVP)
    * 도메인 모델: MatchSession, MatchResult, IMMRCalculator
* 프로젝트 기초 구조 세팅 (#27, #28)
  * 솔루션 폴더 구조 추가 (`src/`, `tests/` 하위 폴더)
  * Visual Studio Solution Folders 구성

### Changed

* PR 템플릿 한글화 및 이모지 추가 (#28)
* PR 제목 규칙 컨벤션 업데이트 (#28)
  * 이슈 번호는 PR 본문에서 처리하도록 변경 (Squash merge 시 중첩 문제 해결)
