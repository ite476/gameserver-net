using FluentAssertions;
using FpsServer.Domain.Matchmaking;
using Xunit;

namespace FpsServer.Domain.Tests.Matchmaking;

[Trait("Feature", "매치메이킹")]
public class MatchmakingDomainServiceTests
{
    [Fact]
    [Trait("Category", "매칭 도메인 서비스")]
    public void null_큐면_ArgumentNullException을_발생시켜야_한다()
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
    [Trait("Category", "매칭 도메인 서비스")]
    public void 빈_큐면_null을_반환해야_한다()
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
    [Trait("Category", "매칭 도메인 서비스")]
    public void 플레이어가_1명이면_null을_반환해야_한다()
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
    [Trait("Category", "매칭 도메인 서비스")]
    public void MMR_허용_범위_내_두_플레이어면_매칭을_생성해야_한다()
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
    [Trait("Category", "매칭 도메인 서비스")]
    public void MMR_허용_범위_밖_두_플레이어면_null을_반환해야_한다()
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
    [Trait("Category", "매칭 도메인 서비스")]
    public void 세_플레이어_중_처음_두_명이_허용_범위_내면_처음_두_명을_매칭해야_한다()
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
    [Trait("Category", "매칭 도메인 서비스")]
    public void 정확히_MMR_허용_범위_경계면_매칭해야_한다()
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
    [Trait("Category", "매칭 도메인 서비스")]
    public void 큐_진입_순서를_준수해야_한다()
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
    [Trait("Category", "매칭 도메인 서비스")]
    public void 여러_유효한_쌍이_있으면_첫_번째_쌍을_매칭해야_한다()
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

