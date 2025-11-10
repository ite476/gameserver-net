# Workflow & Conventions

> 이 문서는 **작업 흐름**(Workflow)과 **작업 컨벤션**을 정의합니다. 프로젝트에서 바로 적용 가능한 최소 규칙(MVP)을 우선으로 서술하며, 세부 가이드는 연결 문서에서 확장합니다.

* 요약 가이드: [../.github/CONTRIBUTING.md](../.github/CONTRIBUTING.md)
* 릴리스 정책: [./RELEASE.md](./RELEASE.md)
* 운영 가이드: [./OPERATING.md](./OPERATING.md)
* 이슈/PR 템플릿: [../.github/ISSUE_TEMPLATE/](../.github/ISSUE_TEMPLATE/), [../.github/PULL_REQUEST_TEMPLATE/](../.github/PULL_REQUEST_TEMPLATE/)
* 보안 정책: [../.github/SECURITY.md](../.github/SECURITY.md)
* CHANGELOG 템플릿: [./templates/CHANGELOG_TEMPLATE.md](./templates/CHANGELOG_TEMPLATE.md)

---

## 1) 브랜치 전략

* 기본: `master`(안정) / `develop`(개발)
* 릴리스: `release/x.y.z` (안정화 후 `master` 병합 → 태그 → `develop` 역병합)
* 핫픽스: `hotfix/x.y.z-YYYY-MM-DD-N` (문제 해결 후 `master` → 태그 → `develop` 역병합)
* 토픽: `feature/<issue>-desc`, 필요 시 `docs/<issue>-desc`, `fix/<issue>-desc`, `chore/*`, `refactor/*`
* 이슈 연동 네이밍 규칙: `type/<issue-number>-short-desc`
  - 예: `docs/2-docs-baseline`, `feature/45-fps-player-upsert`, `fix/101-hotfix-jwt-expiry`
  - 주의: 브랜치명에는 `#` 기호를 포함하지 않습니다 (GitHub/UI/CLI 호환성)
* Merge 전략: 일반 PR = **Squash**, 릴리스/핫픽스 PR = **Merge commit 허용**

### 브랜치 보호(권장)

* `master`, `develop`, `release/*`, `hotfix/*`: 리뷰 1+, CI 통과, force-push 금지

---

## 2) 이슈 & 라벨

* 최소 라벨 조합: `type:*` + `area:*` (+ `priority:*` 선택)
* **공존 규칙:** `type` 1개, `priority` 1개, **`area`** **1개만 사용**
* 전체 프로젝트에 해당하면 `area:cross-service` 단일 라벨 사용
* 상세 규칙: [GitHub Issue Templates](../.github/ISSUE_TEMPLATE/), 색상표는 GitHub Settings → Labels에서 관리

예시

* 기능(FPS): `type:feature` `area:fps` `priority:normal`
* 버그(플랫폼, 긴급): `type:bug` `area:platform` `priority:urgent`

---

## 3) 커밋 & PR

* Conventional Commits: `type(scope): subject`

  * scope = 선택 (예: `FPS`, `GunItemAdapter`)
  * 제목: 50자 내외, 마침표 X, 한국어 명사/명령형 모두 허용
  * **fix 제목**: "~현상/문제"만으로 충분 (해결/수정 단어 생략 OK)
* PR 제목 규칙:

  * 일반 PR: `<commit-type>(<optional-interest>): <summary>`
    * `commit-type`: 커밋 타입 (`feat`, `fix`, `refactor`, `docs`, `test`, `chore`, `perf` 등)
    * `interest`: 선택, 영향 영역 또는 컴포넌트 (예: `project`, `FPS`, `Platform`)
    * `summary`: 변경 요약 (50자 내외, 마침표 X)
    * 이슈 번호는 PR 본문의 "참고" 섹션에서 처리 (제목에 포함 시 Squash merge 시 중첩 문제)
    * 예시: `chore(project): 프로젝트 기초 구조 세팅`, `feat(FPS): 레일 건 추가`
  * 배포 관련 PR:
    * Release: `release(x.y.z): 정규 업데이트`
    * Hotfix: `hotfix(x.y.z-YYYY-MM-DD-N): <한 줄 요약>`
    * Master 병합(필요시): `merge(master): <한 줄 요약>`
* **이슈-PR 연결 및 자동 Close 규칙:**
  * **Feature PR 단계**: 이슈 연결은 하지만 **자동 Close 하지 않음**
    * GitHub PR의 Development 섹션에서 "연결"만 설정 (자동 Close 옵션 비활성화)
    * 또는 PR 본문에 `#<이슈번호>` 언급 (단순 연결)
    * 이유: 작업이 완료되지 않았고, 여러 PR로 분할될 수 있음
  * **Release PR 단계**: 최종적으로 `release/x.y.z` → `master` PR에서 **자동 Close**
    * Release PR 본문에 `Fixes #<이슈번호>`, `Closes #<이슈번호>`, `Resolves #<이슈번호>` 사용
    * 또는 GitHub PR의 Development 섹션에서 "이 PR 완성되면 이 이슈를 닫습니다" 설정
    * 이유: 마일스톤 단위로 작업이 완료되어 릴리스에 포함됨
