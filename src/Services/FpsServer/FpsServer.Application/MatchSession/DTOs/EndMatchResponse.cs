using FpsServer.Domain.MatchSession;

namespace FpsServer.Application.MatchSession.DTOs;

/// <summary>
/// 매치 종료 응답 DTO
/// </summary>
public record EndMatchResponse
{
    /// <summary>
    /// 세션 ID
    /// </summary>
    public required Guid SessionId { get; init; }
    
    /// <summary>
    /// 최종 상태
    /// </summary>
    public required MatchStatus FinalStatus { get; init; }
}

