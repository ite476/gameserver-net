using Microsoft.EntityFrameworkCore;
using FpsServer.Application.MMR.Ports;
using FpsServer.Domain.MMR;
using FpsServer.Infrastructure.Persistence;

namespace FpsServer.Infrastructure.MMR;

/// <summary>
/// EF Core 기반 플레이어 MMR 저장소
/// </summary>
public class EfPlayerMMRRepository : IPlayerMMRRepository
{
    private readonly FpsDbContext _context;
    
    /// <summary>
    /// 생성자
    /// </summary>
    /// <param name="context">DbContext</param>
    public EfPlayerMMRRepository(FpsDbContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
    }
    
    /// <summary>
    /// 플레이어 ID로 MMR 조회
    /// </summary>
    public async Task<PlayerMMR?> FindByPlayerIdAsync(Guid playerId, CancellationToken cancellationToken = default)
    {
        return await _context.PlayerMMRs
            .FirstOrDefaultAsync(p => p.PlayerId == playerId, cancellationToken);
    }
    
    /// <summary>
    /// 여러 플레이어 ID로 MMR 목록 조회
    /// </summary>
    public async Task<IReadOnlyList<PlayerMMR>> FindMultipleByPlayerIdsAsync(
        IReadOnlyList<Guid> playerIds,
        CancellationToken cancellationToken = default)
    {
        return await _context.PlayerMMRs
            .Where(p => playerIds.Contains(p.PlayerId))
            .ToListAsync(cancellationToken);
    }
    
    /// <summary>
    /// 플레이어 MMR 생성 또는 업데이트
    /// </summary>
    public async Task SaveAsync(PlayerMMR playerMMR, CancellationToken cancellationToken = default)
    {
        var existing = await _context.PlayerMMRs
            .FirstOrDefaultAsync(p => p.PlayerId == playerMMR.PlayerId, cancellationToken);
        
        if (existing == null)
        {
            await _context.PlayerMMRs.AddAsync(playerMMR, cancellationToken);
        }
        else
        {
            _context.PlayerMMRs.Update(playerMMR);
        }
        
        await _context.SaveChangesAsync(cancellationToken);
    }
    
    /// <summary>
    /// 여러 플레이어 MMR 일괄 저장
    /// </summary>
    public async Task SaveMultipleAsync(
        IReadOnlyList<PlayerMMR> playerMMRs,
        CancellationToken cancellationToken = default)
    {
        var playerIds = playerMMRs.Select(p => p.PlayerId).ToList();
        var existing = await _context.PlayerMMRs
            .Where(p => playerIds.Contains(p.PlayerId))
            .ToListAsync(cancellationToken);
        
        var existingIds = existing.Select(p => p.PlayerId).ToHashSet();
        
        foreach (var playerMMR in playerMMRs)
        {
            if (existingIds.Contains(playerMMR.PlayerId))
            {
                _context.PlayerMMRs.Update(playerMMR);
            }
            else
            {
                await _context.PlayerMMRs.AddAsync(playerMMR, cancellationToken);
            }
        }
        
        await _context.SaveChangesAsync(cancellationToken);
    }
}

