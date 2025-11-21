namespace FpsServer.Domain.Chat.Exceptions;

/// <summary>
/// 채팅 도메인 예외 기본 클래스
/// </summary>
public abstract class ChatException : Exception
{
    protected ChatException(string message) : base(message) { }
    
    protected ChatException(string message, Exception innerException) 
        : base(message, innerException) { }
}

