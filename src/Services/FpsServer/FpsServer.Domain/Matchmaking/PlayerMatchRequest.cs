namespace FpsServer.Domain.Matchmaking;

/// <summary>
/// 플레이어의 매칭 요청 엔티티
/// </summary>
public class PlayerMatchRequest
{
    /// <summary>
    /// 요청 ID
    /// </summary>
    public Guid RequestId { get; private set; }
    
    /// <summary>
    /// 플레이어 ID
    /// </summary>
    public Guid PlayerId { get; private set; }
    
    /// <summary>
    /// 게임 모드
    /// </summary>
    public MatchmakingMode GameMode { get; private set; }
    
    /// <summary>
    /// 플레이어의 MMR 값
    /// </summary>
    public MMR PlayerMMR { get; private set; }
    
    /// <summary>
    /// 큐 진입 시간
    /// </summary>
    public DateTime EnqueuedAt { get; private set; }
    
    // EF Core용 private 생성자
    private PlayerMatchRequest() { }
    
    /// <summary>
    /// 플레이어 매칭 요청 생성
    /// </summary>
    /// <param name="playerId">플레이어 ID</param>
    /// <param name="gameMode">게임 모드</param>
    /// <param name="playerMMR">플레이어의 MMR 값</param>
    public PlayerMatchRequest(Guid playerId, MatchmakingMode gameMode, MMR playerMMR)
    {
        RequestId = Guid.NewGuid();
        PlayerId = playerId;
        GameMode = gameMode;
        PlayerMMR = playerMMR;
        EnqueuedAt = DateTime.UtcNow;
    }
}

