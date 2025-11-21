using FpsServer.Domain.MMR;
using System.Threading;
using System.Threading.Tasks;

namespace FpsServer.Application.MMR.Ports;

/// <summary>
/// 플레이어 MMR 저장소 Port 인터페이스
/// Infrastructure 레이어에서 구현됩니다 (예: EF Core).
/// </summary>
public interface IPlayerMMRRepository
{
    /// <summary>
    /// 플레이어 ID로 MMR 조회
    /// </summary>
    /// <param name="playerId">플레이어 ID</param>
    /// <param name="cancellationToken">취소 토큰</param>
    /// <returns>조회된 플레이어 MMR 또는 null</returns>
    Task<PlayerMMR?> FindByPlayerIdAsync(Guid playerId, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// 여러 플레이어 ID로 MMR 목록 조회
    /// </summary>
    /// <param name="playerIds">플레이어 ID 목록</param>
    /// <param name="cancellationToken">취소 토큰</param>
    /// <returns>조회된 플레이어 MMR 목록</returns>
    Task<IReadOnlyList<PlayerMMR>> FindMultipleByPlayerIdsAsync(
        IReadOnlyList<Guid> playerIds,
        CancellationToken cancellationToken = default);
    
    /// <summary>
    /// 플레이어 MMR 생성 또는 업데이트
    /// </summary>
    /// <param name="playerMMR">플레이어 MMR</param>
    /// <param name="cancellationToken">취소 토큰</param>
    Task SaveAsync(PlayerMMR playerMMR, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// 여러 플레이어 MMR 일괄 저장
    /// </summary>
    /// <param name="playerMMRs">플레이어 MMR 목록</param>
    /// <param name="cancellationToken">취소 토큰</param>
    Task SaveMultipleAsync(
        IReadOnlyList<PlayerMMR> playerMMRs,
        CancellationToken cancellationToken = default);
}

