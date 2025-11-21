namespace FpsServer.Domain.MatchSession.Exceptions;

/// <summary>
/// 매치 세션을 찾을 수 없는 경우 발생하는 예외
/// </summary>
public class MatchSessionNotFoundException : MatchSessionException
{
    public Guid MatchId { get; }
    
    public MatchSessionNotFoundException(Guid matchId)
        : base($"Match session not found for MatchId: {matchId}")
    {
        MatchId = matchId;
    }
}

