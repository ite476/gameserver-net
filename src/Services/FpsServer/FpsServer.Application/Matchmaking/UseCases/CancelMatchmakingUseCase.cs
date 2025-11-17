using FpsServer.Application.Matchmaking.Ports;
using FpsServer.Domain.Matchmaking;
using FpsServer.Domain.Matchmaking.Exceptions;

namespace FpsServer.Application.Matchmaking.UseCases;

/// <summary>
/// 매치메이킹 큐 취소 유스케이스
/// </summary>
public class CancelMatchmakingUseCase
{
    private readonly IMatchmakingRepository _repository;
    
    /// <summary>
    /// 생성자
    /// </summary>
    /// <param name="repository">매치메이킹 저장소</param>
    public CancelMatchmakingUseCase(IMatchmakingRepository repository)
    {
        _repository = repository ?? throw new ArgumentNullException(nameof(repository));
    }
    
    /// <summary>
    /// 매치메이킹 큐에서 취소
    /// </summary>
    /// <param name="gameMode">게임 모드</param>
    /// <param name="playerId">플레이어 ID</param>
    /// <param name="cancellationToken">취소 토큰</param>
    /// <exception cref="PlayerNotInQueueException">플레이어가 큐에 없는 경우</exception>
    public async Task ExecuteAsync(
        MatchmakingMode gameMode,
        Guid playerId,
        CancellationToken cancellationToken = default)
    {
        // 1. 큐 가져오기
        var queue = await _repository.GetOrCreateQueueAsync(gameMode, cancellationToken);
        
        // 2. Domain의 큐에서 취소 (비즈니스 규칙 검증 포함)
        queue.Cancel(playerId);
        
        // 3. Repository에 저장
        await _repository.CancelAsync(queue, playerId, cancellationToken);
    }
}

