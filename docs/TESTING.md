# Testing Strategy

> 이 문서는 **테스트 전략과 컨벤션**을 정의합니다. 빠르게 적용 가능한 최소 규칙(MVP) 위주로 구성합니다.

* 컨벤션: [./CONVENTIONS.md](./CONVENTIONS.md)
* 아키텍처: [./ARCHITECTURE.md](./ARCHITECTURE.md)
* 릴리스: [./RELEASE.md](./RELEASE.md)

---

## 1) 피라미드 & 커버리지(가이드)

* **Unit** ≥ 70% (핵심 규칙/도메인 로직)
* **Integration**: 저장소/외부 IO 경계 검증(필요한 만큼)
* **E2E/Contract**: 핵심 경로만(회귀 방지 목적)

> 커버리지 수치는 **참고용 지표**이며, 테스트 우선순위는 **핵심 경로**에 둡니다.

---

## 2) 프로젝트 구조

```
/tests
  /Unit            # 순수 로직 테스트 (Domain, Mapper, Policies)
  /Integration     # EF Core/웹 API 통합 (SQLite/InMemory, TestServer)
  /Contract        # API 계약/스냅샷/호환성 (선택)
```

* 네임스페이스/폴더는 **테스트 대상**과 미러링
* 파일명 규칙: `<TypeUnderTest>Tests.cs`
* 메서드 이름: `Method_Should_Behavior` 또는 `Scenario_Expected`

---

## 3) 단위 테스트 (Unit)

* 프레임워크: xUnit (기본)
* 목킹: NSubstitute or Moq (취향)
* 어설션: FluentAssertions 권장
* 도메인/정책은 **Clock/IdProvider** 등 외부 입력을 인터페이스로 분리해 주입 가능하게

예시

```csharp
public class PlayerTests
{
    [Fact]
    public void Register_Should_SetDefaultNickname()
    {
        var player = Player.Register("user@example.com", nickname: null);
        player.Nickname.Value.Should().Be("NewPlayer");
    }
}
```

---

## 4) 통합 테스트 (Integration)

* DB: **SQLite In-Memory** or **EFCore InMemory** (읽기/쓰기 동작 차이 주의)
* 웹: `WebApplicationFactory<TEntryPoint>`로 API 통합 테스트
* 마이그레이션 적용/시드 데이터는 **fixture**로 공용화

예시 (API)

```csharp
public class ApiFixture : WebApplicationFactory<Program> { }

public class PlayerApiTests : IClassFixture<ApiFixture>
{
    private readonly HttpClient _client;
    public PlayerApiTests(ApiFixture fx) => _client = fx.CreateClient();

    [Fact]
    public async Task Post_Player_Should_Return_201()
    {
        var res = await _client.PostAsJsonAsync("/api/fps/players", new { email = "u@ex.com" });
        res.StatusCode.Should().Be(HttpStatusCode.Created);
    }
}
```

예시 (EF Core with SQLite)

```csharp
public class DbFixture : IDisposable
{
    public AppDbContext Db { get; }
    private readonly SqliteConnection _conn;

    public DbFixture()
    {
        _conn = new SqliteConnection("DataSource=:memory:");
        _conn.Open();
        var opt = new DbContextOptionsBuilder<AppDbContext>()
            .UseSqlite(_conn)
            .Options;
        Db = new AppDbContext(opt);
        Db.Database.EnsureCreated();
    }

    public void Dispose() => _conn.Dispose();
}
```

---

## 5) 계약/스냅샷 테스트 (선택)

* API 스키마가 외부 소비자에 중요하면 **Contract Test**로 고정
* 스냅샷(Json) 비교 시 **의미 필드만** 비교(시간/ID 제외)

---

## 6) 테스트 데이터 & 픽스처

* **Builder/Factory** 패턴으로 의미 있는 기본값 제공 (`PlayerBuilder`)
* 픽스처 수명: `CollectionFixture`로 테스트 간 DB/서버 재사용
* 랜덤 데이터는 Bogus(선택)

---

## 7) CI 가이드

* 파이프라인: Build → Test → (선택) Coverage → Lint/Analyzer
* 병렬화: 프로젝트/컬렉션 기준 병렬, DB 공유 시 주의
* 실패 시: 실패 테스트 로그/아티팩트 업로드

---

## 8) 품질 바

> 장기적으로 **테스트 네이밍 전략**을 본 섹션(또는 *테스트 탐색 구조* 가이드)에 **명문화**합니다. 예: *테스트 메서드는 반드시 도메인 용어를 포함한다*.

* PR은 **최소 1개 테스트 변경/추가** 포함 권장
* 핵심 도메인 규칙은 **경계 조건**(null/빈/최대값)까지 커버
* 회귀 버그는 **재현 테스트** 우선 추가

---

## 8.5) 테스트 탐색 구조(권장)

* 구조: **BDD + 중첩 1단계 + partial 분할**
* 표시: `[DisplayName("…한글…")]` 고정, 가로 분류는 `[Trait("Feature","…")]` 등 Trait 사용
* 파일: **영어 유지**(예: `Player_Registration.Success.cs`), 논리 트리는 **한글**(중첩/DisplayName)
* 중첩은 **2단계 이내**(기능 → 시나리오). 더 깊어지면 파일/클래스로 분리

## 9) 링크

* 컨벤션: [./CONVENTIONS.md](./CONVENTIONS.md)
* 아키텍처: [./ARCHITECTURE.md](./ARCHITECTURE.md)
* 운영: [./OPERATING.md](./OPERATING.md)
