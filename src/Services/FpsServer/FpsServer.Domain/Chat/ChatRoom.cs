using FpsServer.Domain.Chat.Exceptions;

namespace FpsServer.Domain.Chat;

/// <summary>
/// 채팅방 Aggregate Root
/// 채팅방 상태 관리 및 메시지 추가 등의 비즈니스 규칙을 보장합니다.
/// </summary>
public class ChatRoom
{
    private readonly List<ChatMessage> _messages = new();
    private readonly HashSet<Guid> _connectedUsers = new();
    
    /// <summary>
    /// 채팅방 ID
    /// </summary>
    public string RoomId { get; private set; }
    
    /// <summary>
    /// 채팅방 이름
    /// </summary>
    public string Name { get; private set; }
    
    /// <summary>
    /// 채팅방 생성 시간
    /// </summary>
    public DateTimeOffset CreatedAt { get; private set; }
    
    /// <summary>
    /// 채팅방에 등록된 메시지 목록 (읽기 전용)
    /// </summary>
    public IReadOnlyList<ChatMessage> Messages => _messages.AsReadOnly();
    
    /// <summary>
    /// 채팅방에 연결된 사용자 ID 목록 (읽기 전용)
    /// </summary>
    public IReadOnlySet<Guid> ConnectedUserIds => _connectedUsers.ToHashSet();
    
    // EF Core용 private 생성자
    private ChatRoom()
    {
        RoomId = string.Empty;
        Name = string.Empty;
    }
    
    /// <summary>
    /// 채팅방 생성
    /// </summary>
    /// <param name="roomId">채팅방 ID</param>
    /// <param name="name">채팅방 이름</param>
    /// <exception cref="ArgumentNullException">필수 파라미터가 null인 경우</exception>
    public ChatRoom(string roomId, string name)
    {
        if (string.IsNullOrWhiteSpace(roomId))
            throw new ArgumentNullException(nameof(roomId), "RoomId cannot be null or empty.");
        
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentNullException(nameof(name), "Name cannot be null or empty.");
        
        RoomId = roomId;
        Name = name;
        CreatedAt = DateTimeOffset.UtcNow;
    }
    
    /// <summary>
    /// 채팅방에 메시지 추가
    /// </summary>
    /// <param name="message">추가할 메시지</param>
    /// <exception cref="ArgumentNullException">메시지가 null인 경우</exception>
    /// <exception cref="InvalidOperationException">메시지의 RoomId가 채팅방 ID와 일치하지 않는 경우</exception>
    public void AddMessage(ChatMessage message)
    {
        if (message == null)
            throw new ArgumentNullException(nameof(message));
        
        if (message.RoomId != RoomId)
            throw new InvalidOperationException(
                $"Message RoomId ({message.RoomId}) does not match ChatRoom RoomId ({RoomId}).");
        
        _messages.Add(message);
        
        // MVP 단계에서는 메모리 기반이므로 메시지 수 제한 없음
        // 향후 확장 시 최근 N개만 유지하는 로직 추가 가능
    }
    
    /// <summary>
    /// 사용자를 채팅방에 연결
    /// </summary>
    /// <param name="userId">사용자 ID</param>
    public void ConnectUser(Guid userId)
    {
        _connectedUsers.Add(userId);
    }
    
    /// <summary>
    /// 사용자를 채팅방에서 연결 해제
    /// </summary>
    /// <param name="userId">사용자 ID</param>
    public void DisconnectUser(Guid userId)
    {
        _connectedUsers.Remove(userId);
    }
    
    /// <summary>
    /// 사용자가 채팅방에 연결되어 있는지 확인
    /// </summary>
    /// <param name="userId">사용자 ID</param>
    /// <returns>연결되어 있으면 true, 아니면 false</returns>
    public bool IsUserConnected(Guid userId)
    {
        return _connectedUsers.Contains(userId);
    }
}

