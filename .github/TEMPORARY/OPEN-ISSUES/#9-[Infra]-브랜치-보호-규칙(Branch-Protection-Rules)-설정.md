### Summary

브랜치 보호 규칙(Rulesets) 설정으로 불필요한 삭제/강제 푸시 방지 및 일관된 협업 프로세스 확립


### Acceptance Criteria

- [ ] master, develop, release/*, hotfix/* 브랜치 보호 규칙 적용
- [ ] 강제 푸시 방지 기본 활성화
- [ ] 브랜치 삭제 방지 활성화
- [ ] 예외 계정/토큰 설정(필요 시)
- [ ] 문서화(docs/OPERATING.md 또는 별도 문서) 추가

### Steps

- [ ] GitHub Settings → Rules → Branch protection rules 생성
- [ ] 패턴: `master`, `develop`, `release/*`, `hotfix/*`
- [ ] 옵션: "Prevent force pushes" 활성화
- [ ] 옵션: "Deleting branches is prevented" 활성화
- [ ] 예외 계정/토큰(선택): 별도 관리 계정 또는 Personal Access Token 준비
- [ ] 문서화: 룰셋 설정 가이드 및 예외 프로세스 정리

### Links

- 운영 가이드: [docs/OPERATING.md](docs/OPERATING.md)
- 브랜치 전략: [docs/CONVENTIONS.md](docs/CONVENTIONS.md)
- 보안: [.github/SECURITY.md](.github/SECURITY.md)