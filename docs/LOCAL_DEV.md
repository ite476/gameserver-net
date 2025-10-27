# Local Development

## Prerequisites
- .NET 9 SDK (또는 .NET 8 LTS 브랜치)
- VS 2022 또는 `dotnet` CLI

## Restore & Build
```bash
dotnet restore
dotnet build
```

## Run (예: FPS API)
```bash
dotnet run --project src/Services/FpsServer/FpsServer.Api
```

## Run (예: MOBA API)
```bash
dotnet run --project src/Services/MobaServer/MobaServer.Api
```

## Environment Variables (예시)
- `ASPNETCORE_ENVIRONMENT`: Development
- `CONNECTIONSTRINGS__Default`: InMemory/LocalDB/SQL
- `OBSERVABILITY__ENABLED`: true|false

## HTTPS Dev Certificate
```bash
dotnet dev-certs https --trust
```

## Tests
```bash
dotnet test
```

## EF Core Migrations (옵션)
```bash
# Add migration
dotnet ef migrations add Init --project src/Services/FpsServer/FpsServer.Infra
# Apply
dotnet ef database update --project src/Services/FpsServer/FpsServer.Infra
```

## Common Issues
- 포트 충돌: 실행 포트 조정 또는 기존 프로세스 종료
- 인증서 이슈: 개발 인증서 재설치/신뢰 등록
- PATH/SDK 이슈: `dotnet --info`로 SDK 버전 확인


