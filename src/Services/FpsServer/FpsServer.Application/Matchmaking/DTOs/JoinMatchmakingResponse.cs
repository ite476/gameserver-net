namespace FpsServer.Application.Matchmaking.DTOs;

/// <summary>
/// 매치메이킹 큐 진입 응답 DTO
/// </summary>
public record JoinMatchmakingResponse
{
    /// <summary>
    /// 요청 ID
    /// </summary>
    public required Guid RequestId { get; init; }
    
    /// <summary>
    /// 상태 (예: "Enqueued", "Matched")
    /// </summary>
    public required string Status { get; init; }
}

