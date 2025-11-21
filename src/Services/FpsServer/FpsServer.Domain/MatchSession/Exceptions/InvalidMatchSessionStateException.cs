using FpsServer.Domain.MatchSession;

namespace FpsServer.Domain.MatchSession.Exceptions;

/// <summary>
/// 매치 세션 상태 전이가 유효하지 않은 경우 발생하는 예외
/// </summary>
public class InvalidMatchSessionStateException : MatchSessionException
{
    public MatchStatus CurrentStatus { get; }
    public MatchStatus AttemptedStatus { get; }
    
    public InvalidMatchSessionStateException(
        MatchStatus currentStatus, 
        MatchStatus attemptedStatus)
        : base($"Invalid state transition from {currentStatus} to {attemptedStatus}")
    {
        CurrentStatus = currentStatus;
        AttemptedStatus = attemptedStatus;
    }
}

