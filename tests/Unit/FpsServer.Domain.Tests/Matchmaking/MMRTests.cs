using FluentAssertions;
using FpsServer.Domain.Matchmaking;
using Xunit;

namespace FpsServer.Domain.Tests.Matchmaking;

public class MMRTests
{
    [Fact]
    public void Constructor_WithValidValue_ShouldCreateMMR()
    {
        // Arrange & Act
        var mmr = new MMR(1500);
        
        // Assert
        mmr.Value.Should().Be(1500);
    }
    
    [Fact]
    public void Constructor_WithNegativeValue_ShouldThrowArgumentException()
    {
        // Arrange & Act
        var act = () => new MMR(-1);
        
        // Assert
        act.Should().Throw<ArgumentException>()
            .WithMessage("MMR cannot be negative*")
            .And.ParamName.Should().Be("value");
    }
    
    [Fact]
    public void Constructor_WithZero_ShouldCreateMMR()
    {
        // Arrange & Act
        var mmr = new MMR(0);
        
        // Assert
        mmr.Value.Should().Be(0);
    }
    
    [Fact]
    public void OperatorPlus_ShouldAddDelta()
    {
        // Arrange
        var mmr = new MMR(1500);
        
        // Act
        var result = mmr + 50;
        
        // Assert
        result.Value.Should().Be(1550);
    }
    
    [Fact]
    public void OperatorMinus_ShouldSubtractDelta()
    {
        // Arrange
        var mmr = new MMR(1500);
        
        // Act
        var result = mmr - 50;
        
        // Assert
        result.Value.Should().Be(1450);
    }
    
    [Fact]
    public void OperatorMinus_TwoMMRs_ShouldReturnDifference()
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
    public void AbsoluteDifference_ShouldReturnAbsoluteValue()
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
    public void AbsoluteDifference_WithReversedOrder_ShouldReturnSameValue()
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

