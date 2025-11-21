using FpsServer.Application.MatchSession.DTOs;
using FpsServer.Application.MatchSession.Mappers;
using FpsServer.Application.MatchSession.Ports;
using DomainMatchSession = FpsServer.Domain.MatchSession.MatchSession;
using FpsServer.Domain.MatchSession.Exceptions;

namespace FpsServer.Application.MatchSession.UseCases;

/// <summary>
/// 매치 종료 유스케이스
/// </summary>
public class EndMatchUseCase
{
    private readonly IMatchSessionRepository _repository;
    
    /// <summary>
    /// 생성자
    /// </summary>
    /// <param name="repository">매치 세션 저장소</param>
    public EndMatchUseCase(IMatchSessionRepository repository)
    {
        _repository = repository ?? throw new ArgumentNullException(nameof(repository));
    }
    
    /// <summary>
    /// 매치 종료
    /// </summary>
    /// <param name="request">매치 종료 요청</param>
    /// <param name="cancellationToken">취소 토큰</param>
    /// <returns>매치 종료 응답</returns>
    /// <exception cref="MatchSessionNotFoundException">매치 세션을 찾을 수 없는 경우</exception>
    /// <exception cref="InvalidMatchSessionStateException">유효하지 않은 상태 전이인 경우</exception>
    public async Task<EndMatchResponse> ExecuteAsync(
        EndMatchRequest request,
        CancellationToken cancellationToken = default)
    {
        // 1. 세션 조회 (매치 ID로)
        var session = await _repository.FindByMatchIdAsync(request.MatchId, cancellationToken);
        if (session == null)
            throw new MatchSessionNotFoundException(request.MatchId);
        
        // 2. DTO를 Domain 모델로 변환
        var result = MatchSessionMapper.ToDomain(request);
        
        // 3. 게임 종료 (상태 전이: InProgress → Finished)
        session.End(result);
        
        // 4. 세션 업데이트
        await _repository.UpdateAsync(session, cancellationToken);
        
        // 5. 응답 반환
        return MatchSessionMapper.ToEndResponse(session);
    }
}

