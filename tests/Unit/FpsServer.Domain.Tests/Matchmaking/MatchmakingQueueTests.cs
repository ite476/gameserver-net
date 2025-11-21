using FluentAssertions;
using FpsServer.Domain.Matchmaking;
using FpsServer.Domain.Matchmaking.Exceptions;
using DomainMMR = FpsServer.Domain.Matchmaking.MMR;
using Xunit;

namespace FpsServer.Domain.Tests.Matchmaking;

[Trait("Feature", "매치메이킹")]
public class MatchmakingQueueTests
{
    [Fact]
    [Trait("Category", "매치메이킹 큐")]
    public void 게임_모드가_주어지면_큐를_생성해야_한다()
    {
        // Arrange & Act
        var queue = new MatchmakingQueue(MatchmakingMode.Solo);
        
        // Assert
        queue.GameMode.Should().Be(MatchmakingMode.Solo);
        queue.QueueId.Should().NotBeEmpty();
        queue.Requests.Should().BeEmpty();
    }
    
    [Fact]
    [Trait("Category", "매치메이킹 큐")]
    public void 유효한_요청이면_큐에_추가해야_한다()
    {
        // Arrange
        var queue = new MatchmakingQueue(MatchmakingMode.Solo);
        var playerId = Guid.NewGuid();
        var request = new PlayerMatchRequest(playerId, MatchmakingMode.Solo, new DomainMMR(1500));
        
        // Act
        queue.Enqueue(request);
        
        // Assert
        queue.Requests.Should().HaveCount(1);
        queue.Requests[0].PlayerId.Should().Be(playerId);
    }
    
    [Fact]
    [Trait("Category", "매치메이킹 큐")]
    public void null_요청이면_ArgumentNullException을_발생시켜야_한다()
    {
        // Arrange
        var queue = new MatchmakingQueue(MatchmakingMode.Solo);
        
        // Act
        var act = () => queue.Enqueue(null!);
        
        // Assert
        act.Should().Throw<ArgumentNullException>()
            .And.ParamName.Should().Be("request");
    }
    
    [Fact]
    [Trait("Category", "매치메이킹 큐")]
    public void 중복_플레이어가_있으면_PlayerAlreadyInQueueException을_발생시켜야_한다()
    {
        // Arrange
        var queue = new MatchmakingQueue(MatchmakingMode.Solo);
        var playerId = Guid.NewGuid();
        var request1 = new PlayerMatchRequest(playerId, MatchmakingMode.Solo, new DomainMMR(1500));
        var request2 = new PlayerMatchRequest(playerId, MatchmakingMode.Solo, new DomainMMR(1600));
        
        queue.Enqueue(request1);
        
        // Act
        var act = () => queue.Enqueue(request2);
        
        // Assert
        act.Should().Throw<PlayerAlreadyInQueueException>()
            .Which.PlayerId.Should().Be(playerId);
    }
    
    [Fact]
    [Trait("Category", "매치메이킹 큐")]
    public void 게임_모드가_불일치하면_InvalidMatchmakingRequestException을_발생시켜야_한다()
    {
        // Arrange
        var queue = new MatchmakingQueue(MatchmakingMode.Solo);
        var request = new PlayerMatchRequest(Guid.NewGuid(), MatchmakingMode.Duo, new DomainMMR(1500));
        
        // Act
        var act = () => queue.Enqueue(request);
        
        // Assert
        act.Should().Throw<InvalidMatchmakingRequestException>()
            .WithMessage("*Game mode mismatch*");
    }
    
    [Fact]
    [Trait("Category", "매치메이킹 큐")]
    public void 존재하는_플레이어를_취소하면_큐에서_제거해야_한다()
    {
        // Arrange
        var queue = new MatchmakingQueue(MatchmakingMode.Solo);
        var playerId = Guid.NewGuid();
        var request = new PlayerMatchRequest(playerId, MatchmakingMode.Solo, new DomainMMR(1500));
        queue.Enqueue(request);
        
        // Act
        queue.Cancel(playerId);
        
        // Assert
        queue.Requests.Should().BeEmpty();
    }
    
    [Fact]
    [Trait("Category", "매치메이킹 큐")]
    public void 존재하지_않는_플레이어를_취소하면_PlayerNotInQueueException을_발생시켜야_한다()
    {
        // Arrange
        var queue = new MatchmakingQueue(MatchmakingMode.Solo);
        var playerId = Guid.NewGuid();
        
        // Act
        var act = () => queue.Cancel(playerId);
        
        // Assert
        act.Should().Throw<PlayerNotInQueueException>()
            .Which.PlayerId.Should().Be(playerId);
    }
    
    [Fact]
    [Trait("Category", "매치메이킹 큐")]
    public void 존재하는_플레이어_ID로_찾으면_요청을_반환해야_한다()
    {
        // Arrange
        var queue = new MatchmakingQueue(MatchmakingMode.Solo);
        var playerId = Guid.NewGuid();
        var request = new PlayerMatchRequest(playerId, MatchmakingMode.Solo, new DomainMMR(1500));
        queue.Enqueue(request);
        
        // Act
        var result = queue.FindByPlayerId(playerId);
        
        // Assert
        result.Should().NotBeNull();
        result!.PlayerId.Should().Be(playerId);
    }
    
    [Fact]
    [Trait("Category", "매치메이킹 큐")]
    public void 존재하지_않는_플레이어_ID로_찾으면_null을_반환해야_한다()
    {
        // Arrange
        var queue = new MatchmakingQueue(MatchmakingMode.Solo);
        var playerId = Guid.NewGuid();
        
        // Act
        var result = queue.FindByPlayerId(playerId);
        
        // Assert
        result.Should().BeNull();
    }
    
    [Fact]
    [Trait("Category", "매치메이킹 큐")]
    public void 여러_플레이어를_추가하면_순서를_유지해야_한다()
    {
        // Arrange
        var queue = new MatchmakingQueue(MatchmakingMode.Solo);
        var player1 = Guid.NewGuid();
        var player2 = Guid.NewGuid();
        var player3 = Guid.NewGuid();
        
        var request1 = new PlayerMatchRequest(player1, MatchmakingMode.Solo, new DomainMMR(1500));
        var request2 = new PlayerMatchRequest(player2, MatchmakingMode.Solo, new DomainMMR(1600));
        var request3 = new PlayerMatchRequest(player3, MatchmakingMode.Solo, new DomainMMR(1700));
        
        // Act
        queue.Enqueue(request1);
        queue.Enqueue(request2);
        queue.Enqueue(request3);
        
        // Assert
        queue.Requests.Should().HaveCount(3);
        queue.Requests[0].PlayerId.Should().Be(player1);
        queue.Requests[1].PlayerId.Should().Be(player2);
        queue.Requests[2].PlayerId.Should().Be(player3);
    }
}

