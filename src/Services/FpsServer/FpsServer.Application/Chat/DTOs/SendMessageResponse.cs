namespace FpsServer.Application.Chat.DTOs;

/// <summary>
/// 메시지 전송 응답 DTO
/// </summary>
public record SendMessageResponse
{
    /// <summary>
    /// 메시지 ID
    /// </summary>
    public required Guid MessageId { get; init; }
    
    /// <summary>
    /// 메시지 전송 시간
    /// </summary>
    public required DateTimeOffset SentAt { get; init; }
}

