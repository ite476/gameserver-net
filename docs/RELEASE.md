# Release Policy

> 이 문서는 **릴리스/핫픽스 운영 기준**과 **PR 양식**을 정의합니다. 일반 PR 규칙은 [.github/CONTRIBUTING.md](../.github/CONTRIBUTING.md) 참고.

---

## 1) 버저닝 규칙

* **정규 버전:** `{MAJOR}.{MINOR}.{BUILD}`

  * `MAJOR`: 호환성 깨짐
  * `MINOR`: 하위호환 기능 추가
  * `BUILD`: 패치/버그픽스
* **핫픽스 버전:** `{MAJOR}.{MINOR}.{BUILD}-{yyyy-MM-dd}-{N}`

  * 예: `5.5.5-2025-10-15-3` (해당 날짜 **3번째** 핫픽스)
* 태그는 **정규/핫픽스 형식 그대로** 생성합니다. (예: `0.2.0`, `5.5.5-2025-10-15-3`)

---

## 2) 브랜치/플로우

* 정규 릴리스: `develop` → `release/x.y.z` → 안정화 → `master` 머지 → 태그 → `develop` **역병합**
* 핫픽스: `master` → `hotfix/x.y.z-YYYY-MM-DD-N` → `master` 머지 → 태그 → `develop` **역병합**
* 병합 전략: 릴리스/핫픽스 PR은 **Merge commit 허용**, 일반 PR은 Squash 권장

---

## 3) PR 제목 규칙

* **Release:**

  * `release(x.y.z): 정규 업데이트`
* **Hotfix:**

  * `hotfix(x.y.z-YYYY-MM-DD-N): <한 줄 요약>`
* **Master 병합(필요시):**

  * `merge(master): <한 줄 요약>`

> 일반 기능 PR의 제목/본문 규칙은 [CONTRIBUTING](../.github/CONTRIBUTING.md) 참고.

---

## 4) PR 템플릿 위치

* 일반 PR: [../.github/PULL_REQUEST_TEMPLATE/DEFAULT.md](../.github/PULL_REQUEST_TEMPLATE/DEFAULT.md)
* 릴리스 PR: [../.github/PULL_REQUEST_TEMPLATE/RELEASE.md](../.github/PULL_REQUEST_TEMPLATE/RELEASE.md)
* 핫픽스 PR: [../.github/PULL_REQUEST_TEMPLATE/HOTFIX.md](../.github/PULL_REQUEST_TEMPLATE/HOTFIX.md)

> 새 PR 작성 시 템플릿 선택 드롭다운에서 고르거나, URL에 `?template=RELEASE.md` 식으로 지정할 수 있습니다.

---

## 5) 체크리스트 (릴리스/핫픽스 공통)

### ✅ 필수(MVP)

- [ ] **버전 번호 업데이트** (어셈블리/패키지/이미지 태그 일관)

- [ ] **테스트/빌드/정적분석 통과** (CI 상태 확인)

- [ ] **CHANGELOG.md 갱신** (루트 또는 서비스별)

- [ ] **문서 업데이트** (README/OPERATING/CONVENTIONS 등 필요 시)

- [ ] **태그 생성 및 푸시** (예: `x.y.z`, `x.y.z-YYYY-MM-DD-N`)

- [ ] **배포 후 Smoke Test** 수행 및 결과 코멘트

- [ ] **develop 역병합**(핫픽스 포함)

### ✨ 선택(확장)

- [ ] **DB 마이그레이션 검토** (전방 호환/롤백 전략/스크립트 포함)

- [ ] **Feature Flag 적용/기본값 확인** (안전한 기본값, 단계적 On)

- [ ] **모니터링/알람 확인** (대시보드/SLI·SLO/경고 룰 준비)

- [ ] **성능 예산 점검** (p95 지연/에러율/자원 사용)

- [ ] **롤백 플랜 명시** (직전 태그/이미지로 재배포 경로)

- [ ] **리스크/영향 범위 정리** (서비스/엔드포인트/DB)

- [ ] **승인/검토 이력** (리뷰어, QA/스테이징 사인오프)

- [ ] **릴리스 노트** (사용자 영향·Deprecated 항목 포함)

### 🩹 Hotfix 전용 추가

- [ ] **현상 한 줄 요약** (제목에는 “~현상/문제”만, “해결/수정” 생략 OK)

- [ ] **Root Cause & Fix** (원인과 조치 요약)

- [ ] **회귀 테스트 추가** (재발 방지)

- [ ] **관련 인시던트/이슈 링크** (있으면)

---

## 6) 태깅 규칙

* 정규: `git tag x.y.z && git push origin x.y.z`
* 핫픽스: `git tag x.y.z-YYYY-MM-DD-N && git push origin x.y.z-YYYY-MM-DD-N`

> 태그는 릴리스 PR 머지 직후 `master` 기준으로 생성합니다.

---

## 7) 프리즈/커트오프 (선택)

* **적용 여부:** 릴리스 상황에 따라 필요 시 적용합니다.
* **권장 예시:** 코드 프리즈(릴리스 전 약 24h, `release/*`는 버그픽스만), 커트오프(배포 당일 약 2h 전 신규 변경 머지 중단).

---

## 8) CHANGELOG 위치 & 템플릿

* **관리 여부:** 형상관리 함. 릴리스/핫픽스마다 기록합니다.
* **위치**

  * 루트 공통: `./CHANGELOG.md` (**필수**)
  * 서비스별: `./src/Services/<Service>/CHANGELOG.md` (**선택**) — 서비스 특화 변경이 많은 경우만 분리
* **작성 규칙**

  * 새 항목은 **문서 맨 위**에 추가(최신이 위)
  * 제목: `## [x.y.z] - YYYY-MM-DD`
  * 섹션: `### Added` / `### Changed` / `### Fixed` / `### Deprecated` / `### Removed` / `### Security`
  * 각 항목은 불릿, 끝에 관련 이슈 `(#123)`를 명시
* **템플릿:** [./templates/CHANGELOG_TEMPLATE.md](./templates/CHANGELOG_TEMPLATE.md)

---

## 9) 보안 패치

* 보안 이슈는 공개 이슈 대신 **비공개로 제보**: [.github/SECURITY.md](../.github/SECURITY.md)
* 긴급 패치는 **핫픽스 플로우** 사용

---

## 10) 참고 링크

* 요약 규칙: [.github/CONTRIBUTING.md](../.github/CONTRIBUTING.md)
* 운영 가이드: [docs/OPERATING.md](./OPERATING.md)

---

## 11) 마일스톤 네이밍 규칙

* 제목(Title): `{MAJOR}.{MINOR}.{BUILD} - {Title Alias In English}`
  - 예: `0.1.0 - Docs Baseline`, `0.2.0 - FPS Skeleton`
* 설명(Description): 한국어 상세 서술(목적/범위/산출물/체크리스트/후속)
* 릴리스와의 관계: 마일스톤 1:1 태그(Release)를 권장
* 링크 표기: 레포 상대 경로 사용(예: `[docs/ARCHITECTURE.md](docs/ARCHITECTURE.md)`)
