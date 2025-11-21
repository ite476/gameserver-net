using Microsoft.AspNetCore.SignalR;
using FpsServer.Application.Chat.Ports;
using FpsServer.Domain.Chat;

namespace FpsServer.Infrastructure.Chat;

/// <summary>
/// 채팅 SignalR Hub
/// 클라이언트 연결 관리 및 메시지 브로드캐스트에 사용됩니다.
/// </summary>
public class ChatHub : Hub
{
    /// <summary>
    /// 채팅방에 가입
    /// </summary>
    /// <param name="roomId">채팅방 ID</param>
    public async Task JoinRoom(string roomId)
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, roomId);
    }
    
    /// <summary>
    /// 채팅방에서 나가기
    /// </summary>
    /// <param name="roomId">채팅방 ID</param>
    public async Task LeaveRoom(string roomId)
    {
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, roomId);
    }
}

/// <summary>
/// SignalR 기반 채팅 메시지 알림 구현
/// SignalR Hub를 통해 클라이언트에 실시간 메시지를 브로드캐스트합니다.
/// </summary>
public class SignalRChatNotifier : IChatNotifier
{
    private readonly IHubContext<ChatHub> _hubContext;
    
    /// <summary>
    /// 생성자
    /// </summary>
    /// <param name="hubContext">SignalR Hub Context</param>
    public SignalRChatNotifier(IHubContext<ChatHub> hubContext)
    {
        _hubContext = hubContext ?? throw new ArgumentNullException(nameof(hubContext));
    }
    
    /// <summary>
    /// 메시지 전송 알림
    /// </summary>
    public async Task NotifyMessageSentAsync(string roomId, ChatMessage message, CancellationToken cancellationToken = default)
    {
        // 채팅방 그룹에 메시지 브로드캐스트
        await _hubContext.Clients.Group(roomId)
            .SendAsync("MessageReceived", new
            {
                MessageId = message.MessageId,
                RoomId = message.RoomId,
                UserId = message.Sender.UserId,
                Nickname = message.Sender.Nickname,
                Content = message.Content,
                SentAt = message.SentAt
            }, cancellationToken);
    }
}

