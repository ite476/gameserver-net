namespace FpsServer.Domain.MatchSession;

/// <summary>
/// 플레이어 매치 결과 값 객체
/// </summary>
public record PlayerResult
{
    /// <summary>
    /// 플레이어 ID
    /// </summary>
    public Guid PlayerId { get; init; }
    
    /// <summary>
    /// 승리 여부
    /// </summary>
    public bool IsWinner { get; init; }
    
    /// <summary>
    /// 점수 (게임별 통계, 선택적)
    /// </summary>
    public int? Score { get; init; }
    
    /// <summary>
    /// 플레이어 결과 생성
    /// </summary>
    /// <param name="playerId">플레이어 ID</param>
    /// <param name="isWinner">승리 여부</param>
    /// <param name="score">점수 (선택적)</param>
    public PlayerResult(Guid playerId, bool isWinner, int? score = null)
    {
        if (playerId == Guid.Empty)
            throw new ArgumentException("PlayerId cannot be empty", nameof(playerId));
        
        PlayerId = playerId;
        IsWinner = isWinner;
        Score = score;
    }
}

