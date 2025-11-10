using FluentAssertions;
using FpsServer.Domain.Matchmaking;
using FpsServer.Domain.Matchmaking.Exceptions;
using Xunit;

namespace FpsServer.Domain.Tests.Matchmaking;

public class MatchmakingQueueTests
{
    [Fact]
    public void Constructor_WithGameMode_ShouldCreateQueue()
    {
        // Arrange & Act
        var queue = new MatchmakingQueue(MatchmakingMode.Solo);
        
        // Assert
        queue.GameMode.Should().Be(MatchmakingMode.Solo);
        queue.QueueId.Should().NotBeEmpty();
        queue.Requests.Should().BeEmpty();
    }
    
    [Fact]
    public void Enqueue_WithValidRequest_ShouldAddToQueue()
    {
        // Arrange
        var queue = new MatchmakingQueue(MatchmakingMode.Solo);
        var playerId = Guid.NewGuid();
        var request = new PlayerMatchRequest(playerId, MatchmakingMode.Solo, new MMR(1500));
        
        // Act
        queue.Enqueue(request);
        
        // Assert
        queue.Requests.Should().HaveCount(1);
        queue.Requests[0].PlayerId.Should().Be(playerId);
    }
    
    [Fact]
    public void Enqueue_WithNullRequest_ShouldThrowArgumentNullException()
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
    public void Enqueue_WithDuplicatePlayer_ShouldThrowPlayerAlreadyInQueueException()
    {
        // Arrange
        var queue = new MatchmakingQueue(MatchmakingMode.Solo);
        var playerId = Guid.NewGuid();
        var request1 = new PlayerMatchRequest(playerId, MatchmakingMode.Solo, new MMR(1500));
        var request2 = new PlayerMatchRequest(playerId, MatchmakingMode.Solo, new MMR(1600));
        
        queue.Enqueue(request1);
        
        // Act
        var act = () => queue.Enqueue(request2);
        
        // Assert
        act.Should().Throw<PlayerAlreadyInQueueException>()
            .Which.PlayerId.Should().Be(playerId);
    }
    
    [Fact]
    public void Enqueue_WithMismatchedGameMode_ShouldThrowInvalidMatchmakingRequestException()
    {
        // Arrange
        var queue = new MatchmakingQueue(MatchmakingMode.Solo);
        var request = new PlayerMatchRequest(Guid.NewGuid(), MatchmakingMode.Duo, new MMR(1500));
        
        // Act
        var act = () => queue.Enqueue(request);
        
        // Assert
        act.Should().Throw<InvalidMatchmakingRequestException>()
            .WithMessage("*Game mode mismatch*");
    }
    
    [Fact]
    public void Cancel_WithExistingPlayer_ShouldRemoveFromQueue()
    {
        // Arrange
        var queue = new MatchmakingQueue(MatchmakingMode.Solo);
        var playerId = Guid.NewGuid();
        var request = new PlayerMatchRequest(playerId, MatchmakingMode.Solo, new MMR(1500));
        queue.Enqueue(request);
        
        // Act
        queue.Cancel(playerId);
        
        // Assert
        queue.Requests.Should().BeEmpty();
    }
    
    [Fact]
    public void Cancel_WithNonExistentPlayer_ShouldThrowPlayerNotInQueueException()
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
    public void FindByPlayerId_WithExistingPlayer_ShouldReturnRequest()
    {
        // Arrange
        var queue = new MatchmakingQueue(MatchmakingMode.Solo);
        var playerId = Guid.NewGuid();
        var request = new PlayerMatchRequest(playerId, MatchmakingMode.Solo, new MMR(1500));
        queue.Enqueue(request);
        
        // Act
        var result = queue.FindByPlayerId(playerId);
        
        // Assert
        result.Should().NotBeNull();
        result!.PlayerId.Should().Be(playerId);
    }
    
    [Fact]
    public void FindByPlayerId_WithNonExistentPlayer_ShouldReturnNull()
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
    public void Enqueue_MultiplePlayers_ShouldMaintainOrder()
    {
        // Arrange
        var queue = new MatchmakingQueue(MatchmakingMode.Solo);
        var player1 = Guid.NewGuid();
        var player2 = Guid.NewGuid();
        var player3 = Guid.NewGuid();
        
        var request1 = new PlayerMatchRequest(player1, MatchmakingMode.Solo, new MMR(1500));
        var request2 = new PlayerMatchRequest(player2, MatchmakingMode.Solo, new MMR(1600));
        var request3 = new PlayerMatchRequest(player3, MatchmakingMode.Solo, new MMR(1700));
        
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

