namespace FpsServer.Application.MatchSession.DTOs;

/// <summary>
/// 매치 종료 요청 DTO
/// </summary>
public record EndMatchRequest
{
    /// <summary>
    /// 매치 ID
    /// </summary>
    public required Guid MatchId { get; init; }
    
    /// <summary>
    /// 플레이어 결과 목록
    /// </summary>
    public required IReadOnlyList<PlayerResultDto> Results { get; init; }
    
    /// <summary>
    /// 승리 팀/플레이어 ID (선택적)
    /// </summary>
    public Guid? WinnerId { get; init; }
}

/// <summary>
/// 플레이어 결과 DTO
/// </summary>
public record PlayerResultDto
{
    /// <summary>
    /// 플레이어 ID
    /// </summary>
    public required Guid PlayerId { get; init; }
    
    /// <summary>
    /// 승리 여부
    /// </summary>
    public required bool IsWinner { get; init; }
    
    /// <summary>
    /// 점수 (선택적)
    /// </summary>
    public int? Score { get; init; }
}

