using FluentAssertions;
using FpsServer.Domain.Matchmaking;
using FpsServer.Infrastructure.Matchmaking;
using DomainMMR = FpsServer.Domain.Matchmaking.MMR;
using Xunit;

namespace FpsServer.Infrastructure.Tests.Matchmaking;

[Trait("Feature", "매치메이킹")]
public class InMemoryMatchmakingRepositoryTests
{
    private readonly InMemoryMatchmakingRepository _repository;
    
    public InMemoryMatchmakingRepositoryTests()
    {
        _repository = new InMemoryMatchmakingRepository();
    }
    
    [Fact]
    [Trait("Category", "메모리 저장소")]
    public async Task 게임_모드가_주어지면_큐를_생성해야_한다()
    {
        // Act
        var queue = await _repository.GetOrCreateQueueAsync(MatchmakingMode.Solo);
        
        // Assert
        queue.Should().NotBeNull();
        queue.GameMode.Should().Be(MatchmakingMode.Solo);
        queue.QueueId.Should().NotBeEmpty();
    }
    
    [Fact]
    [Trait("Category", "메모리 저장소")]
    public async Task 같은_게임_모드면_동일한_큐를_반환해야_한다()
    {
        // Act
        var queue1 = await _repository.GetOrCreateQueueAsync(MatchmakingMode.Solo);
        var queue2 = await _repository.GetOrCreateQueueAsync(MatchmakingMode.Solo);
        
        // Assert
        queue1.Should().BeSameAs(queue2);
        queue1.QueueId.Should().Be(queue2.QueueId);
    }
    
    [Fact]
    [Trait("Category", "메모리 저장소")]
    public async Task 다른_게임_모드면_다른_큐를_반환해야_한다()
    {
        // Act
        var soloQueue = await _repository.GetOrCreateQueueAsync(MatchmakingMode.Solo);
        var duoQueue = await _repository.GetOrCreateQueueAsync(MatchmakingMode.Duo);
        
        // Assert
        soloQueue.Should().NotBeSameAs(duoQueue);
        soloQueue.QueueId.Should().NotBe(duoQueue.QueueId);
    }
    
    [Fact]
    [Trait("Category", "메모리 저장소")]
    public async Task 큐에_플레이어가_있으면_플레이어를_찾을_수_있어야_한다()
    {
        // Arrange
        var queue = await _repository.GetOrCreateQueueAsync(MatchmakingMode.Solo);
        var playerId = Guid.NewGuid();
        var request = new PlayerMatchRequest(playerId, MatchmakingMode.Solo, new DomainMMR(1500));
        queue.Enqueue(request);
        
        // Act
        var found = await _repository.FindByPlayerIdAsync(MatchmakingMode.Solo, playerId);
        
        // Assert
        found.Should().NotBeNull();
        found!.PlayerId.Should().Be(playerId);
    }
    
    [Fact]
    [Trait("Category", "메모리 저장소")]
    public async Task 큐에_플레이어가_없으면_null을_반환해야_한다()
    {
        // Act
        var found = await _repository.FindByPlayerIdAsync(MatchmakingMode.Solo, Guid.NewGuid());
        
        // Assert
        found.Should().BeNull();
    }
}

