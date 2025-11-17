using FpsServer.Domain.Matchmaking.Exceptions;

namespace FpsServer.Domain.Matchmaking;

/// <summary>
/// 매치메이킹 큐 Aggregate Root
/// 게임 모드별 큐를 관리하며, 큐에 플레이어 추가/제거, 중복 진입 방지 등의 비즈니스 규칙을 보장합니다.
/// </summary>
public class MatchmakingQueue
{
    private readonly List<PlayerMatchRequest> _requests = new();
    
    /// <summary>
    /// 큐 ID
    /// </summary>
    public Guid QueueId { get; private set; }
    
    /// <summary>
    /// 게임 모드
    /// </summary>
    public MatchmakingMode GameMode { get; private set; }
    
    /// <summary>
    /// 큐에 등록된 플레이어 요청 목록 (읽기 전용)
    /// </summary>
    public IReadOnlyList<PlayerMatchRequest> Requests => _requests.AsReadOnly();
    
    // EF Core용 private 생성자
    private MatchmakingQueue() 
    {
        _requests = new List<PlayerMatchRequest>();
    }
    
    /// <summary>
    /// 매치메이킹 큐 생성
    /// </summary>
    /// <param name="gameMode">게임 모드</param>
    public MatchmakingQueue(MatchmakingMode gameMode)
    {
        QueueId = Guid.NewGuid();
        GameMode = gameMode;
    }
    
    /// <summary>
    /// 큐에 플레이어 추가
    /// </summary>
    /// <param name="request">플레이어 매칭 요청</param>
    /// <exception cref="ArgumentNullException">요청이 null인 경우</exception>
    /// <exception cref="PlayerAlreadyInQueueException">플레이어가 이미 큐에 있는 경우</exception>
    /// <exception cref="InvalidMatchmakingRequestException">게임 모드가 일치하지 않는 경우</exception>
    public void Enqueue(PlayerMatchRequest request)
    {
        if (request == null)
            throw new ArgumentNullException(nameof(request));
        
        if (_requests.Any(r => r.PlayerId == request.PlayerId))
            throw new PlayerAlreadyInQueueException(request.PlayerId);
        
        if (request.GameMode != GameMode)
            throw new InvalidMatchmakingRequestException(
                $"Game mode mismatch. Queue mode: {GameMode}, Request mode: {request.GameMode}");
        
        _requests.Add(request);
    }
    
    /// <summary>
    /// 큐에서 플레이어 제거
    /// </summary>
    /// <param name="playerId">플레이어 ID</param>
    /// <exception cref="PlayerNotInQueueException">플레이어가 큐에 없는 경우</exception>
    public void Cancel(Guid playerId)
    {
        var request = _requests.FirstOrDefault(r => r.PlayerId == playerId);
        if (request == null)
            throw new PlayerNotInQueueException(playerId);
        
        _requests.Remove(request);
    }
    
    /// <summary>
    /// 플레이어 ID로 큐에서 요청 찾기
    /// </summary>
    /// <param name="playerId">플레이어 ID</param>
    /// <returns>플레이어 요청 또는 null</returns>
    public PlayerMatchRequest? FindByPlayerId(Guid playerId)
    {
        return _requests.FirstOrDefault(r => r.PlayerId == playerId);
    }
}

