using FluentAssertions;
using FpsServer.Application.Matchmaking.Ports;
using FpsServer.Application.Matchmaking.UseCases;
using FpsServer.Domain.Matchmaking;
using FpsServer.Domain.Matchmaking.Exceptions;
using DomainMMR = FpsServer.Domain.Matchmaking.MMR;
using Moq;
using Xunit;

namespace FpsServer.Application.Tests.Matchmaking;

[Trait("Feature", "매치메이킹")]
public class CancelMatchmakingUseCaseTests
{
    private readonly Mock<IMatchmakingRepository> _repositoryMock;
    private readonly CancelMatchmakingUseCase _useCase;
    
    public CancelMatchmakingUseCaseTests()
    {
        _repositoryMock = new Mock<IMatchmakingRepository>();
        _useCase = new CancelMatchmakingUseCase(_repositoryMock.Object);
    }
    
    [Fact]
    [Trait("Category", "큐 취소 유스케이스")]
    public async Task 큐에_있는_플레이어면_취소해야_한다()
    {
        // Arrange
        var playerId = Guid.NewGuid();
        var queue = new MatchmakingQueue(MatchmakingMode.Solo);
        var request = new PlayerMatchRequest(playerId, MatchmakingMode.Solo, new DomainMMR(1500));
        queue.Enqueue(request);
        
        _repositoryMock
            .Setup(r => r.GetOrCreateQueueAsync(It.IsAny<MatchmakingMode>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(queue);
        _repositoryMock
            .Setup(r => r.CancelAsync(It.IsAny<MatchmakingQueue>(), It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);
        
        // Act
        await _useCase.ExecuteAsync(MatchmakingMode.Solo, playerId);
        
        // Assert
        _repositoryMock.Verify(r => r.GetOrCreateQueueAsync(MatchmakingMode.Solo, It.IsAny<CancellationToken>()), Times.Once);
        _repositoryMock.Verify(r => r.CancelAsync(It.IsAny<MatchmakingQueue>(), playerId, It.IsAny<CancellationToken>()), Times.Once);
    }
    
    [Fact]
    [Trait("Category", "큐 취소 유스케이스")]
    public async Task 큐에_없는_플레이어면_예외를_발생시켜야_한다()
    {
        // Arrange
        var playerId = Guid.NewGuid();
        var queue = new MatchmakingQueue(MatchmakingMode.Solo);
        // 큐에 플레이어 추가 안 함
        
        _repositoryMock
            .Setup(r => r.GetOrCreateQueueAsync(It.IsAny<MatchmakingMode>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(queue);
        
        // Act & Assert
        var act = async () => await _useCase.ExecuteAsync(MatchmakingMode.Solo, playerId);
        await act.Should().ThrowAsync<PlayerNotInQueueException>();
    }
}

