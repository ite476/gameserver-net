namespace FpsServer.Application.MMR.DTOs;

/// <summary>
/// MMR 업데이트 요청 DTO
/// </summary>
public record UpdateMMRRequest
{
    /// <summary>
    /// 플레이어 ID
    /// </summary>
    public required Guid PlayerId { get; init; }
    
    /// <summary>
    /// 매치 ID
    /// </summary>
    public required Guid MatchId { get; init; }
    
    /// <summary>
    /// 승리 여부 (true: 승리, false: 패배)
    /// </summary>
    public required bool IsWinner { get; init; }
    
    /// <summary>
    /// 상대방 평균 MMR (팀 매칭의 경우 팀 평균)
    /// </summary>
    public required int OpponentAverageMMR { get; init; }
    
    /// <summary>
    /// K 값 (기본값: 32, 조정 가능)
    /// </summary>
    public int KFactor { get; init; } = 32;
}

