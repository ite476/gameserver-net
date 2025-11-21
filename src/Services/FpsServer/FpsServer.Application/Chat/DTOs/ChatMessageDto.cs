namespace FpsServer.Application.Chat.DTOs;

/// <summary>
/// 채팅 메시지 DTO
/// </summary>
public record ChatMessageDto
{
    /// <summary>
    /// 메시지 ID
    /// </summary>
    public required Guid MessageId { get; init; }
    
    /// <summary>
    /// 사용자 ID
    /// </summary>
    public required Guid UserId { get; init; }
    
    /// <summary>
    /// 사용자 닉네임
    /// </summary>
    public required string Nickname { get; init; }
    
    /// <summary>
    /// 메시지 내용
    /// </summary>
    public required string Content { get; init; }
    
    /// <summary>
    /// 메시지 전송 시간
    /// </summary>
    public required DateTimeOffset SentAt { get; init; }
}

