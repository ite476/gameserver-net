using FpsServer.Application.Chat.DTOs;
using FpsServer.Domain.Chat;

namespace FpsServer.Application.Chat.Mappers;

/// <summary>
/// 채팅 DTO ↔ Domain 변환 Mapper
/// </summary>
public static class ChatMapper
{
    /// <summary>
    /// SendMessageRequest → ChatUser 변환
    /// </summary>
    public static ChatUser ToChatUser(SendMessageRequest request)
    {
        return new ChatUser(request.UserId, request.Nickname);
    }
    
    /// <summary>
    /// ChatMessage → ChatMessageDto 변환
    /// </summary>
    public static ChatMessageDto ToDto(ChatMessage message)
    {
        return new ChatMessageDto
        {
            MessageId = message.MessageId,
            UserId = message.Sender.UserId,
            Nickname = message.Sender.Nickname,
            Content = message.Content,
            SentAt = message.SentAt
        };
    }
    
    /// <summary>
    /// ChatMessage 목록 → ChatMessageDto 목록 변환
    /// </summary>
    public static IReadOnlyList<ChatMessageDto> ToDtoList(IEnumerable<ChatMessage> messages)
    {
        return messages.Select(ToDto).ToList().AsReadOnly();
    }
    
    /// <summary>
    /// ChatMessage → SendMessageResponse 변환
    /// </summary>
    public static SendMessageResponse ToSendMessageResponse(ChatMessage message)
    {
        return new SendMessageResponse
        {
            MessageId = message.MessageId,
            SentAt = message.SentAt
        };
    }
}

