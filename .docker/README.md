# Docker-first

## Prerequisites
- Docker Desktop

## Quick Start
```bash
docker compose up -d
```

## Services (옵션)
- api: FpsServer/MobaServer 중 선택 기동
- redis: 매칭/세션 캐시
- prometheus/grafana: 관측(옵션)

## Configuration
- `.env`로 포트/토글 관리 (예: `OBSERVABILITY_ENABLED=true`)
- 개발/운영 분리 시 `compose.dev.yaml`, `compose.prod.yaml` 사용

## Common Tasks
```bash
docker compose logs -f api
docker compose restart api
docker compose down -v
```


