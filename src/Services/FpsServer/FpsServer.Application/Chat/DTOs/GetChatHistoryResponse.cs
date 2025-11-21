namespace FpsServer.Application.Chat.DTOs;

/// <summary>
/// 채팅 히스토리 조회 응답 DTO
/// </summary>
public record GetChatHistoryResponse
{
    /// <summary>
    /// 채팅방 ID
    /// </summary>
    public required string RoomId { get; init; }
    
    /// <summary>
    /// 메시지 목록
    /// </summary>
    public required IReadOnlyList<ChatMessageDto> Messages { get; init; }
    
    /// <summary>
    /// 전체 메시지 수
    /// </summary>
    public int TotalCount { get; init; }
}

