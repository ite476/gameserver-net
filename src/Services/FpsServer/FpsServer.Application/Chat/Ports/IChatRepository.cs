using FpsServer.Domain.Chat;

namespace FpsServer.Application.Chat.Ports;

/// <summary>
/// 채팅 저장소 Port 인터페이스
/// Infrastructure 레이어에서 구현됩니다.
/// </summary>
public interface IChatRepository
{
    /// <summary>
    /// 채팅방을 가져오거나 생성합니다.
    /// </summary>
    /// <param name="roomId">채팅방 ID</param>
    /// <param name="roomName">채팅방 이름</param>
    /// <param name="cancellationToken">취소 토큰</param>
    /// <returns>채팅방</returns>
    Task<ChatRoom> GetOrCreateRoomAsync(string roomId, string roomName, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// 채팅방을 조회합니다.
    /// </summary>
    /// <param name="roomId">채팅방 ID</param>
    /// <param name="cancellationToken">취소 토큰</param>
    /// <returns>채팅방 또는 null</returns>
    Task<ChatRoom?> FindRoomAsync(string roomId, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// 메시지를 저장합니다.
    /// </summary>
    /// <param name="room">채팅방</param>
    /// <param name="message">저장할 메시지</param>
    /// <param name="cancellationToken">취소 토큰</param>
    Task SaveMessageAsync(ChatRoom room, ChatMessage message, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// 채팅방의 메시지 목록을 조회합니다 (페이지네이션).
    /// </summary>
    /// <param name="roomId">채팅방 ID</param>
    /// <param name="skip">건너뛸 메시지 수</param>
    /// <param name="take">가져올 메시지 수</param>
    /// <param name="cancellationToken">취소 토큰</param>
    /// <returns>메시지 목록</returns>
    Task<IReadOnlyList<ChatMessage>> GetMessagesAsync(
        string roomId, 
        int skip = 0, 
        int take = 50, 
        CancellationToken cancellationToken = default);
}

