using FluentAssertions;
using FpsServer.Application.MMR.DTOs;
using FpsServer.Application.MMR.Ports;
using FpsServer.Application.MMR.UseCases;
using FpsServer.Domain.MMR;
using DomainMMR = FpsServer.Domain.Matchmaking.MMR;
using Moq;
using Xunit;

namespace FpsServer.Application.Tests.MMR;

[Trait("Feature", "MMR 업데이트")]
public class UpdatePlayerMMRUseCaseTests
{
    private readonly Mock<IPlayerMMRRepository> _repositoryMock;
    private readonly Mock<IMMRCalculator> _calculatorMock;
    private readonly UpdatePlayerMMRUseCase _useCase;

    public UpdatePlayerMMRUseCaseTests()
    {
        _repositoryMock = new Mock<IPlayerMMRRepository>();
        _calculatorMock = new Mock<IMMRCalculator>();
        _useCase = new UpdatePlayerMMRUseCase(_repositoryMock.Object, _calculatorMock.Object);
    }

    [Fact]
    [Trait("Category", "MMR 업데이트 유스케이스")]
    public async Task 기존_플레이어_MMR이_있으면_업데이트해야_한다()
    {
        // Arrange
        var playerId = Guid.NewGuid();
        var matchId = Guid.NewGuid();
        var oldMMR = new DomainMMR(1500);
        var newMMR = new DomainMMR(1516);
        
        var existingPlayerMMR = new PlayerMMR(playerId, oldMMR);
        
        var request = new UpdateMMRRequest
        {
            PlayerId = playerId,
            MatchId = matchId,
            IsWinner = true,
            OpponentAverageMMR = 1500,
            KFactor = 32
        };

        _repositoryMock
            .Setup(r => r.FindByPlayerIdAsync(playerId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(existingPlayerMMR);
        
        _calculatorMock
            .Setup(c => c.CalculateNewMMR(
                It.IsAny<DomainMMR>(),
                It.IsAny<DomainMMR>(),
                It.IsAny<double>(),
                It.IsAny<int>()))
            .Returns(newMMR);
        
        _repositoryMock
            .Setup(r => r.SaveAsync(It.IsAny<PlayerMMR>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _useCase.ExecuteAsync(request);

        // Assert
        result.Should().NotBeNull();
        result.PlayerId.Should().Be(playerId);
        result.OldMMR.Should().Be(1500);
        result.NewMMR.Should().Be(1516);

        _repositoryMock.Verify(r => r.FindByPlayerIdAsync(playerId, It.IsAny<CancellationToken>()), Times.Once);
        _calculatorMock.Verify(c => c.CalculateNewMMR(
            oldMMR,
            It.Is<DomainMMR>(m => m.Value == 1500),
            1.0,
            32), Times.Once);
        _repositoryMock.Verify(r => r.SaveAsync(It.IsAny<PlayerMMR>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    [Trait("Category", "MMR 업데이트 유스케이스")]
    public async Task 플레이어_MMR이_없으면_초기_MMR로_생성해야_한다()
    {
        // Arrange
        var playerId = Guid.NewGuid();
        var matchId = Guid.NewGuid();
        var newMMR = new DomainMMR(1516);
        
        var request = new UpdateMMRRequest
        {
            PlayerId = playerId,
            MatchId = matchId,
            IsWinner = true,
            OpponentAverageMMR = 1500,
            KFactor = 32
        };

        _repositoryMock
            .Setup(r => r.FindByPlayerIdAsync(playerId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((PlayerMMR?)null);
        
        _calculatorMock
            .Setup(c => c.CalculateNewMMR(
                It.Is<DomainMMR>(m => m.Value == 1500), // 초기 MMR: 1500
                It.IsAny<DomainMMR>(),
                It.IsAny<double>(),
                It.IsAny<int>()))
            .Returns(newMMR);
        
        _repositoryMock
            .Setup(r => r.SaveAsync(It.IsAny<PlayerMMR>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _useCase.ExecuteAsync(request);

        // Assert
        result.Should().NotBeNull();
        result.PlayerId.Should().Be(playerId);
        result.OldMMR.Should().Be(1500); // 초기 MMR
        result.NewMMR.Should().Be(1516);

        _repositoryMock.Verify(r => r.FindByPlayerIdAsync(playerId, It.IsAny<CancellationToken>()), Times.Once);
        _calculatorMock.Verify(c => c.CalculateNewMMR(
            It.Is<DomainMMR>(m => m.Value == 1500), // 초기 MMR
            It.Is<DomainMMR>(m => m.Value == 1500),
            1.0,
            32), Times.Once);
        _repositoryMock.Verify(r => r.SaveAsync(It.IsAny<PlayerMMR>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    [Trait("Category", "MMR 업데이트 유스케이스")]
    public async Task 패배_시_실제_점수가_0_0으로_전달되어야_한다()
    {
        // Arrange
        var playerId = Guid.NewGuid();
        var matchId = Guid.NewGuid();
        var oldMMR = new DomainMMR(1500);
        var newMMR = new DomainMMR(1484);
        
        var existingPlayerMMR = new PlayerMMR(playerId, oldMMR);
        
        var request = new UpdateMMRRequest
        {
            PlayerId = playerId,
            MatchId = matchId,
            IsWinner = false, // 패배
            OpponentAverageMMR = 1500,
            KFactor = 32
        };

        _repositoryMock
            .Setup(r => r.FindByPlayerIdAsync(playerId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(existingPlayerMMR);
        
        _calculatorMock
            .Setup(c => c.CalculateNewMMR(
                It.IsAny<DomainMMR>(),
                It.IsAny<DomainMMR>(),
                It.IsAny<double>(),
                It.IsAny<int>()))
            .Returns(newMMR);
        
        _repositoryMock
            .Setup(r => r.SaveAsync(It.IsAny<PlayerMMR>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _useCase.ExecuteAsync(request);

        // Assert
        result.Should().NotBeNull();
        result.NewMMR.Should().BeLessThan(result.OldMMR);

        _calculatorMock.Verify(c => c.CalculateNewMMR(
            It.IsAny<DomainMMR>(),
            It.IsAny<DomainMMR>(),
            0.0, // 패배 시 실제 점수: 0.0
            It.IsAny<int>()), Times.Once);
    }

    [Fact]
    [Trait("Category", "MMR 업데이트 유스케이스")]
    public async Task 커스텀_K_값이_전달되어야_한다()
    {
        // Arrange
        var playerId = Guid.NewGuid();
        var matchId = Guid.NewGuid();
        var oldMMR = new DomainMMR(1500);
        var newMMR = new DomainMMR(1520);
        
        var existingPlayerMMR = new PlayerMMR(playerId, oldMMR);
        
        var request = new UpdateMMRRequest
        {
            PlayerId = playerId,
            MatchId = matchId,
            IsWinner = true,
            OpponentAverageMMR = 1500,
            KFactor = 64 // 커스텀 K 값
        };

        _repositoryMock
            .Setup(r => r.FindByPlayerIdAsync(playerId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(existingPlayerMMR);
        
        _calculatorMock
            .Setup(c => c.CalculateNewMMR(
                It.IsAny<DomainMMR>(),
                It.IsAny<DomainMMR>(),
                It.IsAny<double>(),
                It.IsAny<int>()))
            .Returns(newMMR);
        
        _repositoryMock
            .Setup(r => r.SaveAsync(It.IsAny<PlayerMMR>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _useCase.ExecuteAsync(request);

        // Assert
        _calculatorMock.Verify(c => c.CalculateNewMMR(
            It.IsAny<DomainMMR>(),
            It.IsAny<DomainMMR>(),
            It.IsAny<double>(),
            64), Times.Once); // 커스텀 K 값 전달 확인
    }
}

