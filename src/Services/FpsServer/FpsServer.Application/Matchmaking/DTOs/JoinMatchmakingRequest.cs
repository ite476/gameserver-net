using FpsServer.Domain.Matchmaking;

namespace FpsServer.Application.Matchmaking.DTOs;

/// <summary>
/// 매치메이킹 큐 진입 요청 DTO
/// </summary>
public record JoinMatchmakingRequest
{
    /// <summary>
    /// 플레이어 ID
    /// </summary>
    public required Guid PlayerId { get; init; }
    
    /// <summary>
    /// 게임 모드
    /// </summary>
    public required MatchmakingMode GameMode { get; init; }
    
    /// <summary>
    /// 플레이어의 MMR 값
    /// </summary>
    public required int MMR { get; init; }
}

