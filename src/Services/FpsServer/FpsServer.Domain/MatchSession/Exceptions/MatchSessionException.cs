namespace FpsServer.Domain.MatchSession.Exceptions;

/// <summary>
/// 매치 세션 관련 예외의 베이스 클래스
/// </summary>
public class MatchSessionException : Exception
{
    public MatchSessionException(string message) : base(message) { }
    
    public MatchSessionException(string message, Exception innerException) 
        : base(message, innerException) { }
}

