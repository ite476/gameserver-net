using System.Collections.Concurrent;
using FpsServer.Application.Matchmaking.Ports;
using FpsServer.Domain.Matchmaking;

namespace FpsServer.Infrastructure.Matchmaking;

/// <summary>
/// 메모리 기반 매치메이킹 큐 저장소 구현
/// MVP 단계에서 사용하며, 향후 Redis 기반으로 확장 가능합니다.
/// </summary>
public class InMemoryMatchmakingRepository : IMatchmakingRepository
{
    private readonly ConcurrentDictionary<MatchmakingMode, MatchmakingQueue> _queues = new();
    private readonly object _lockObject = new();
    
    /// <summary>
    /// 큐에 플레이어 요청 추가
    /// </summary>
    public Task EnqueueAsync(MatchmakingQueue queue, PlayerMatchRequest request, CancellationToken cancellationToken = default)
    {
        // 큐는 이미 GetOrCreateQueueAsync에서 가져온 것이므로, 
        // Domain의 큐에 이미 추가되어 있을 것입니다.
        // 여기서는 큐 자체를 저장소에 유지하는 역할만 합니다.
        return Task.CompletedTask;
    }
    
    /// <summary>
    /// 큐에서 플레이어 요청 제거 (매칭 성공 시)
    /// </summary>
    public Task DequeueAsync(MatchmakingQueue queue, IEnumerable<Guid> requestIds, CancellationToken cancellationToken = default)
    {
        // Domain의 큐에서 이미 제거되었을 것이므로,
        // 여기서는 추가 작업이 필요 없습니다.
        return Task.CompletedTask;
    }
    
    /// <summary>
    /// 플레이어 ID로 큐에서 요청 찾기
    /// </summary>
    public Task<PlayerMatchRequest?> FindByPlayerIdAsync(MatchmakingMode gameMode, Guid playerId, CancellationToken cancellationToken = default)
    {
        if (!_queues.TryGetValue(gameMode, out var queue))
            return Task.FromResult<PlayerMatchRequest?>(null);
        
        var request = queue.FindByPlayerId(playerId);
        return Task.FromResult(request);
    }
    
    /// <summary>
    /// 큐에서 플레이어 요청 취소
    /// </summary>
    public Task CancelAsync(MatchmakingQueue queue, Guid playerId, CancellationToken cancellationToken = default)
    {
        // Domain의 큐에서 이미 취소되었을 것이므로,
        // 여기서는 추가 작업이 필요 없습니다.
        return Task.CompletedTask;
    }
    
    /// <summary>
    /// 게임 모드별 큐 조회 (없으면 생성)
    /// </summary>
    public Task<MatchmakingQueue> GetOrCreateQueueAsync(MatchmakingMode gameMode, CancellationToken cancellationToken = default)
    {
        return Task.FromResult(_queues.GetOrAdd(gameMode, _ => new MatchmakingQueue(gameMode)));
    }
}

