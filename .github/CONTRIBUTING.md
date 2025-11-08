# Contributing Guide 

> 이 문서는 **요약 규칙**만 담습니다. 상세 색상표/스크립트는 저장소 Settings → Labels에서 직접 관리합니다.

---

## 브랜치 전략 (요약)

* 기본 브랜치: `master`(안정) / `develop`(개발)
* 토픽: `feature/<issue>-desc`, `release/x.y.z`, `hotfix/x.y.z-YYYY-MM-DD-N`
* 머지: 일반 PR은 **Squash & Merge** 권장. 릴리스/핫픽스는 Merge commit 허용.

## 커밋/PR

* **커밋**: Conventional Commits 권장 `type(scope): subject` (`feat|fix|refactor|docs|test|chore|perf`). *scope*는 **선택**이며 `area` 또는 **컴포넌트/클래스**명을 사용합니다. 예) `feat(FPS): 레일 건 추가`, `fix(GunItemAdapter): 레일 건 탄환 수 비정상 적용 현상`

  * 한 줄 요약은 50자 내외, 끝에 마침표 X, 한국어는 **명사형/명령형** 모두 허용.
  * **fix 요약**은 “~현상/문제” 식으로 충분(“해결/수정” 단어 생략 OK). 상세 원인/조치/영향 범위는 **본문**에 기술.
  * 관련 이슈 링크: 본문에 `Resolves #<이슈>` 또는 `#<이슈>` 명시.
* **PR 제목**: (일반 PR) 커밋 요약을 반영(필요 시 scope 포함), 본문에 `Resolves #<이슈>` 포함. **배포 관련 PR(`release/*`, `hotfix/*`, `master` 병합)**은 다음 형식 사용: `release(x.y.z): cut release x.y.z` / `hotfix(x.y.z-YYYY-MM-DD-N): <요약>` / `merge(master): <요약>` — 상세 체크리스트/본문 템플릿은 [docs/RELEASE.md](../docs/RELEASE.md) 참조.
* **체크리스트**: 빌드/테스트 통과, AC 충족, 문서 업데이트

## 라벨 (원칙)

* 최소 조합: `type:*` + `area:*` (+ `priority:*` 선택)
* `type`은 이슈의 성격(기능/버그/부채/설계), `area`는 영향 영역(FPS/MOBA/Platform), `priority`는 긴급도입니다.
* **공존 규칙:** `type` 1개, `priority` 1개, **`area`도 1개만 사용**. 전체 프로젝트에 해당하는 과제는 `area:cross-service` **단일 라벨**을 사용하세요.
* 여러 영역이 실제로 필요하면 **서브태스크/스토리로 분할**하고, 부모 이슈 본문에 관련 이슈를 `#번호`로 링크합니다.

> 기본 우선순위는 `priority:normal` 입니다. 긴급 시 `priority:high/urgent`로 격상하세요.

## 보안 이슈

* 공개 이슈 대신 **비공개 제보**: [Security Policy](../.github/SECURITY.md) 참고

## 문서 링크

* 프로젝트 소개: [README](../README.md)
* 워크플로우/컨벤션: [docs/CONVENTIONS.md](../docs/CONVENTIONS.md)
* 릴리스 정책: [docs/RELEASE.md](../docs/RELEASE.md)
* 운영 가이드: [docs/OPERATING.md](../docs/OPERATING.md)
