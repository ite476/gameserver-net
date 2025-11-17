namespace FpsServer.Domain.Matchmaking.Exceptions;

/// <summary>
/// 플레이어가 이미 큐에 있는 경우 발생하는 예외
/// </summary>
public class PlayerAlreadyInQueueException : MatchmakingException
{
    /// <summary>
    /// 큐에 이미 있는 플레이어 ID
    /// </summary>
    public Guid PlayerId { get; }
    
    public PlayerAlreadyInQueueException(Guid playerId) 
        : base($"Player {playerId} is already in the queue")
    {
        PlayerId = playerId;
    }
}

