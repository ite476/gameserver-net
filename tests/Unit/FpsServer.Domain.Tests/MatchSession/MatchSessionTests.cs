using FluentAssertions;
using FpsServer.Domain.Matchmaking;
using DomainMatchSession = FpsServer.Domain.MatchSession.MatchSession;
using FpsServer.Domain.MatchSession;
using FpsServer.Domain.MatchSession.Exceptions;
using Xunit;

namespace FpsServer.Domain.Tests.MatchSession;

[Trait("Feature", "매치 세션")]
public class MatchSessionTests
{
    [Fact]
    [Trait("Category", "매치 세션 생성")]
    public void 유효한_매개변수가_주어지면_세션을_생성해야_한다()
    {
        // Arrange
        var matchId = Guid.NewGuid();
        var playerIds = new List<Guid> { Guid.NewGuid(), Guid.NewGuid() };
        var gameMode = MatchmakingMode.Solo;
        
        // Act
        var session = new DomainMatchSession(matchId, playerIds, gameMode);
        
        // Assert
        session.MatchId.Should().Be(matchId);
        session.PlayerIds.Should().BeEquivalentTo(playerIds);
        session.GameMode.Should().Be(gameMode);
        session.Status.Should().Be(MatchStatus.Matched);
        session.SessionId.Should().NotBeEmpty();
        session.StartedAt.Should().BeNull();
        session.EndedAt.Should().BeNull();
    }
    
    [Fact]
    [Trait("Category", "매치 세션 생성")]
    public void 빈_매치_ID가_주어지면_ArgumentException을_발생시켜야_한다()
    {
        // Arrange
        var playerIds = new List<Guid> { Guid.NewGuid() };
        
        // Act
        var act = () => new DomainMatchSession(Guid.Empty, playerIds, MatchmakingMode.Solo);
        
        // Assert
        act.Should().Throw<ArgumentException>()
            .And.ParamName.Should().Be("matchId");
    }
    
    [Fact]
    [Trait("Category", "매치 세션 생성")]
    public void 빈_플레이어_목록이_주어지면_ArgumentException을_발생시켜야_한다()
    {
        // Arrange
        var matchId = Guid.NewGuid();
        
        // Act
        var act = () => new DomainMatchSession(matchId, new List<Guid>(), MatchmakingMode.Solo);
        
        // Assert
        act.Should().Throw<ArgumentException>()
            .And.ParamName.Should().Be("playerIds");
    }
    
    [Fact]
    [Trait("Category", "상태 전이")]
    public void Matched_상태에서_Start를_호출하면_InProgress로_전이해야_한다()
    {
        // Arrange
        var session = CreateSession();
        
        // Act
        session.Start();
        
        // Assert
        session.Status.Should().Be(MatchStatus.InProgress);
        session.StartedAt.Should().NotBeNull();
    }
    
    [Fact]
    [Trait("Category", "상태 전이")]
    public void InProgress_상태가_아니면_Start를_호출할_수_없어야_한다()
    {
        // Arrange
        var session = CreateSession();
        session.Start();
        session.End(CreateMatchResult(session.MatchId));
        
        // Act
        var act = () => session.Start();
        
        // Assert
        act.Should().Throw<InvalidMatchSessionStateException>()
            .Which.CurrentStatus.Should().Be(MatchStatus.Finished);
    }
    
    [Fact]
    [Trait("Category", "상태 전이")]
    public void InProgress_상태에서_End를_호출하면_Finished로_전이해야_한다()
    {
        // Arrange
        var session = CreateSession();
        session.Start();
        var result = CreateMatchResult(session.MatchId);
        
        // Act
        session.End(result);
        
        // Assert
        session.Status.Should().Be(MatchStatus.Finished);
        session.EndedAt.Should().NotBeNull();
        session.Result.Should().Be(result);
    }
    
    [Fact]
    [Trait("Category", "상태 전이")]
    public void InProgress_상태가_아니면_End를_호출할_수_없어야_한다()
    {
        // Arrange
        var session = CreateSession();
        var result = CreateMatchResult(session.MatchId);
        
        // Act
        var act = () => session.End(result);
        
        // Assert
        act.Should().Throw<InvalidMatchSessionStateException>()
            .Which.CurrentStatus.Should().Be(MatchStatus.Matched);
    }
    
    [Fact]
    [Trait("Category", "상태 전이")]
    public void End_호출_시_매치_ID가_일치하지_않으면_ArgumentException을_발생시켜야_한다()
    {
        // Arrange
        var session = CreateSession();
        session.Start();
        var result = CreateMatchResult(Guid.NewGuid()); // 다른 매치 ID
        
        // Act
        var act = () => session.End(result);
        
        // Assert
        act.Should().Throw<ArgumentException>()
            .WithMessage("*MatchId mismatch*");
    }
    
    [Fact]
    [Trait("Category", "상태 전이")]
    public void Matched_또는_InProgress_상태에서_Cancel을_호출하면_Cancelled로_전이해야_한다()
    {
        // Arrange
        var session = CreateSession();
        
        // Act
        session.Cancel();
        
        // Assert
        session.Status.Should().Be(MatchStatus.Cancelled);
        session.EndedAt.Should().NotBeNull();
    }
    
    [Fact]
    [Trait("Category", "상태 전이")]
    public void Finished_상태에서는_Cancel을_호출할_수_없어야_한다()
    {
        // Arrange
        var session = CreateSession();
        session.Start();
        session.End(CreateMatchResult(session.MatchId));
        
        // Act
        var act = () => session.Cancel();
        
        // Assert
        act.Should().Throw<InvalidMatchSessionStateException>()
            .Which.CurrentStatus.Should().Be(MatchStatus.Finished);
    }
    
    private static DomainMatchSession CreateSession()
    {
        return new DomainMatchSession(
            Guid.NewGuid(),
            new List<Guid> { Guid.NewGuid(), Guid.NewGuid() },
            MatchmakingMode.Solo
        );
    }
    
    private static MatchResult CreateMatchResult(Guid matchId)
    {
        var playerResults = new List<PlayerResult>
        {
            new PlayerResult(Guid.NewGuid(), true, 100),
            new PlayerResult(Guid.NewGuid(), false, 50)
        };
        
        return new MatchResult(matchId, playerResults);
    }
}

