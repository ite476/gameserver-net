using FpsServer.Domain.Chat;

namespace FpsServer.Application.Chat.Ports;

/// <summary>
/// 채팅 메시지 알림 Port 인터페이스
/// Infrastructure 레이어에서 구현됩니다 (예: SignalR, WebSocket 등).
/// </summary>
public interface IChatNotifier
{
    /// <summary>
    /// 메시지 전송 알림
    /// </summary>
    /// <param name="roomId">채팅방 ID</param>
    /// <param name="message">전송된 메시지</param>
    /// <param name="cancellationToken">취소 토큰</param>
    Task NotifyMessageSentAsync(string roomId, ChatMessage message, CancellationToken cancellationToken = default);
}

