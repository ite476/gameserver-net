using FluentAssertions;
using FpsServer.Application.Matchmaking.DTOs;
using FpsServer.Application.Matchmaking.Ports;
using FpsServer.Application.Matchmaking.UseCases;
using FpsServer.Domain.Matchmaking;
using DomainMatch = FpsServer.Domain.Matchmaking.Match;
using DomainMMR = FpsServer.Domain.Matchmaking.MMR;
using FpsServer.Domain.Matchmaking.Exceptions;
using Moq;
using Xunit;

namespace FpsServer.Application.Tests.Matchmaking;

[Trait("Feature", "매치메이킹")]
public class JoinMatchmakingQueueUseCaseTests
{
    private readonly Mock<IMatchmakingRepository> _repositoryMock;
    private readonly Mock<IMatchmakingNotifier> _notifierMock;
    private readonly MatchmakingDomainService _domainService;
    private readonly JoinMatchmakingQueueUseCase _useCase;
    
    public JoinMatchmakingQueueUseCaseTests()
    {
        _repositoryMock = new Mock<IMatchmakingRepository>();
        _notifierMock = new Mock<IMatchmakingNotifier>();
        _domainService = new MatchmakingDomainService();
        _useCase = new JoinMatchmakingQueueUseCase(
            _repositoryMock.Object,
            _notifierMock.Object,
            _domainService
        );
    }
    
    [Fact]
    [Trait("Category", "큐 진입 유스케이스")]
    public async Task 유효한_요청이면_큐에_진입해야_한다()
    {
        // Arrange
        var request = new JoinMatchmakingRequest
        {
            PlayerId = Guid.NewGuid(),
            GameMode = MatchmakingMode.Solo,
            MMR = 1500
        };
        
        var queue = new MatchmakingQueue(MatchmakingMode.Solo);
        _repositoryMock
            .Setup(r => r.GetOrCreateQueueAsync(It.IsAny<MatchmakingMode>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(queue);
        _repositoryMock
            .Setup(r => r.EnqueueAsync(It.IsAny<MatchmakingQueue>(), It.IsAny<PlayerMatchRequest>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);
        
        // Act
        var result = await _useCase.ExecuteAsync(request);
        
        // Assert
        result.Should().NotBeNull();
        result.RequestId.Should().NotBeEmpty();
        result.Status.Should().Be("Enqueued");
        _repositoryMock.Verify(r => r.GetOrCreateQueueAsync(MatchmakingMode.Solo, It.IsAny<CancellationToken>()), Times.Once);
        _repositoryMock.Verify(r => r.EnqueueAsync(It.IsAny<MatchmakingQueue>(), It.IsAny<PlayerMatchRequest>(), It.IsAny<CancellationToken>()), Times.Once);
        _notifierMock.Verify(n => n.NotifyMatchFoundAsync(It.IsAny<DomainMatch>(), It.IsAny<CancellationToken>()), Times.Never);
    }
    
    [Fact]
    [Trait("Category", "큐 진입 유스케이스")]
    public async Task 매칭_성공하면_알림을_전송해야_한다()
    {
        // Arrange
        var player1Id = Guid.NewGuid();
        var player2Id = Guid.NewGuid();
        
        var request1 = new JoinMatchmakingRequest
        {
            PlayerId = player1Id,
            GameMode = MatchmakingMode.Solo,
            MMR = 1500
        };
        
        var request2 = new JoinMatchmakingRequest
        {
            PlayerId = player2Id,
            GameMode = MatchmakingMode.Solo,
            MMR = 1550 // MMR 차이 50 (허용 범위 내)
        };
        
        var queue = new MatchmakingQueue(MatchmakingMode.Solo);
        
        // 첫 번째 플레이어 큐 진입
        var player1Request = new PlayerMatchRequest(player1Id, MatchmakingMode.Solo, new DomainMMR(1500));
        queue.Enqueue(player1Request);
        
        // 두 번째 플레이어 큐 진입 (매칭 성공)
        _repositoryMock
            .Setup(r => r.GetOrCreateQueueAsync(It.IsAny<MatchmakingMode>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(queue);
        _repositoryMock
            .Setup(r => r.EnqueueAsync(It.IsAny<MatchmakingQueue>(), It.IsAny<PlayerMatchRequest>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);
        _repositoryMock
            .Setup(r => r.DequeueAsync(It.IsAny<MatchmakingQueue>(), It.IsAny<IEnumerable<Guid>>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);
        
        // Act
        var result = await _useCase.ExecuteAsync(request2);
        
        // Assert
        result.Should().NotBeNull();
        result.Status.Should().Be("Matched");
        _repositoryMock.Verify(r => r.DequeueAsync(It.IsAny<MatchmakingQueue>(), It.IsAny<IEnumerable<Guid>>(), It.IsAny<CancellationToken>()), Times.Once);
        _notifierMock.Verify(n => n.NotifyMatchFoundAsync(It.IsAny<DomainMatch>(), It.IsAny<CancellationToken>()), Times.Once);
    }
    
    [Fact]
    [Trait("Category", "큐 진입 유스케이스")]
    public async Task 이미_큐에_있는_플레이어면_예외를_발생시켜야_한다()
    {
        // Arrange
        var playerId = Guid.NewGuid();
        var request = new JoinMatchmakingRequest
        {
            PlayerId = playerId,
            GameMode = MatchmakingMode.Solo,
            MMR = 1500
        };
        
        var queue = new MatchmakingQueue(MatchmakingMode.Solo);
        var existingRequest = new PlayerMatchRequest(playerId, MatchmakingMode.Solo, new DomainMMR(1500));
        queue.Enqueue(existingRequest);
        
        _repositoryMock
            .Setup(r => r.GetOrCreateQueueAsync(It.IsAny<MatchmakingMode>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(queue);
        
        // Act & Assert
        var act = async () => await _useCase.ExecuteAsync(request);
        await act.Should().ThrowAsync<PlayerAlreadyInQueueException>();
    }
    
    [Fact]
    [Trait("Category", "큐 진입 유스케이스")]
    public async Task 게임_모드가_일치하지_않으면_예외를_발생시켜야_한다()
    {
        // Arrange
        var request = new JoinMatchmakingRequest
        {
            PlayerId = Guid.NewGuid(),
            GameMode = MatchmakingMode.Solo,
            MMR = 1500
        };
        
        var queue = new MatchmakingQueue(MatchmakingMode.Duo); // 다른 게임 모드
        _repositoryMock
            .Setup(r => r.GetOrCreateQueueAsync(It.IsAny<MatchmakingMode>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(queue);
        
        // Act & Assert
        var act = async () => await _useCase.ExecuteAsync(request);
        await act.Should().ThrowAsync<InvalidMatchmakingRequestException>();
    }
}

