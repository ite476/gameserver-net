namespace FpsServer.Domain.MatchSession;

/// <summary>
/// 매치 결과 엔티티
/// </summary>
public class MatchResult
{
    /// <summary>
    /// 매치 결과 ID
    /// </summary>
    public Guid ResultId { get; private set; }
    
    /// <summary>
    /// 매치 ID
    /// </summary>
    public Guid MatchId { get; private set; }
    
    /// <summary>
    /// 플레이어 결과 목록
    /// </summary>
    public IReadOnlyList<PlayerResult> PlayerResults { get; private set; }
    
    /// <summary>
    /// 승리 팀/플레이어 ID (선택적)
    /// </summary>
    public Guid? WinnerId { get; private set; }
    
    /// <summary>
    /// 매치 종료 시간
    /// </summary>
    public DateTimeOffset EndedAt { get; private set; }
    
    // EF Core용 private 생성자
    private MatchResult()
    {
        PlayerResults = new List<PlayerResult>();
    }
    
    /// <summary>
    /// 매치 결과 생성
    /// </summary>
    /// <param name="matchId">매치 ID</param>
    /// <param name="playerResults">플레이어 결과 목록</param>
    /// <param name="winnerId">승리 팀/플레이어 ID (선택적)</param>
    /// <exception cref="ArgumentNullException">플레이어 결과 목록이 null인 경우</exception>
    /// <exception cref="ArgumentException">플레이어 결과 목록이 비어있는 경우</exception>
    public MatchResult(
        Guid matchId, 
        IReadOnlyList<PlayerResult> playerResults, 
        Guid? winnerId = null)
    {
        if (matchId == Guid.Empty)
            throw new ArgumentException("MatchId cannot be empty", nameof(matchId));
        
        if (playerResults == null)
            throw new ArgumentNullException(nameof(playerResults));
        
        if (playerResults.Count == 0)
            throw new ArgumentException("PlayerResults cannot be empty", nameof(playerResults));
        
        ResultId = Guid.NewGuid();
        MatchId = matchId;
        PlayerResults = playerResults;
        WinnerId = winnerId;
        EndedAt = DateTimeOffset.UtcNow;
    }
}

