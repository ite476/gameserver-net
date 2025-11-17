using FpsServer.Domain.Matchmaking;

namespace FpsServer.Application.Matchmaking.DTOs;

/// <summary>
/// 매칭 성공 응답 DTO
/// </summary>
public record MatchFoundResponse
{
    /// <summary>
    /// 매칭 ID
    /// </summary>
    public required Guid MatchId { get; init; }
    
    /// <summary>
    /// 매칭된 플레이어 ID 목록
    /// </summary>
    public required IReadOnlyList<Guid> Players { get; init; }
    
    /// <summary>
    /// 게임 모드
    /// </summary>
    public required MatchmakingMode GameMode { get; init; }
}

