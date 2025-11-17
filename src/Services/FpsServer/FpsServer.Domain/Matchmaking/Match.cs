namespace FpsServer.Domain.Matchmaking;

/// <summary>
/// 매칭 성공 결과 엔티티
/// </summary>
public class Match
{
    /// <summary>
    /// 매칭 ID
    /// </summary>
    public Guid MatchId { get; private set; }
    
    /// <summary>
    /// 게임 모드
    /// </summary>
    public MatchmakingMode GameMode { get; private set; }
    
    /// <summary>
    /// 매칭된 플레이어 목록
    /// </summary>
    public IReadOnlyList<PlayerMatchRequest> Players { get; private set; }
    
    /// <summary>
    /// 매칭 성공 시간
    /// </summary>
    public DateTimeOffset MatchedAt { get; private set; }
    
    // EF Core용 private 생성자
    private Match() 
    {
        Players = new List<PlayerMatchRequest>();
    }
    
    /// <summary>
    /// 매칭 엔티티 생성
    /// </summary>
    /// <param name="gameMode">게임 모드</param>
    /// <param name="players">매칭된 플레이어 목록</param>
    /// <exception cref="ArgumentNullException">플레이어 목록이 null인 경우</exception>
    public Match(MatchmakingMode gameMode, IReadOnlyList<PlayerMatchRequest> players)
    {
        if (players == null)
            throw new ArgumentNullException(nameof(players));
        
        MatchId = Guid.NewGuid();
        GameMode = gameMode;
        Players = players;
        MatchedAt = DateTimeOffset.UtcNow;
    }
}

