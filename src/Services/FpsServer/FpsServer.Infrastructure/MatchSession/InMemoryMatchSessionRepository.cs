using FpsServer.Application.MatchSession.Ports;
using DomainMatchSession = FpsServer.Domain.MatchSession.MatchSession;
using System.Collections.Concurrent;

namespace FpsServer.Infrastructure.MatchSession;

/// <summary>
/// 메모리 기반 매치 세션 저장소 (MVP)
/// 향후 EF Core 기반 구현으로 교체 예정 (#26)
/// </summary>
public class InMemoryMatchSessionRepository : IMatchSessionRepository
{
    private readonly ConcurrentDictionary<Guid, DomainMatchSession> _sessions = new();
    private readonly ConcurrentDictionary<Guid, Guid> _matchIdToSessionId = new(); // MatchId -> SessionId 매핑
    
    /// <summary>
    /// 매치 세션 생성
    /// </summary>
    public Task CreateAsync(DomainMatchSession session, CancellationToken cancellationToken = default)
    {
        _sessions[session.SessionId] = session;
        _matchIdToSessionId[session.MatchId] = session.SessionId;
        return Task.CompletedTask;
    }
    
    /// <summary>
    /// 세션 ID로 매치 세션 조회
    /// </summary>
    public Task<DomainMatchSession?> FindByIdAsync(Guid sessionId, CancellationToken cancellationToken = default)
    {
        _sessions.TryGetValue(sessionId, out var session);
        return Task.FromResult<DomainMatchSession?>(session);
    }
    
    /// <summary>
    /// 매치 ID로 매치 세션 조회
    /// </summary>
    public Task<DomainMatchSession?> FindByMatchIdAsync(Guid matchId, CancellationToken cancellationToken = default)
    {
        if (_matchIdToSessionId.TryGetValue(matchId, out var sessionId))
        {
            _sessions.TryGetValue(sessionId, out var session);
            return Task.FromResult<DomainMatchSession?>(session);
        }
        return Task.FromResult<DomainMatchSession?>(null);
    }
    
    /// <summary>
    /// 매치 세션 업데이트
    /// </summary>
    public Task UpdateAsync(DomainMatchSession session, CancellationToken cancellationToken = default)
    {
        _sessions[session.SessionId] = session;
        return Task.CompletedTask;
    }
}

