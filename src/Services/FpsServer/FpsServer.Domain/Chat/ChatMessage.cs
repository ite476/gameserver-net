namespace FpsServer.Domain.Chat;

/// <summary>
/// 채팅 메시지 엔티티
/// 메시지 내용 및 메타데이터를 관리합니다.
/// </summary>
public class ChatMessage
{
    /// <summary>
    /// 메시지 ID
    /// </summary>
    public Guid MessageId { get; private set; }
    
    /// <summary>
    /// 채팅방 ID
    /// </summary>
    public string RoomId { get; private set; }
    
    /// <summary>
    /// 메시지 작성자
    /// </summary>
    public ChatUser Sender { get; private set; }
    
    /// <summary>
    /// 메시지 내용
    /// </summary>
    public string Content { get; private set; }
    
    /// <summary>
    /// 메시지 전송 시간
    /// </summary>
    public DateTimeOffset SentAt { get; private set; }
    
    // EF Core용 private 생성자
    private ChatMessage() 
    {
        RoomId = string.Empty;
        Sender = null!;
        Content = string.Empty;
    }
    
    /// <summary>
    /// 채팅 메시지 생성
    /// </summary>
    /// <param name="roomId">채팅방 ID</param>
    /// <param name="sender">메시지 작성자</param>
    /// <param name="content">메시지 내용</param>
    /// <exception cref="ArgumentNullException">필수 파라미터가 null인 경우</exception>
    public ChatMessage(string roomId, ChatUser sender, string content)
    {
        if (string.IsNullOrWhiteSpace(roomId))
            throw new ArgumentNullException(nameof(roomId), "RoomId cannot be null or empty.");
        
        if (sender == null)
            throw new ArgumentNullException(nameof(sender));
        
        if (string.IsNullOrWhiteSpace(content))
            throw new ArgumentNullException(nameof(content), "Content cannot be null or empty.");
        
        MessageId = Guid.NewGuid();
        RoomId = roomId;
        Sender = sender;
        Content = content;
        SentAt = DateTimeOffset.UtcNow;
    }
}

