using FluentAssertions;
using FpsServer.Domain.Matchmaking;
using Xunit;

namespace FpsServer.Domain.Tests.Matchmaking;

[Trait("Feature", "매치메이킹")]
public class MMRTests
{
    [Fact]
    [Trait("Category", "MMR 값 객체")]
    public void 유효한_값이면_MMR을_생성해야_한다()
    {
        // Arrange & Act
        var mmr = new MMR(1500);
        
        // Assert
        mmr.Value.Should().Be(1500);
    }
    
    [Fact]
    [Trait("Category", "MMR 값 객체")]
    public void 음수_값이면_ArgumentException을_발생시켜야_한다()
    {
        // Arrange & Act
        var act = () => new MMR(-1);
        
        // Assert
        act.Should().Throw<ArgumentException>()
            .WithMessage("MMR cannot be negative*")
            .And.ParamName.Should().Be("value");
    }
    
    [Fact]
    [Trait("Category", "MMR 값 객체")]
    public void 영_값이면_MMR을_생성해야_한다()
    {
        // Arrange & Act
        var mmr = new MMR(0);
        
        // Assert
        mmr.Value.Should().Be(0);
    }
    
    [Fact]
    [Trait("Category", "MMR 값 객체")]
    public void 델타를_더하면_MMR이_증가해야_한다()
    {
        // Arrange
        var mmr = new MMR(1500);
        
        // Act
        var result = mmr + 50;
        
        // Assert
        result.Value.Should().Be(1550);
    }
    
    [Fact]
    [Trait("Category", "MMR 값 객체")]
    public void 델타를_빼면_MMR이_감소해야_한다()
    {
        // Arrange
        var mmr = new MMR(1500);
        
        // Act
        var result = mmr - 50;
        
        // Assert
        result.Value.Should().Be(1450);
    }
    
    [Fact]
    [Trait("Category", "MMR 값 객체")]
    public void 두_MMR의_차이를_계산하면_정수값을_반환해야_한다()
    {
        // Arrange
        var mmr1 = new MMR(1500);
        var mmr2 = new MMR(1450);
        
        // Act
        var result = mmr1 - mmr2;
        
        // Assert
        result.Should().Be(50);
    }
    
    [Fact]
    [Trait("Category", "MMR 값 객체")]
    public void 두_MMR의_절대_차이를_계산하면_양수값을_반환해야_한다()
    {
        // Arrange
        var mmr1 = new MMR(1500);
        var mmr2 = new MMR(1450);
        
        // Act
        var result = MMR.AbsoluteDifference(mmr1, mmr2);
        
        // Assert
        result.Should().Be(50);
    }
    
    [Fact]
    [Trait("Category", "MMR 값 객체")]
    public void 순서가_바뀌어도_절대_차이는_동일해야_한다()
    {
        // Arrange
        var mmr1 = new MMR(1500);
        var mmr2 = new MMR(1450);
        
        // Act
        var result1 = MMR.AbsoluteDifference(mmr1, mmr2);
        var result2 = MMR.AbsoluteDifference(mmr2, mmr1);
        
        // Assert
        result1.Should().Be(50);
        result2.Should().Be(50);
    }
}

