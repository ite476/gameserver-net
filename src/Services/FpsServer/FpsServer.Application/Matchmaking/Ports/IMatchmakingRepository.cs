using FpsServer.Domain.Matchmaking;

namespace FpsServer.Application.Matchmaking.Ports;

/// <summary>
/// 매치메이킹 큐 저장소 Port 인터페이스
/// Infrastructure 레이어에서 구현됩니다.
/// </summary>
public interface IMatchmakingRepository
{
    /// <summary>
    /// 큐에 플레이어 요청 추가
    /// </summary>
    /// <param name="queue">매치메이킹 큐</param>
    /// <param name="request">플레이어 매칭 요청</param>
    /// <param name="cancellationToken">취소 토큰</param>
    Task EnqueueAsync(MatchmakingQueue queue, PlayerMatchRequest request, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// 큐에서 플레이어 요청 제거 (매칭 성공 시)
    /// </summary>
    /// <param name="queue">매치메이킹 큐</param>
    /// <param name="requestIds">제거할 요청 ID 목록</param>
    /// <param name="cancellationToken">취소 토큰</param>
    Task DequeueAsync(MatchmakingQueue queue, IEnumerable<Guid> requestIds, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// 플레이어 ID로 큐에서 요청 찾기
    /// </summary>
    /// <param name="gameMode">게임 모드</param>
    /// <param name="playerId">플레이어 ID</param>
    /// <param name="cancellationToken">취소 토큰</param>
    /// <returns>플레이어 요청, 없으면 null</returns>
    Task<PlayerMatchRequest?> FindByPlayerIdAsync(MatchmakingMode gameMode, Guid playerId, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// 큐에서 플레이어 요청 취소
    /// </summary>
    /// <param name="queue">매치메이킹 큐</param>
    /// <param name="playerId">플레이어 ID</param>
    /// <param name="cancellationToken">취소 토큰</param>
    Task CancelAsync(MatchmakingQueue queue, Guid playerId, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// 게임 모드별 큐 조회 (없으면 생성)
    /// </summary>
    /// <param name="gameMode">게임 모드</param>
    /// <param name="cancellationToken">취소 토큰</param>
    /// <returns>매치메이킹 큐</returns>
    Task<MatchmakingQueue> GetOrCreateQueueAsync(MatchmakingMode gameMode, CancellationToken cancellationToken = default);
}

