using FpsServer.Application.MMR.Ports;
using FpsServer.Domain.MMR;
using System.Collections.Concurrent;

namespace FpsServer.Infrastructure.MMR;

/// <summary>
/// 메모리 기반 플레이어 MMR 저장소 (MVP)
/// 향후 EF Core 기반 구현으로 교체 예정 (#26)
/// </summary>
public class InMemoryPlayerMMRRepository : IPlayerMMRRepository
{
    private readonly ConcurrentDictionary<Guid, PlayerMMR> _playerMMRs = new();
    
    /// <summary>
    /// 플레이어 ID로 MMR 조회
    /// </summary>
    public Task<PlayerMMR?> FindByPlayerIdAsync(Guid playerId, CancellationToken cancellationToken = default)
    {
        _playerMMRs.TryGetValue(playerId, out var playerMMR);
        return Task.FromResult<PlayerMMR?>(playerMMR);
    }
    
    /// <summary>
    /// 여러 플레이어 ID로 MMR 목록 조회
    /// </summary>
    public Task<IReadOnlyList<PlayerMMR>> FindMultipleByPlayerIdsAsync(
        IReadOnlyList<Guid> playerIds,
        CancellationToken cancellationToken = default)
    {
        var results = playerIds
            .Where(id => _playerMMRs.ContainsKey(id))
            .Select(id => _playerMMRs[id])
            .ToList();
        
        return Task.FromResult<IReadOnlyList<PlayerMMR>>(results);
    }
    
    /// <summary>
    /// 플레이어 MMR 생성 또는 업데이트
    /// </summary>
    public Task SaveAsync(PlayerMMR playerMMR, CancellationToken cancellationToken = default)
    {
        _playerMMRs[playerMMR.PlayerId] = playerMMR;
        return Task.CompletedTask;
    }
    
    /// <summary>
    /// 여러 플레이어 MMR 일괄 저장
    /// </summary>
    public Task SaveMultipleAsync(
        IReadOnlyList<PlayerMMR> playerMMRs,
        CancellationToken cancellationToken = default)
    {
        foreach (var playerMMR in playerMMRs)
        {
            _playerMMRs[playerMMR.PlayerId] = playerMMR;
        }
        return Task.CompletedTask;
    }
}