* PR 템플릿: [default](../.github/PULL_REQUEST_TEMPLATE/DEFAULT.md) · [release](../.github/PULL_REQUEST_TEMPLATE/RELEASE.md) · [hotfix](../.github/PULL_REQUEST_TEMPLATE/HOTFIX.md)

---

## 4) 버저닝 & 태깅

* `{MAJOR}.{MINOR}.{BUILD}` / 핫픽스 `{MAJOR}.{MINOR}.{BUILD}-{yyyy-MM-dd}-{N}`
* 태그는 릴리스/핫픽스 형식 그대로 생성 (예: `0.2.0`, `5.5.5-2025-10-15-3`)
* 상세: [Release Policy](./RELEASE.md)

---

## 5) 솔루션 구조 (.NET)

* 목표: **API ⇆ Application ⇆ Domain ⇆ Infrastructure** 경계 명확화
* 예시 폴더(초안):

```
/src
  /Api           # ASP.NET Core Web API (Controllers, Filters, Middlewares)
  /Application   # UseCases/Services, DTO, Validators
  /Domain        # Entities/Aggregates, Value Objects, Domain Events, Specifications
  /Infrastructure# EF Core / Dapper / Outbox / Integrations (Redis, Kafka, gRPC, etc.)
/tests
  /Unit
  /Integration
  /Contract      # API contract tests (optional)
```

*상세 폴더 구조/파일명 규칙은 **[./ARCHITECTURE.md](./ARCHITECTURE.md)**에서 관리하며, 변경 시 본 문서와 동기화합니다.*

### 의존성 규칙

* Api → Application → Domain (단방향)
* Infrastructure는 Domain, Application 인터페이스 구현체 제공
* **Domain은 외부 참조 금지** (순수성 유지)

---

## 6) 코딩 규약 (C# / ASP.NET Core)

* Target: **.NET 8 LTS**(또는 .NET 9 미리보기 사용 시 명시)
* Nullable Reference Types 활성화, `async/await` 우선
* DI: Constructor Injection, **IOptions**로 설정 바인딩
* 컨트롤러는 **Thin**, 유스케이스는 **Application**에 배치
* DTO ↔ Domain 매핑은 **Application 레이어의 Mapper**에서 수행 (권장: Mapster 코드생성 또는 수동 Mapper). *Domain은 DTO를 알지 않음* — 필요 시 `From*`/`To*` 패턴은 **Application 확장메서드/Assembler**에서만 사용.
* 에러 처리: ProblemDetails(JSON), 도메인 예외 → 4xx, 시스템 예외 → 5xx
* API 버전관리: URL or Header 버전(선택), 최소 1 메이저 유지

---

## 7) 데이터 & 트랜잭션

* Repository/UnitOfWork 패턴(필요 시), EF Core 기본
* Outbox 패턴(요약): **at-least-once** 이벤트 발행으로 dual-write 불일치 방지. 상세는 Infrastructure 컨벤션에서 다룹니다.
* 분산락은 **최소화**. 필요 시 Redis Redlock/SQL App Lock 등 **명시적 제한된 범위**로 적용
* 마이그레이션: EF Core Migrations, **전방 호환** 우선 (롤백 어려움)

*자세한 스키마/인덱스/EF 매핑/Dispatcher 설정은 ****Infrastructure**** 쪽 상세 컨벤션에서 다룹니다.*

---

## 8) 성능 & 신뢰성

* SLO(초안): p95 응답 200ms(로컬 기준), 에러율 < 1%
* 캐싱: 읽기 캐시(메모리/Redis), Idempotency Key(필요 시)
* 재시도/서킷브레이커(정책 기반): Polly (또는 미들웨어)

---

## 9) 테스트 전략

* 테스트 피라미드: Unit 70%+, Integration 적정, E2E는 핵심 시나리오만
* 컨벤션: Given/When/Then, 이름 `Method_Should_Behavior`
* PR은 최소 1개 테스트 변경/추가 동반 권장

---

## 10) 관측(Observability) *초안*

* 로깅: Serilog(파일/콘솔) → (선택) OpenTelemetry Exporter
* 메트릭: OpenTelemetry Metrics, p95/에러율 기본지표
* 트레이싱: OpenTelemetry Trace, 주요 UseCase/외부 I/O 구간

> 세부 설계는 별도 이슈에서 확정 후 본 문서 업데이트 예정

---

## 11) 보안 & 비밀정보

* Secrets는 코드에 포함 금지, **User Secret**(로컬) / **KeyVault/Parameter Store**(배포)
* 민감 설정은 `IOptions<T>` + 환경변수/매개변수로 주입
* 보안 이슈: [SECURITY.md](../.github/SECURITY.md)

---

## 12) CI/CD (초안)

