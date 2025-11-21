using FpsServer.Domain.MatchSession;

namespace FpsServer.Application.MatchSession.DTOs;

/// <summary>
/// 매치 시작 응답 DTO
/// </summary>
public record StartMatchResponse
{
    /// <summary>
    /// 세션 ID
    /// </summary>
    public required Guid SessionId { get; init; }
    
    /// <summary>
    /// 세션 상태
    /// </summary>
    public required MatchStatus Status { get; init; }
}

