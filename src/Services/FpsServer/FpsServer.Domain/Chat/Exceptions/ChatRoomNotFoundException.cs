namespace FpsServer.Domain.Chat.Exceptions;

/// <summary>
/// 채팅방을 찾을 수 없을 때 발생하는 예외
/// </summary>
public class ChatRoomNotFoundException : ChatException
{
    public string RoomId { get; }
    
    public ChatRoomNotFoundException(string roomId) 
        : base($"Chat room not found: {roomId}")
    {
        RoomId = roomId;
    }
}

