# Changelog

All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.0.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

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

