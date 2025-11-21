namespace FpsServer.Application.Chat.DTOs;

/// <summary>
/// 메시지 전송 요청 DTO
/// </summary>
public record SendMessageRequest
{
    /// <summary>
    /// 채팅방 ID
    /// </summary>
    public required string RoomId { get; init; }
    
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
}

