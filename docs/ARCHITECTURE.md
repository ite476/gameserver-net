# Architecture

## Scope
- 실시간 게임 서버(FPS/MOBA) 공통 가이드와 본 레포 구조 요약.

## Layers
- API → App → Domain → Infra
  - API: HTTP/SignalR 엔드포인트, 입력 검증, DTO 변환
  - App: 유스케이스, 트랜잭션 경계, Port 호출, 응용 서비스
  - Domain: 엔티티/값객체/도메인 서비스, 규칙과 불변성
  - Infra: Repository/MessageBus/Cache/Lock 어댑터 (App Port 구현)

## Platform
- SharedKernel, Observability(OpenTelemetry), Locks, Persistence(EF Core/Redis)

## Services
- FpsServer, MobaServer (각각 독립 레이어 세트, 공통 Platform 활용)

## Dependency Rules
- 단방향: API → App → Domain
- Infra는 App의 Port(Interface)를 구현하여 주입

## Ports & Adapters
- App Port(Interface) ←→ Infra Adapter(구현)
- 외부 의존(데이터베이스/캐시/메시지버스) 교체 용이성 확보

## State & Idempotency
- 멱등 키/메시지 재처리/중복방지 정책은 서비스별 문서로 확장 권장

## Observability
- 핵심 지점: 매칭/세션 수명주기/락 경합/에러율/지연
- 환경 변수 기반 토글로 수집 부하 최소화

## Folder Layout
```
src/
  Platform/
  Services/
    FpsServer/    (Api/App/Domain/Infra/Contracts)
    MobaServer/   (Api/App/Domain/Infra/Contracts)
tests/
```


