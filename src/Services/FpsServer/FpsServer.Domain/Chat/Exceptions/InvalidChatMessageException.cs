namespace FpsServer.Domain.Chat.Exceptions;

/// <summary>
/// 유효하지 않은 채팅 메시지 예외
/// 메시지 길이 제한 초과, 금지어 포함 등의 경우 발생합니다.
/// </summary>
public class InvalidChatMessageException : ChatException
{
    public InvalidChatMessageException(string message) : base(message) { }
    
    public InvalidChatMessageException(string message, Exception innerException) 
        : base(message, innerException) { }
}

