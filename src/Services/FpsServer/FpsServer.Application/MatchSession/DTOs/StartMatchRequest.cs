namespace FpsServer.Application.MatchSession.DTOs;

/// <summary>
/// 매치 시작 요청 DTO
/// </summary>
public record StartMatchRequest
{
    /// <summary>
    /// 매치 ID
    /// </summary>
    public required Guid MatchId { get; init; }
    
    /// <summary>
    /// 플레이어 ID 목록
    /// </summary>
    public required IReadOnlyList<Guid> PlayerIds { get; init; }
}

