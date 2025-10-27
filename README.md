# GameServer.Net

> **목적:** .NET 기반 게임 서버 아키텍처 레퍼런스.
> **구성:** 공통 플랫폼(관측/락/영속) + 서비스(FPS, MOBA) 샘플.
> **타깃:** 실무형 포트폴리오 & 팀 온보딩용 베이스라인.

---

## 🔧 주요 스택

* **Runtime:** .NET 9 (개발 기본), *옵션* .NET 8 LTS 브랜치
* **Web:** ASP.NET Core Web API, SignalR
* **Data:** EF Core (InMemory/SQL), Redis(옵션)
* **Obs.:** OpenTelemetry, Prometheus (옵션 Grafana)
* **Test:** xUnit, FluentAssertions, Moq
* **Build:** VS 2022 / `dotnet` CLI

---

## 🧩 아키텍처

* 레이어: **API → App → Domain → Infra**, 공통 **Platform** 모듈 분리
* 서비스: **FpsServer**, **MobaServer** (각각 독립 레이어 세트)
* 의존 규칙: API⇢App⇢Domain 단방향, Infra는 App의 Port 구현

```
src/
  Platform/       # 공통: SharedKernel, Observability, Locks, Persistence
  Services/
    FpsServer/    # FpsServer.Api/App/Domain/Infra/Contracts
    MobaServer/   # MobaServer.Api/App/Domain/Infra/Contracts
tests/
```

---

## ✨ 주요 특이사항

* **실전 대비:** 모노레포 + 멀티프로젝트, Git-Flow 컨벤션 적용
* **확장성:** 서비스별 독립 버전·릴리스 태깅(옵션: `fps/0.2.0` 등)
* **학습 포인트:** 멱등/상태전이 중심 설계, 필요 시 분산락 브랜치 온
* **관측성 내장:** 요청/매칭/락 경합 등 메트릭 노출(선택적 활성화)

---

## 🚀 빠른 시작

> **Docker-first**: 실행/환경 구성은 [./.docker/README.md](./.docker/README.md)에서 관리합니다. 로컬 개발용 수동 실행 가이드는 `.docker` 정리 이후 `./docs/LOCAL_DEV.md`로 이동 예정입니다.

---

## 🔗 주요 링크

* ▶️ **Quick Start 위키/문서**: (추가 예정)
* 📐 **Workflow & Conventions**: [./docs/CONVENTIONS.md](./docs/CONVENTIONS.md)
* 🤝 **Contributing Guide**: [./.github/CONTRIBUTING.md](./.github/CONTRIBUTING.md)
* 🚢 **Release Policy**: [./docs/RELEASE.md](./docs/RELEASE.md)
* 🧭 **Operating Guide**: [./docs/OPERATING.md](./docs/OPERATING.md)
* 🧪 **Issue/PR 템플릿**: [./.github/ISSUE_TEMPLATE/](./.github/ISSUE_TEMPLATE/), [./.github/PULL_REQUEST_TEMPLATE.md](./.github/PULL_REQUEST_TEMPLATE.md)

---

## 🧭 버전 및 브랜치 전략

* 버전: `{MAJOR}.{MINOR}.{BUILD}` (자세한 정책: [./docs/RELEASE.md](./docs/RELEASE.md)) / 핫픽스: `{버전}-{yyyy-MM-dd}-{N}`
* 브랜치: `master`, `develop`, `feature/<issue>-desc`, `release/x.y.z`, `hotfix/x.y.z-날짜-N`
