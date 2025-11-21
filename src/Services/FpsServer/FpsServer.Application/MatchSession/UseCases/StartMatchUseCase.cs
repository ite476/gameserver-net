using FpsServer.Application.MatchSession.DTOs;
using FpsServer.Application.MatchSession.Mappers;
using FpsServer.Application.MatchSession.Ports;
using FpsServer.Domain.Matchmaking;
using DomainMatchSession = FpsServer.Domain.MatchSession.MatchSession;
using FpsServer.Domain.MatchSession.Exceptions;

namespace FpsServer.Application.MatchSession.UseCases;

/// <summary>
/// 매치 시작 유스케이스
/// </summary>
public class StartMatchUseCase
{
    private readonly IMatchSessionRepository _repository;
    
    /// <summary>
    /// 생성자
    /// </summary>
    /// <param name="repository">매치 세션 저장소</param>
    public StartMatchUseCase(IMatchSessionRepository repository)
    {
        _repository = repository ?? throw new ArgumentNullException(nameof(repository));
    }
    
    /// <summary>
    /// 매치 시작
    /// </summary>
    /// <param name="request">매치 시작 요청</param>
    /// <param name="gameMode">게임 모드</param>
    /// <param name="cancellationToken">취소 토큰</param>
    /// <returns>매치 시작 응답</returns>
    /// <exception cref="MatchSessionNotFoundException">매치 세션을 찾을 수 없는 경우</exception>
    /// <exception cref="InvalidMatchSessionStateException">유효하지 않은 상태 전이인 경우</exception>
    public async Task<StartMatchResponse> ExecuteAsync(
        StartMatchRequest request,
        MatchmakingMode gameMode,
        CancellationToken cancellationToken = default)
    {
        // 1. 기존 세션 조회 (매치 ID로)
        var existingSession = await _repository.FindByMatchIdAsync(request.MatchId, cancellationToken);
        
        DomainMatchSession session;
        if (existingSession != null)
        {
            // 기존 세션이 있으면 재사용
            session = existingSession;
        }
        else
        {
            // 2. 새 세션 생성
            session = MatchSessionMapper.ToDomain(request, gameMode);
            await _repository.CreateAsync(session, cancellationToken);
        }
        
        // 3. 게임 시작 (상태 전이: Matched → InProgress)
        session.Start();
        
        // 4. 세션 업데이트
        await _repository.UpdateAsync(session, cancellationToken);
        
        // 5. 응답 반환
        return MatchSessionMapper.ToStartResponse(session);
    }
}

