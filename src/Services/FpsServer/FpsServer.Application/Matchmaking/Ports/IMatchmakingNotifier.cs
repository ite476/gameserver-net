using FpsServer.Domain.Matchmaking;

namespace FpsServer.Application.Matchmaking.Ports;

/// <summary>
/// 매칭 성공 알림 Port 인터페이스
/// Infrastructure 레이어에서 구현됩니다 (예: SignalR, WebSocket 등).
/// </summary>
public interface IMatchmakingNotifier
{
    /// <summary>
    /// 매칭 성공 알림 전송
    /// </summary>
    /// <param name="match">매칭 결과</param>
    /// <param name="cancellationToken">취소 토큰</param>
    Task NotifyMatchFoundAsync(Match match, CancellationToken cancellationToken = default);
}

