using FluentAssertions;
using FpsServer.Domain.Matchmaking;
using Xunit;

namespace FpsServer.Domain.Tests.Matchmaking;

public class MatchmakingDomainServiceTests
{
    [Fact]
    public void TryMatch_WithNullQueue_ShouldThrowArgumentNullException()
    {
        // Arrange
        var service = new MatchmakingDomainService();
        
        // Act
        var act = () => service.TryMatch(null!);
        
        // Assert
        act.Should().Throw<ArgumentNullException>()
            .And.ParamName.Should().Be("queue");
    }
    
    [Fact]
    public void TryMatch_WithEmptyQueue_ShouldReturnNull()
    {
        // Arrange
        var service = new MatchmakingDomainService();
        var queue = new MatchmakingQueue(MatchmakingMode.Solo);
        
        // Act
        var result = service.TryMatch(queue);
        
        // Assert
        result.Should().BeNull();
    }
    
    [Fact]
    public void TryMatch_WithSinglePlayer_ShouldReturnNull()
    {
        // Arrange
        var service = new MatchmakingDomainService();
        var queue = new MatchmakingQueue(MatchmakingMode.Solo);
        var player1 = new PlayerMatchRequest(Guid.NewGuid(), MatchmakingMode.Solo, new MMR(1500));
        queue.Enqueue(player1);
        
        // Act
        var result = service.TryMatch(queue);
        
        // Assert
        result.Should().BeNull();
        queue.Requests.Should().HaveCount(1); // 플레이어가 큐에 남아있어야 함
    }
    
    [Fact]
    public void TryMatch_WithTwoPlayersWithinMMRTolerance_ShouldCreateMatch()
    {
        // Arrange
        var service = new MatchmakingDomainService();
        var queue = new MatchmakingQueue(MatchmakingMode.Solo);
        var player1 = new PlayerMatchRequest(Guid.NewGuid(), MatchmakingMode.Solo, new MMR(1500));
        var player2 = new PlayerMatchRequest(Guid.NewGuid(), MatchmakingMode.Solo, new MMR(1550)); // ±100 범위 내
        queue.Enqueue(player1);
        queue.Enqueue(player2);
        
        // Act
        var result = service.TryMatch(queue);
        
        // Assert
        result.Should().NotBeNull();
        result!.Players.Should().HaveCount(2);
        result.Players.Should().Contain(player1);
        result.Players.Should().Contain(player2);
        result.GameMode.Should().Be(MatchmakingMode.Solo);
        queue.Requests.Should().BeEmpty(); // 매칭된 플레이어들이 큐에서 제거되어야 함
    }
    
    [Fact]
    public void TryMatch_WithTwoPlayersOutsideMMRTolerance_ShouldReturnNull()
    {
        // Arrange
        var service = new MatchmakingDomainService();
        var queue = new MatchmakingQueue(MatchmakingMode.Solo);
        var player1 = new PlayerMatchRequest(Guid.NewGuid(), MatchmakingMode.Solo, new MMR(1500));
        var player2 = new PlayerMatchRequest(Guid.NewGuid(), MatchmakingMode.Solo, new MMR(1700)); // ±100 범위 밖
        queue.Enqueue(player1);
        queue.Enqueue(player2);
        
        // Act
        var result = service.TryMatch(queue);
        
        // Assert
        result.Should().BeNull();
        queue.Requests.Should().HaveCount(2); // 플레이어들이 큐에 남아있어야 함
    }
    
