namespace FpsServer.Domain.Matchmaking.Exceptions;

/// <summary>
/// 잘못된 매칭 요청인 경우 발생하는 예외
/// </summary>
public class InvalidMatchmakingRequestException : MatchmakingException
{
    public InvalidMatchmakingRequestException(string message) : base(message) { }
    
    public InvalidMatchmakingRequestException(string message, Exception innerException) 
        : base(message, innerException) { }
}

