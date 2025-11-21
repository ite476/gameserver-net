using FpsServer.Application.Matchmaking.DTOs;
using FpsServer.Domain.Matchmaking;
using DomainMMR = FpsServer.Domain.Matchmaking.MMR;

namespace FpsServer.Application.Matchmaking.Mappers;

/// <summary>
/// 매치메이킹 DTO ↔ Domain 변환 Mapper
/// </summary>
public static class MatchmakingMapper
{
    /// <summary>
    /// JoinMatchmakingRequest → PlayerMatchRequest 변환
    /// </summary>
    public static PlayerMatchRequest ToDomain(JoinMatchmakingRequest request)
    {
        return new PlayerMatchRequest(
            request.PlayerId,
            request.GameMode,
            new DomainMMR(request.MMR)
        );
    }
    
    /// <summary>
    /// PlayerMatchRequest → JoinMatchmakingResponse 변환
    /// </summary>
    public static JoinMatchmakingResponse ToResponse(PlayerMatchRequest request)
    {
        return new JoinMatchmakingResponse
        {
            RequestId = request.RequestId,
            Status = "Enqueued"
        };
    }
    
    /// <summary>
    /// Match → MatchFoundResponse 변환
    /// </summary>
    public static MatchFoundResponse ToResponse(Match match)
    {
        return new MatchFoundResponse
        {
            MatchId = match.MatchId,
            Players = match.Players.Select(p => p.PlayerId).ToList(),
            GameMode = match.GameMode
        };
    }
}

