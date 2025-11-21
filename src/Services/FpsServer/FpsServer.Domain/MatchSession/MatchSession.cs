using FpsServer.Domain.Matchmaking;
using FpsServer.Domain.MatchSession.Exceptions;

namespace FpsServer.Domain.MatchSession;

/// <summary>
/// 매치 세션 Aggregate Root
/// 게임 세션의 라이프사이클을 관리하며, 상태 전이 규칙을 보장합니다.
/// </summary>
public class MatchSession
{
    private MatchResult? _result;
    
    /// <summary>
    /// 세션 ID
    /// </summary>
    public Guid SessionId { get; private set; }
    
    /// <summary>
    /// 매치 ID (매칭 시스템에서 생성된 ID)
    /// </summary>
    public Guid MatchId { get; private set; }
    
    /// <summary>
    /// 세션 상태
    /// </summary>
    public MatchStatus Status { get; private set; }
    
    /// <summary>
    /// 게임 시작 시간
    /// </summary>
    public DateTimeOffset? StartedAt { get; private set; }
    
    /// <summary>
    /// 게임 종료 시간
    /// </summary>
    public DateTimeOffset? EndedAt { get; private set; }
    
    /// <summary>
    /// 플레이어 ID 목록
    /// </summary>
    public IReadOnlyList<Guid> PlayerIds { get; private set; }
    
    /// <summary>
    /// 게임 모드
    /// </summary>
    public MatchmakingMode GameMode { get; private set; }
    
    /// <summary>
    /// 매치 결과 (게임 종료 후 설정)
    /// </summary>
    public MatchResult? Result => _result;
    
    // EF Core용 private 생성자
    private MatchSession()
    {
        PlayerIds = new List<Guid>();
    }
    
    /// <summary>
    /// 매치 세션 생성
    /// </summary>
    /// <param name="matchId">매치 ID</param>
    /// <param name="playerIds">플레이어 ID 목록</param>
    /// <param name="gameMode">게임 모드</param>
    /// <exception cref="ArgumentException">매치 ID가 비어있거나 플레이어 목록이 비어있는 경우</exception>
    public MatchSession(Guid matchId, IReadOnlyList<Guid> playerIds, MatchmakingMode gameMode)
    {
        if (matchId == Guid.Empty)
            throw new ArgumentException("MatchId cannot be empty", nameof(matchId));
        
        if (playerIds == null || playerIds.Count == 0)
            throw new ArgumentException("PlayerIds cannot be null or empty", nameof(playerIds));
        
        SessionId = Guid.NewGuid();
        MatchId = matchId;
        PlayerIds = playerIds;
        GameMode = gameMode;
        Status = MatchStatus.Matched;
    }
    
    /// <summary>
    /// 게임 시작
    /// 상태 전이: Matched → InProgress
    /// </summary>
    /// <exception cref="InvalidMatchSessionStateException">현재 상태에서 시작할 수 없는 경우</exception>
    public void Start()
    {
        if (Status != MatchStatus.Matched)
            throw new InvalidMatchSessionStateException(Status, MatchStatus.InProgress);
        
        Status = MatchStatus.InProgress;
        StartedAt = DateTimeOffset.UtcNow;
    }
    
    /// <summary>
    /// 게임 종료
    /// 상태 전이: InProgress → Finished
    /// </summary>
    /// <param name="result">매치 결과</param>
    /// <exception cref="ArgumentNullException">결과가 null인 경우</exception>
    /// <exception cref="InvalidMatchSessionStateException">현재 상태에서 종료할 수 없는 경우</exception>
    public void End(MatchResult result)
    {
        if (result == null)
            throw new ArgumentNullException(nameof(result));
        
        if (Status != MatchStatus.InProgress)
            throw new InvalidMatchSessionStateException(Status, MatchStatus.Finished);
        
        if (result.MatchId != MatchId)
            throw new ArgumentException($"MatchId mismatch. Session MatchId: {MatchId}, Result MatchId: {result.MatchId}", nameof(result));
        
        Status = MatchStatus.Finished;
        EndedAt = DateTimeOffset.UtcNow;
        _result = result;
    }
    
    /// <summary>
    /// 게임 취소
    /// 상태 전이: Matched 또는 InProgress → Cancelled
    /// </summary>
    /// <exception cref="InvalidMatchSessionStateException">현재 상태에서 취소할 수 없는 경우</exception>
    public void Cancel()
    {
        if (Status != MatchStatus.Matched && Status != MatchStatus.InProgress)
            throw new InvalidMatchSessionStateException(Status, MatchStatus.Cancelled);
        
        Status = MatchStatus.Cancelled;
        EndedAt = DateTimeOffset.UtcNow;
    }
}

