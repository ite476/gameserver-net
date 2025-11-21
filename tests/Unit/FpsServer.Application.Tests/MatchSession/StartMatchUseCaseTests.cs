using FluentAssertions;
using FpsServer.Application.MatchSession.DTOs;
using FpsServer.Application.MatchSession.Ports;
using FpsServer.Application.MatchSession.UseCases;
using FpsServer.Domain.Matchmaking;
using DomainMatchSession = FpsServer.Domain.MatchSession.MatchSession;
using FpsServer.Domain.MatchSession;
using FpsServer.Domain.MatchSession.Exceptions;
using Moq;
using Xunit;

namespace FpsServer.Application.Tests.MatchSession;

[Trait("Feature", "매치 세션")]
public class StartMatchUseCaseTests
{
    private readonly Mock<IMatchSessionRepository> _repositoryMock;
    private readonly StartMatchUseCase _useCase;
    
    public StartMatchUseCaseTests()
    {
        _repositoryMock = new Mock<IMatchSessionRepository>();
        _useCase = new StartMatchUseCase(_repositoryMock.Object);
    }
    
    [Fact]
    [Trait("Category", "매치 시작 유스케이스")]
    public async Task 기존_세션이_없으면_새_세션을_생성하고_시작해야_한다()
    {
        // Arrange
        var matchId = Guid.NewGuid();
        var playerIds = new List<Guid> { Guid.NewGuid(), Guid.NewGuid() };
        var request = new StartMatchRequest
        {
            MatchId = matchId,
            PlayerIds = playerIds
        };
        
        _repositoryMock
            .Setup(r => r.FindByMatchIdAsync(matchId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((DomainMatchSession?)null);
        _repositoryMock
            .Setup(r => r.CreateAsync(It.IsAny<DomainMatchSession>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);
        _repositoryMock
            .Setup(r => r.UpdateAsync(It.IsAny<DomainMatchSession>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);
        
        // Act
        var result = await _useCase.ExecuteAsync(request, MatchmakingMode.Solo);
        
        // Assert
        result.Should().NotBeNull();
        result.SessionId.Should().NotBeEmpty();
        result.Status.Should().Be(MatchStatus.InProgress);
        
        _repositoryMock.Verify(r => r.FindByMatchIdAsync(matchId, It.IsAny<CancellationToken>()), Times.Once);
        _repositoryMock.Verify(r => r.CreateAsync(It.IsAny<DomainMatchSession>(), It.IsAny<CancellationToken>()), Times.Once);
        _repositoryMock.Verify(r => r.UpdateAsync(It.IsAny<DomainMatchSession>(), It.IsAny<CancellationToken>()), Times.Once);
    }
    
    [Fact]
    [Trait("Category", "매치 시작 유스케이스")]
    public async Task 기존_세션이_있으면_재사용하고_시작해야_한다()
    {
        // Arrange
        var matchId = Guid.NewGuid();
        var playerIds = new List<Guid> { Guid.NewGuid(), Guid.NewGuid() };
        var request = new StartMatchRequest
        {
            MatchId = matchId,
            PlayerIds = playerIds
        };
        
        var existingSession = new DomainMatchSession(matchId, playerIds, MatchmakingMode.Solo);
        _repositoryMock
            .Setup(r => r.FindByMatchIdAsync(matchId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(existingSession);
        _repositoryMock
            .Setup(r => r.UpdateAsync(It.IsAny<DomainMatchSession>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);
        
        // Act
        var result = await _useCase.ExecuteAsync(request, MatchmakingMode.Solo);
        
        // Assert
        result.Should().NotBeNull();
        result.SessionId.Should().Be(existingSession.SessionId);
        result.Status.Should().Be(MatchStatus.InProgress);
        
        _repositoryMock.Verify(r => r.FindByMatchIdAsync(matchId, It.IsAny<CancellationToken>()), Times.Once);
        _repositoryMock.Verify(r => r.CreateAsync(It.IsAny<DomainMatchSession>(), It.IsAny<CancellationToken>()), Times.Never);
        _repositoryMock.Verify(r => r.UpdateAsync(It.IsAny<DomainMatchSession>(), It.IsAny<CancellationToken>()), Times.Once);
    }
    
    [Fact]
    [Trait("Category", "매치 시작 유스케이스")]
    public async Task 기존_세션이_이미_시작된_상태이면_InvalidMatchSessionStateException을_발생시켜야_한다()
    {
        // Arrange
        var matchId = Guid.NewGuid();
        var playerIds = new List<Guid> { Guid.NewGuid(), Guid.NewGuid() };
        var request = new StartMatchRequest
        {
            MatchId = matchId,
            PlayerIds = playerIds
        };
        
        var existingSession = new DomainMatchSession(matchId, playerIds, MatchmakingMode.Solo);
        existingSession.Start(); // 이미 시작된 상태
        
        _repositoryMock
            .Setup(r => r.FindByMatchIdAsync(matchId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(existingSession);
        
        // Act
        var act = async () => await _useCase.ExecuteAsync(request, MatchmakingMode.Solo);
        
        // Assert
        await act.Should().ThrowAsync<InvalidMatchSessionStateException>();
    }
}

