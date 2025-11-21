namespace FpsServer.Domain.MatchSession;

/// <summary>
/// 매치 세션 상태
/// </summary>
public enum MatchStatus
{
    /// <summary>
    /// 매칭 완료 (게임 시작 전)
    /// </summary>
    Matched = 1,
    
    /// <summary>
    /// 게임 시작 중 (초기화 단계)
    /// </summary>
    Starting = 2,
    
    /// <summary>
    /// 게임 진행 중
    /// </summary>
    InProgress = 3,
    
    /// <summary>
    /// 게임 종료
    /// </summary>
    Finished = 4,
    
    /// <summary>
    /// 게임 취소됨
    /// </summary>
    Cancelled = 5
}

