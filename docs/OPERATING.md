# Operating Guide

## Environments
- Environment Names: **Development** / **Staging** / **Production**
- 공통 구성: API, Redis(옵션), DB, Observability(옵션)

## Health & Readiness
- `/health`, `/ready` (도입 시 운영 시스템과 연계)
- 알림 임계치(지연/에러율/큐 길이)는 환경별 분리

## Configuration
- 환경변수 우선, `appsettings.{env}.json` 보조
- Secrets: 운영은 Secret Manager/Vault 사용(로컬 .env는 제외)

### Environment Variables (MVP)
- `ASPNETCORE_ENVIRONMENT`: 실행 환경 (Development/Staging/Production)
- `URLS` (또는 `ASPNETCORE_URLS`): 바인딩 주소/포트 (예: `http://0.0.0.0:8080`)
- `CONNECTIONSTRINGS__Default`: 기본 DB 연결 문자열
- `REDIS__CONNECTION`: Redis 연결 문자열 (옵션)
- `OBSERVABILITY__ENABLED`: 관측 기능 On/Off
- `OTEL_EXPORTER_OTLP_ENDPOINT`: OTLP 수집기 엔드포인트 (옵션)
- `LOG__LEVEL__DEFAULT`: 기본 로그 레벨 (예: `Information`)
- `AUTH__JWT__SECRET`: JWT 서명 키 (옵션, 비밀관리로 주입)

## Observability
- 토글: `OBSERVABILITY__ENABLED`
- 수집: Metrics/Traces/Logs, 샘플링/엔드포인트는 환경별 설정

## Scaling
- Stateless 우선, 세션/매칭 상태는 외부 저장소(예: Redis) 분리
- 오토스케일 신호: CPU/지연/큐 길이

## Deploy
- CI/CD: Build → Test → Scan → Image Push → Deploy
- 롤백: 버전 태그 기반 이전 이미지로 즉시 복귀

### Container Registry (Plan)
- 기본 방향: 단일 레지스트리 사용(예: GHCR `ghcr.io/<org>/gameserver.net/...`).
- 변수 가이드(초안): `REGISTRY`, `IMAGE_NAME`, `IMAGE_TAG`, `REGISTRY_USERNAME/TOKEN`.
- 상세는 별도 GitHub 이슈에서 확정 후 CI 문서에 반영.

## Backup/Restore
- DB 백업 주기/보관 기간 명시
- Redis는 캐시 성격, 필요 시 AOF/RDB 정책 선택

## Incident
- 공용 런북 경로(추후 `docs/runbooks/*`)와 알림 채널 사용


