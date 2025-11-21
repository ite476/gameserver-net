using FpsServer.Application.MatchSession.DTOs;
using FpsServer.Domain.Matchmaking;
using DomainMatchSession = FpsServer.Domain.MatchSession.MatchSession;
using FpsServer.Domain.MatchSession;

namespace FpsServer.Application.MatchSession.Mappers;

/// <summary>
/// 매치 세션 DTO ↔ Domain 변환 Mapper
/// </summary>
public static class MatchSessionMapper
{
    /// <summary>
    /// StartMatchRequest → Domain 변환 (MatchSession 생성)
    /// </summary>
    public static DomainMatchSession ToDomain(StartMatchRequest request, MatchmakingMode gameMode)
    {
        return new DomainMatchSession(request.MatchId, request.PlayerIds, gameMode);
    }
    
    /// <summary>
    /// MatchSession → StartMatchResponse 변환
    /// </summary>
    public static StartMatchResponse ToStartResponse(DomainMatchSession session)
    {
        return new StartMatchResponse
        {
            SessionId = session.SessionId,
            Status = session.Status
        };
    }
    
    /// <summary>
    /// EndMatchRequest → MatchResult 변환
    /// </summary>
    public static MatchResult ToDomain(EndMatchRequest request)
    {
        var playerResults = request.Results.Select(r => 
            new PlayerResult(r.PlayerId, r.IsWinner, r.Score)).ToList();
        
        return new MatchResult(request.MatchId, playerResults, request.WinnerId);
    }
    
    /// <summary>
    /// MatchSession → EndMatchResponse 변환
    /// </summary>
    public static EndMatchResponse ToEndResponse(DomainMatchSession session)
    {
        return new EndMatchResponse
        {
            SessionId = session.SessionId,
            FinalStatus = session.Status
        };
    }
}