* CI: Build → Test → Lint → (선택) SCA/CodeQL
* 배포: 브랜치별 자동화 (dev=develop, staging=release/*, prod=master tag)
* 아티팩트: Docker 이미지 태그 = 버전/커밋 SHA

---

## 13) 코드 리뷰 체크리스트

* 설계 적합성(API/App/Domain/Infra 경계 준수)
* 가독성/네이밍/주석(필수 지점만)
* 오류 처리/예외/로그 레벨 적정
* 성능/쿼리 수/캐시 기회
* 테스트 커버리지/관계없는 변경 포함 여부

---

## 14) 문서화 & ADR

* ADR: `docs/adr/0001-title.md` **연속 번호 체계**. `master` 병합 시 최종 번호 확인/정렬(또는 추후 일괄 정리). 이슈/PR 링크는 본문 frontmatter에 기록.
* README(루트): 프로젝트 목적/스택/링크 요약
* CHANGELOG: 루트 `./CHANGELOG.md`(연도별 아카이브는 추후 결정)

---

## 15) 예시 커밋 메시지

```
feat(FPS): Player Upsert API 추가
fix(GunItemAdapter): 레일 건 탄환 수 비정상 적용 현상
refactor(Application): 유스케이스 인터페이스 분리
chore: CI 빌드 캐시 조정
```

---

## 16) FAQ 

* Q. `area` 여러 개 붙여도 되나요? → A. **아니요, 1개만.** 전체 프로젝트 과제면 `area:cross-service`.
* Q. `develop` 롤백은? → A. 브랜치 강제 되감기 대신 **Revert/Hotfix/Cherry-pick**을 사용.
* Q. 분산락 써도 되나요? → A. 꼭 필요한 범위에 한정해 사용(락 경합/데드락 주의). 설계 리뷰 필수.
* Q. CHANGELOG는 언제 업데이트하나요? → A. **릴리스 단계에서 통합 업데이트**합니다. Feature PR 단계에서는 체크리스트를 체크하지 않고, `release/x.y.z` → `master` PR 생성 전에 마일스톤 단위로 통합 정리합니다. 이유: 불완전한 기능 기록 방지, 중복/누락 방지, 릴리스 체크리스트와 일치.
* Q. 작은 작업(문서 업데이트, 컨벤션 정리 등)도 이슈를 만들어야 하나요? → A. **아니요, 이슈 없이 PR로 바로 처리 가능**합니다. PR 자체가 작업 요청과 해결을 모두 포함하므로 충분합니다. 큰 기능이나 논의가 필요한 작업만 이슈를 만듭니다.
* Q. 실수로 잘못된 브랜치에 머지한 PR이나 리버트 PR도 이슈를 만들어야 하나요? → A. **아니요, 새 이슈 생성 불필요**합니다. 이미 처리 완료된 작업이므로 PR 히스토리로 충분히 추적 가능합니다. 다만, 기존 이슈에서 파생된 작업이라면 GitHub PR의 Development 섹션에서 수동으로 연결하거나, PR 본문에 `#<이슈번호>`를 언급하여 연결하는 것을 권장합니다.
* Q. 이슈는 언제 자동으로 닫히나요? → A. **Release PR 단계에서만 자동 Close**합니다. Feature PR 단계에서는 이슈를 연결만 하고 자동 Close는 하지 않습니다 (작업이 완료되지 않았고, 여러 PR로 분할될 수 있음). 최종적으로 `release/x.y.z` → `master` PR에서 `Fixes #`, `Closes #`, `Resolves #` 키워드 사용하거나 GitHub PR의 Development 섹션에서 "이 PR 완성되면 이 이슈를 닫습니다" 옵션을 활성화하여 자동 Close 처리합니다.
* Q. 솔루션 파일의 `ProjectConfigurationPlatforms` 섹션이 필요한가요? → A. **Visual Studio 사용 시 유지 권장**합니다. .NET SDK 스타일 프로젝트는 자동 매핑되지만, VS 테스트 탐색기 및 빌드 구성 관리 안정성을 위해 명시적 매핑이 더 안정적입니다. CLI 중심 작업만 하는 경우 제거해도 되지만, VS가 자동 재생성할 수 있습니다.

---

## 17) 마일스톤 네이밍 규칙

* 목적: 릴리스 목표 단위로 이슈/PR를 묶고 진행률을 추적하기 위함
* 제목(Title): `{MAJOR}.{MINOR}.{BUILD} - {Title Alias In English}`
  - 예: `0.1.0 - Docs Baseline`, `0.2.0 - FPS Skeleton`, `0.3.0 - MOBA Skeleton`
  - 규칙: 버전은 SemVer, 알리아스는 영문(ASCII) 권장 — 검색/자동화 안정성
* 설명(Description): 한국어로 상세 작성
  - 포함 항목 권장: 목적/범위, 산출물(파일 링크), 체크리스트(AC), 후속 이슈
  - 예 링크: `[docs/OPERATING.md](docs/OPERATING.md)` 등 레포 상대 경로 사용
* 운영 팁
  - 마감일(Due date)·담당자는 필요 시 설정
  - 마일스톤은 태그(Release)와 1:1을 권장하되, 필요 시 누적형도 허용
