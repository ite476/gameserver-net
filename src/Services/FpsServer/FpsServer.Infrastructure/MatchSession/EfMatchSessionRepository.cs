using Microsoft.EntityFrameworkCore;
using FpsServer.Application.MatchSession.Ports;
using FpsServer.Infrastructure.Persistence;
using DomainMatchSession = FpsServer.Domain.MatchSession.MatchSession;

namespace FpsServer.Infrastructure.MatchSession;

/// <summary>
/// EF Core 기반 매치 세션 저장소
/// </summary>
public class EfMatchSessionRepository : IMatchSessionRepository
{
    private readonly FpsDbContext _context;
    
    /// <summary>
    /// 생성자
    /// </summary>
    /// <param name="context">DbContext</param>
    public EfMatchSessionRepository(FpsDbContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
    }
    
    /// <summary>
    /// 매치 세션 생성
    /// </summary>
    public async Task CreateAsync(DomainMatchSession session, CancellationToken cancellationToken = default)
    {
        await _context.MatchSessions.AddAsync(session, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
    }
    
    /// <summary>
    /// 세션 ID로 매치 세션 조회
    /// </summary>
    public async Task<DomainMatchSession?> FindByIdAsync(Guid sessionId, CancellationToken cancellationToken = default)
    {
        return await _context.MatchSessions
            .FirstOrDefaultAsync(s => s.SessionId == sessionId, cancellationToken);
    }
    
    /// <summary>
    /// 매치 ID로 매치 세션 조회
    /// </summary>
    public async Task<DomainMatchSession?> FindByMatchIdAsync(Guid matchId, CancellationToken cancellationToken = default)
    {
        return await _context.MatchSessions
            .FirstOrDefaultAsync(s => s.MatchId == matchId, cancellationToken);
    }
    
    /// <summary>
    /// 매치 세션 업데이트
    /// </summary>
    public async Task UpdateAsync(DomainMatchSession session, CancellationToken cancellationToken = default)
    {
        _context.MatchSessions.Update(session);
        await _context.SaveChangesAsync(cancellationToken);
    }
}

