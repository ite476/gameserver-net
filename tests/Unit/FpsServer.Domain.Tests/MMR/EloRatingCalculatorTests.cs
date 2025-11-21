using FluentAssertions;
using FpsServer.Domain.MMR;
using DomainMMR = FpsServer.Domain.Matchmaking.MMR;
using Xunit;

namespace FpsServer.Domain.Tests.MMR;

[Trait("Feature", "MMR 계산")]
public class EloRatingCalculatorTests
{
    private readonly EloRatingCalculator _calculator;

    public EloRatingCalculatorTests()
    {
        _calculator = new EloRatingCalculator();
    }

    [Fact]
    [Trait("Category", "Elo 레이팅 계산")]
    public void 승리_시_MMR이_증가해야_한다()
    {
        // Arrange
        var currentMMR = new DomainMMR(1500);
        var opponentMMR = new DomainMMR(1500); // 동일한 MMR
        var actualScore = 1.0; // 승리
        var kFactor = 32;

        // Act
        var newMMR = _calculator.CalculateNewMMR(currentMMR, opponentMMR, actualScore, kFactor);

        // Assert
        newMMR.Value.Should().BeGreaterThan(currentMMR.Value);
        // 동일한 MMR에서 승리 시 약 16점 증가 (K=32, Expected=0.5, Actual=1.0)
        newMMR.Value.Should().Be(1516);
    }

    [Fact]
    [Trait("Category", "Elo 레이팅 계산")]
    public void 패배_시_MMR이_감소해야_한다()
    {
        // Arrange
        var currentMMR = new DomainMMR(1500);
        var opponentMMR = new DomainMMR(1500); // 동일한 MMR
        var actualScore = 0.0; // 패배
        var kFactor = 32;

        // Act
        var newMMR = _calculator.CalculateNewMMR(currentMMR, opponentMMR, actualScore, kFactor);

        // Assert
        newMMR.Value.Should().BeLessThan(currentMMR.Value);
        // 동일한 MMR에서 패배 시 약 16점 감소 (K=32, Expected=0.5, Actual=0.0)
        newMMR.Value.Should().Be(1484);
    }

    [Fact]
    [Trait("Category", "Elo 레이팅 계산")]
    public void 높은_MMR_상대에게_승리하면_큰_폭으로_증가해야_한다()
    {
        // Arrange
        var currentMMR = new DomainMMR(1500);
        var opponentMMR = new DomainMMR(1700); // 높은 MMR 상대
        var actualScore = 1.0; // 승리
        var kFactor = 32;

        // Act
        var newMMR = _calculator.CalculateNewMMR(currentMMR, opponentMMR, actualScore, kFactor);

        // Assert
        var mmrIncrease = newMMR.Value - currentMMR.Value;
        mmrIncrease.Should().BeGreaterThan(16); // 동일 MMR 승리보다 더 큰 증가
        // Expected Score가 낮으므로 (약 0.24), 실제 점수와의 차이가 커서 더 큰 증가
    }

    [Fact]
    [Trait("Category", "Elo 레이팅 계산")]
    public void 낮은_MMR_상대에게_패배하면_큰_폭으로_감소해야_한다()
    {
        // Arrange
        var currentMMR = new DomainMMR(1500);
        var opponentMMR = new DomainMMR(1300); // 낮은 MMR 상대
        var actualScore = 0.0; // 패배
        var kFactor = 32;

        // Act
        var newMMR = _calculator.CalculateNewMMR(currentMMR, opponentMMR, actualScore, kFactor);

        // Assert
        var mmrDecrease = currentMMR.Value - newMMR.Value;
        mmrDecrease.Should().BeGreaterThan(16); // 동일 MMR 패배보다 더 큰 감소
        // Expected Score가 높으므로 (약 0.76), 실제 점수와의 차이가 커서 더 큰 감소
    }

    [Fact]
    [Trait("Category", "Elo 레이팅 계산")]
    public void 낮은_MMR_상대에게_승리하면_작은_폭으로_증가해야_한다()
    {
        // Arrange
        var currentMMR = new DomainMMR(1500);
        var opponentMMR = new DomainMMR(1300); // 낮은 MMR 상대
        var actualScore = 1.0; // 승리
        var kFactor = 32;

        // Act
        var newMMR = _calculator.CalculateNewMMR(currentMMR, opponentMMR, actualScore, kFactor);

        // Assert
        var mmrIncrease = newMMR.Value - currentMMR.Value;
        mmrIncrease.Should().BeLessThan(16); // 동일 MMR 승리보다 작은 증가
        // Expected Score가 높으므로 (약 0.76), 실제 점수와의 차이가 작아서 작은 증가
    }

