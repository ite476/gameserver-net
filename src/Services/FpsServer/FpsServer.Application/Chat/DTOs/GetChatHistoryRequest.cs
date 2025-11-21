namespace FpsServer.Application.Chat.DTOs;

/// <summary>
/// 채팅 히스토리 조회 요청 DTO
/// </summary>
public record GetChatHistoryRequest
{
    /// <summary>
    /// 채팅방 ID
    /// </summary>
    public required string RoomId { get; init; }
    
    /// <summary>
    /// 건너뛸 메시지 수 (페이지네이션)
    /// </summary>
    public int Skip { get; init; } = 0;
    
    /// <summary>
    /// 가져올 메시지 수 (페이지네이션)
    /// </summary>
    public int Take { get; init; } = 50;
}

