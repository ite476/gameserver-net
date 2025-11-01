### Summary

마크다운 링크 유효성 검사 CI(Job) 도입

### Acceptance Criteria

- [ ] 링크 유효성 검사 워크플로우 추가
- [ ] PR에서 자동 실행(변경 문서 기준), 결과 코멘트 제공
- [ ] README에 배지(선택)


### Steps

- [ ] `.github/workflows/link-check.yml` 추가(lychee 또는 markdown-link-check)
- [ ] 외부 도메인 타임아웃/재시도/허용 리스트 최소 설정
- [ ] README 배지 추가(선택)


### Links

- 컨벤션: [docs/CONVENTIONS.md](docs/CONVENTIONS.md)
- 운영: [docs/OPERATING.md](docs/OPERATING.md)