    [Fact]
    [Trait("Category", "Elo 레이팅 계산")]
    public void 높은_MMR_상대에게_패배하면_작은_폭으로_감소해야_한다()
    {
        // Arrange
        var currentMMR = new DomainMMR(1500);
        var opponentMMR = new DomainMMR(1700); // 높은 MMR 상대
        var actualScore = 0.0; // 패배
        var kFactor = 32;

        // Act
        var newMMR = _calculator.CalculateNewMMR(currentMMR, opponentMMR, actualScore, kFactor);

        // Assert
        var mmrDecrease = currentMMR.Value - newMMR.Value;
        mmrDecrease.Should().BeLessThan(16); // 동일 MMR 패배보다 작은 감소
        // Expected Score가 낮으므로 (약 0.24), 실제 점수와의 차이가 작아서 작은 감소
    }

    [Fact]
    [Trait("Category", "K 값 조정")]
    public void K_값을_조정하면_증감_폭이_변화해야_한다()
    {
        // Arrange
        var currentMMR = new DomainMMR(1500);
        var opponentMMR = new DomainMMR(1500);
        var actualScore = 1.0; // 승리
        var kFactor16 = 16;
        var kFactor32 = 32;
        var kFactor64 = 64;

        // Act
        var newMMR16 = _calculator.CalculateNewMMR(currentMMR, opponentMMR, actualScore, kFactor16);
        var newMMR32 = _calculator.CalculateNewMMR(currentMMR, opponentMMR, actualScore, kFactor32);
        var newMMR64 = _calculator.CalculateNewMMR(currentMMR, opponentMMR, actualScore, kFactor64);

        // Assert
        var increase16 = newMMR16.Value - currentMMR.Value;
        var increase32 = newMMR32.Value - currentMMR.Value;
        var increase64 = newMMR64.Value - currentMMR.Value;

        increase16.Should().Be(8);  // K=16: 8점 증가
        increase32.Should().Be(16);  // K=32: 16점 증가
        increase64.Should().Be(32);  // K=64: 32점 증가

        increase64.Should().BeGreaterThan(increase32);
        increase32.Should().BeGreaterThan(increase16);
    }

    [Fact]
    [Trait("Category", "예외 처리")]
    public void actualScore가_범위를_벗어나면_ArgumentException을_발생시켜야_한다()
    {
        // Arrange
        var currentMMR = new DomainMMR(1500);
        var opponentMMR = new DomainMMR(1500);

        // Act & Assert
        var act1 = () => _calculator.CalculateNewMMR(currentMMR, opponentMMR, -0.1, 32);
        act1.Should().Throw<ArgumentException>()
            .WithMessage("*Actual score must be between 0.0 and 1.0*");

        var act2 = () => _calculator.CalculateNewMMR(currentMMR, opponentMMR, 1.1, 32);
        act2.Should().Throw<ArgumentException>()
            .WithMessage("*Actual score must be between 0.0 and 1.0*");
    }

    [Fact]
    [Trait("Category", "예외 처리")]
    public void kFactor가_0_이하면_ArgumentException을_발생시켜야_한다()
    {
        // Arrange
        var currentMMR = new DomainMMR(1500);
        var opponentMMR = new DomainMMR(1500);

        // Act & Assert
        var act1 = () => _calculator.CalculateNewMMR(currentMMR, opponentMMR, 1.0, 0);
        act1.Should().Throw<ArgumentException>()
            .WithMessage("*K factor must be greater than 0*");

        var act2 = () => _calculator.CalculateNewMMR(currentMMR, opponentMMR, 1.0, -1);
        act2.Should().Throw<ArgumentException>()
            .WithMessage("*K factor must be greater than 0*");
    }

    [Fact]
    [Trait("Category", "MMR 최소값 보장")]
    public void 계산_결과가_음수가_되면_0으로_보정해야_한다()
    {
        // Arrange
        var currentMMR = new DomainMMR(10); // 낮은 MMR
        var opponentMMR = new DomainMMR(2000); // 매우 높은 MMR
        var actualScore = 0.0; // 패배
        var kFactor = 32;

        // Act
        var newMMR = _calculator.CalculateNewMMR(currentMMR, opponentMMR, actualScore, kFactor);

        // Assert
        newMMR.Value.Should().BeGreaterThanOrEqualTo(0);
    }
}

