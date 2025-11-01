### Summary

Platform 공통 모듈(SharedKernel, Observability, Locks, Persistence) 기초 구조 및 프로젝트 솔루션 세팅

### Acceptance Criteria

- [ ] 솔루션/프로젝트 구조(docs/ARCHITECTURE.md, docs/CONVENTIONS.md 기준)
- [ ] Platform/SharedKernel 모듈 초기화
- [ ] Platform/Observability 초기화(OpenTelemetry 기본 설정)
- [ ] Platform/Locks 초기화(기본 인터페이스)
- [ ] Platform/Persistence 초기화(EF Core 기본 구성)
- [ ] 빌드/실행 성공

### Steps

- [ ] 솔루션 파일(.sln) 생성 및 프로젝트 구조(docs 아키텍처 섹션 5 참조)
- [ ] Platform/SharedKernel 프로젝트 생성(값객체/예외 등 기본 타입)
- [ ] Platform/Observability 프로젝트 생성(OTel 기본 설정/App 상속)
- [ ] Platform/Locks 프로젝트 생성(IRedisDistributedLock 인터페이스 등)
- [ ] Platform/Persistence 프로젝트 생성(EF Core DbContext 기본, 마이그레이션 폴더)
- [ ] 기본 DI 구성(appsettings.json 포함)
- [ ] `dotnet build`, `dotnet test` 통과

### Links

- 아키텍처: [docs/ARCHITECTURE.md](docs/ARCHITECTURE.md)
- 컨벤션: [docs/CONVENTIONS.md](docs/CONVENTIONS.md)
- 로컬 개발: [docs/LOCAL_DEV.md](docs/LOCAL_DEV.md)