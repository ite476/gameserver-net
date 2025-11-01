### Summary

레지스트리 선택 및 CI 이미지 Push 최소 구성

### Acceptance Criteria

- [ ] 레지스트리 선택(GHCR 권장) 및 네임스페이스 규칙 확정
- [ ] CI 로그인/푸시(변수: `REGISTRY`, `IMAGE_NAME`, `IMAGE_TAG`)
- [ ] 레포 시크릿에 자격 증명 저장


### Steps

- [ ] 레지스트리 결정 및 네이밍 규칙 문서화
- [ ] CI 워크플로우에 로그인/푸시 단계 추가
- [ ] `docs/DEPLOYMENT.md`/`docs/OPERATING.md`에 변수 설명 업데이트

### Links

- 운영 계획: [docs/OPERATING.md](docs/OPERATING.md)
- 배포 가이드: [docs/DEPLOYMENT.md](docs/DEPLOYMENT.md)