    [Fact]
    public void TryMatch_WithThreePlayers_FirstTwoWithinTolerance_ShouldMatchFirstTwo()
    {
        // Arrange
        var service = new MatchmakingDomainService();
        var queue = new MatchmakingQueue(MatchmakingMode.Solo);
        var player1 = new PlayerMatchRequest(Guid.NewGuid(), MatchmakingMode.Solo, new MMR(1500));
        var player2 = new PlayerMatchRequest(Guid.NewGuid(), MatchmakingMode.Solo, new MMR(1550)); // ±100 범위 내
        var player3 = new PlayerMatchRequest(Guid.NewGuid(), MatchmakingMode.Solo, new MMR(1700)); // ±100 범위 밖
        queue.Enqueue(player1);
        queue.Enqueue(player2);
        queue.Enqueue(player3);
        
        // Act
        var result = service.TryMatch(queue);
        
        // Assert
        result.Should().NotBeNull();
        result!.Players.Should().HaveCount(2);
        result.Players.Should().Contain(player1);
        result.Players.Should().Contain(player2);
        result.Players.Should().NotContain(player3);
        queue.Requests.Should().HaveCount(1); // player3만 남아있어야 함
        queue.Requests[0].PlayerId.Should().Be(player3.PlayerId);
    }
    
    [Fact]
    public void TryMatch_WithExactMMRTolerance_ShouldMatch()
    {
        // Arrange
        var service = new MatchmakingDomainService();
        var queue = new MatchmakingQueue(MatchmakingMode.Solo);
        var player1 = new PlayerMatchRequest(Guid.NewGuid(), MatchmakingMode.Solo, new MMR(1500));
        var player2 = new PlayerMatchRequest(Guid.NewGuid(), MatchmakingMode.Solo, new MMR(1600)); // 정확히 ±100
        queue.Enqueue(player1);
        queue.Enqueue(player2);
        
        // Act
        var result = service.TryMatch(queue);
        
        // Assert
        result.Should().NotBeNull();
        result!.Players.Should().HaveCount(2);
    }
    
    [Fact]
    public void TryMatch_ShouldRespectEnqueueOrder()
    {
        // Arrange
        var service = new MatchmakingDomainService();
        var queue = new MatchmakingQueue(MatchmakingMode.Solo);
        
        // player1이 먼저 진입, player2는 나중에 진입하지만 MMR이 더 가까움
        var player1 = new PlayerMatchRequest(Guid.NewGuid(), MatchmakingMode.Solo, new MMR(1500));
        var player2 = new PlayerMatchRequest(Guid.NewGuid(), MatchmakingMode.Solo, new MMR(1505)); // 더 가까움
        var player3 = new PlayerMatchRequest(Guid.NewGuid(), MatchmakingMode.Solo, new MMR(1550)); // player1과 매칭 가능
        
        queue.Enqueue(player1);
        queue.Enqueue(player2);
        queue.Enqueue(player3);
        
        // Act
        var result = service.TryMatch(queue);
        
        // Assert
        result.Should().NotBeNull();
        result!.Players.Should().HaveCount(2);
        // 선입선출 원칙: player1과 매칭 가능한 첫 번째 플레이어(player2)가 매칭되어야 함
        result.Players.Should().Contain(player1);
        result.Players.Should().Contain(player2);
    }
    
    [Fact]
    public void TryMatch_WithMultipleValidPairs_ShouldMatchFirstPair()
    {
        // Arrange
        var service = new MatchmakingDomainService();
        var queue = new MatchmakingQueue(MatchmakingMode.Solo);
        
        var player1 = new PlayerMatchRequest(Guid.NewGuid(), MatchmakingMode.Solo, new MMR(1500));
        var player2 = new PlayerMatchRequest(Guid.NewGuid(), MatchmakingMode.Solo, new MMR(1550));
        var player3 = new PlayerMatchRequest(Guid.NewGuid(), MatchmakingMode.Solo, new MMR(1500));
        var player4 = new PlayerMatchRequest(Guid.NewGuid(), MatchmakingMode.Solo, new MMR(1550));
        
        queue.Enqueue(player1);
        queue.Enqueue(player2);
        queue.Enqueue(player3);
        queue.Enqueue(player4);
        
        // Act
        var result = service.TryMatch(queue);
        
        // Assert
        result.Should().NotBeNull();
        result!.Players.Should().HaveCount(2);
        // 첫 번째 쌍(player1, player2)이 매칭되어야 함
        result.Players.Should().Contain(player1);
        result.Players.Should().Contain(player2);
        queue.Requests.Should().HaveCount(2); // player3, player4가 남아있어야 함
    }
}

