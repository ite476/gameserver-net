using FpsServer.Application.Matchmaking.DTOs;
using FpsServer.Application.Matchmaking.Mappers;
using FpsServer.Application.Matchmaking.Ports;
using FpsServer.Domain.Matchmaking;
using FpsServer.Domain.Matchmaking.Exceptions;

namespace FpsServer.Application.Matchmaking.UseCases;

/// <summary>
/// 매치메이킹 큐 진입 유스케이스
/// </summary>
public class JoinMatchmakingQueueUseCase
{
    private readonly IMatchmakingRepository _repository;
    private readonly IMatchmakingNotifier _notifier;
    private readonly MatchmakingDomainService _domainService;
    
    /// <summary>
    /// 생성자
    /// </summary>
    /// <param name="repository">매치메이킹 저장소</param>
    /// <param name="notifier">매칭 알림 서비스</param>
    /// <param name="domainService">매치메이킹 도메인 서비스</param>
    public JoinMatchmakingQueueUseCase(
        IMatchmakingRepository repository,
        IMatchmakingNotifier notifier,
        MatchmakingDomainService domainService)
    {
        _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        _notifier = notifier ?? throw new ArgumentNullException(nameof(notifier));
        _domainService = domainService ?? throw new ArgumentNullException(nameof(domainService));
    }
    
    /// <summary>
    /// 매치메이킹 큐에 진입
    /// </summary>
    /// <param name="request">큐 진입 요청</param>
    /// <param name="cancellationToken">취소 토큰</param>
    /// <returns>큐 진입 응답</returns>
    /// <exception cref="PlayerAlreadyInQueueException">플레이어가 이미 큐에 있는 경우</exception>
    /// <exception cref="InvalidMatchmakingRequestException">유효하지 않은 요청인 경우</exception>
    public async Task<JoinMatchmakingResponse> ExecuteAsync(
        JoinMatchmakingRequest request,
        CancellationToken cancellationToken = default)
    {
        // 1. 큐 가져오기 또는 생성
        var queue = await _repository.GetOrCreateQueueAsync(request.GameMode, cancellationToken);
        
        // 2. DTO를 Domain 모델로 변환
        var playerRequest = MatchmakingMapper.ToDomain(request);
        
        // 3. Domain의 큐에 추가 (비즈니스 규칙 검증 포함)
        queue.Enqueue(playerRequest);
        
        // 4. Repository에 저장
        await _repository.EnqueueAsync(queue, playerRequest, cancellationToken);
        
        // 5. 매칭 시도
        var match = _domainService.TryMatch(queue);
        
        if (match != null)
        {
            // 6. 매칭 성공 시 큐에서 제거
            var requestIds = match.Players.Select(p => p.RequestId).ToList();
            await _repository.DequeueAsync(queue, requestIds, cancellationToken);
            
            // 7. 매칭 성공 알림 전송
            await _notifier.NotifyMatchFoundAsync(match, cancellationToken);
            
            // 8. 매칭 성공 응답 반환
            return new JoinMatchmakingResponse
            {
                RequestId = playerRequest.RequestId,
                Status = "Matched"
            };
        }
        
        // 9. 큐 진입 성공 응답 반환
        return MatchmakingMapper.ToResponse(playerRequest);
    }
}

