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
public class EndMatchUseCaseTests
{
    private readonly Mock<IMatchSessionRepository> _repositoryMock;
    private readonly EndMatchUseCase _useCase;
    
    public EndMatchUseCaseTests()
    {
        _repositoryMock = new Mock<IMatchSessionRepository>();
        _useCase = new EndMatchUseCase(_repositoryMock.Object);
    }
    
    [Fact]
    [Trait("Category", "매치 종료 유스케이스")]
    public async Task 유효한_요청이면_매치를_종료해야_한다()
    {
        // Arrange
        var matchId = Guid.NewGuid();
        var player1Id = Guid.NewGuid();
        var player2Id = Guid.NewGuid();
        
        var session = new DomainMatchSession(matchId, new List<Guid> { player1Id, player2Id }, MatchmakingMode.Solo);
        session.Start(); // InProgress 상태로 전이
        
        var request = new EndMatchRequest
        {
            MatchId = matchId,
            Results = new List<PlayerResultDto>
            {
                new PlayerResultDto { PlayerId = player1Id, IsWinner = true, Score = 100 },
                new PlayerResultDto { PlayerId = player2Id, IsWinner = false, Score = 50 }
            },
            WinnerId = player1Id
        };
        
        _repositoryMock
            .Setup(r => r.FindByMatchIdAsync(matchId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(session);
        _repositoryMock
            .Setup(r => r.UpdateAsync(It.IsAny<DomainMatchSession>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);
        
        // Act
        var result = await _useCase.ExecuteAsync(request);
        
        // Assert
        result.Should().NotBeNull();
        result.SessionId.Should().Be(session.SessionId);
        result.FinalStatus.Should().Be(MatchStatus.Finished);
        session.Result.Should().NotBeNull();
        session.Result!.MatchId.Should().Be(matchId);
        
        _repositoryMock.Verify(r => r.FindByMatchIdAsync(matchId, It.IsAny<CancellationToken>()), Times.Once);
        _repositoryMock.Verify(r => r.UpdateAsync(It.IsAny<DomainMatchSession>(), It.IsAny<CancellationToken>()), Times.Once);
    }
    
    [Fact]
    [Trait("Category", "매치 종료 유스케이스")]
    public async Task 세션을_찾을_수_없으면_MatchSessionNotFoundException을_발생시켜야_한다()
    {
        // Arrange
        var matchId = Guid.NewGuid();
        var request = new EndMatchRequest
        {
            MatchId = matchId,
            Results = new List<PlayerResultDto>
            {
                new PlayerResultDto { PlayerId = Guid.NewGuid(), IsWinner = true }
            }
        };
        
        _repositoryMock
            .Setup(r => r.FindByMatchIdAsync(matchId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((DomainMatchSession?)null);
        
        // Act
        var act = async () => await _useCase.ExecuteAsync(request);
        
        // Assert
        var exception = await act.Should().ThrowAsync<MatchSessionNotFoundException>();
        exception.Which.MatchId.Should().Be(matchId);
    }
    
    [Fact]
    [Trait("Category", "매치 종료 유스케이스")]
    public async Task 세션이_InProgress_상태가_아니면_InvalidMatchSessionStateException을_발생시켜야_한다()
    {
        // Arrange
        var matchId = Guid.NewGuid();
        var session = new DomainMatchSession(matchId, new List<Guid> { Guid.NewGuid() }, MatchmakingMode.Solo);
        // Matched 상태 (아직 시작하지 않음)
        
        var request = new EndMatchRequest
        {
            MatchId = matchId,
            Results = new List<PlayerResultDto>
            {
                new PlayerResultDto { PlayerId = Guid.NewGuid(), IsWinner = true }
            }
        };
        
        _repositoryMock
            .Setup(r => r.FindByMatchIdAsync(matchId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(session);
        
        // Act
        var act = async () => await _useCase.ExecuteAsync(request);
        
        // Assert
        await act.Should().ThrowAsync<InvalidMatchSessionStateException>();
    }
}

