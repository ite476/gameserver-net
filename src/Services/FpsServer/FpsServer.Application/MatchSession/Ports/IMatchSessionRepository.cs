using DomainMatchSession = FpsServer.Domain.MatchSession.MatchSession;

namespace FpsServer.Application.MatchSession.Ports;

/// <summary>
/// 매치 세션 저장소 Port 인터페이스
/// Infrastructure 레이어에서 구현됩니다.
/// </summary>
public interface IMatchSessionRepository
{
    /// <summary>
    /// 매치 세션 생성
    /// </summary>
    /// <param name="session">매치 세션</param>
    /// <param name="cancellationToken">취소 토큰</param>
    Task CreateAsync(DomainMatchSession session, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// 세션 ID로 매치 세션 조회
    /// </summary>
    /// <param name="sessionId">세션 ID</param>
    /// <param name="cancellationToken">취소 토큰</param>
    /// <returns>매치 세션, 없으면 null</returns>
    Task<DomainMatchSession?> FindByIdAsync(Guid sessionId, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// 매치 ID로 매치 세션 조회
    /// </summary>
    /// <param name="matchId">매치 ID</param>
    /// <param name="cancellationToken">취소 토큰</param>
    /// <returns>매치 세션, 없으면 null</returns>
    Task<DomainMatchSession?> FindByMatchIdAsync(Guid matchId, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// 매치 세션 업데이트
    /// </summary>
    /// <param name="session">매치 세션</param>
    /// <param name="cancellationToken">취소 토큰</param>
    Task UpdateAsync(DomainMatchSession session, CancellationToken cancellationToken = default);
}

