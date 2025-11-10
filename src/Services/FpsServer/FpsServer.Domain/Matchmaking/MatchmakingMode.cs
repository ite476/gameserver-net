namespace FpsServer.Domain.Matchmaking;

/// <summary>
/// 매치메이킹 게임 모드
/// </summary>
public enum MatchmakingMode
{
    /// <summary>
    /// 솔로 매칭 (MVP 지원)
    /// </summary>
    Solo = 1,
    
    /// <summary>
    /// 듀오 매칭 (향후 확장)
    /// </summary>
    Duo = 2,
    
    /// <summary>
    /// 스쿼드 매칭 (향후 확장)
    /// </summary>
    Squad = 4
}

