namespace FpsServer.Domain.Matchmaking.Exceptions;

/// <summary>
/// 매치메이킹 관련 예외의 베이스 클래스
/// </summary>
public class MatchmakingException : Exception
{
    public MatchmakingException(string message) : base(message) { }
    
    public MatchmakingException(string message, Exception innerException) 
        : base(message, innerException) { }
}

