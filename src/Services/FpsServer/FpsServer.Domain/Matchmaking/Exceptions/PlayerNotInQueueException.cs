namespace FpsServer.Domain.Matchmaking.Exceptions;

/// <summary>
/// 플레이어가 큐에 없는 경우 발생하는 예외
/// </summary>
public class PlayerNotInQueueException : MatchmakingException
{
    /// <summary>
    /// 큐에 없는 플레이어 ID
    /// </summary>
    public Guid PlayerId { get; }
    
    public PlayerNotInQueueException(Guid playerId) 
        : base($"Player {playerId} is not in the queue")
    {
        PlayerId = playerId;
    }
}

