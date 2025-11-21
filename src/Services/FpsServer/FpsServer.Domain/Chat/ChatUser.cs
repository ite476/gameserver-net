namespace FpsServer.Domain.Chat;

/// <summary>
/// 채팅 사용자 값 객체
/// 사용자 정보를 불변 객체로 표현합니다.
/// </summary>
public record ChatUser
{
    /// <summary>
    /// 사용자 ID
    /// </summary>
    public Guid UserId { get; init; }
    
    /// <summary>
    /// 사용자 닉네임
    /// </summary>
    public string Nickname { get; init; }
    
    /// <summary>
    /// 채팅 사용자 생성
    /// </summary>
    /// <param name="userId">사용자 ID</param>
    /// <param name="nickname">사용자 닉네임</param>
    /// <exception cref="ArgumentNullException">닉네임이 null이거나 비어있는 경우</exception>
    public ChatUser(Guid userId, string nickname)
    {
        if (string.IsNullOrWhiteSpace(nickname))
            throw new ArgumentNullException(nameof(nickname), "Nickname cannot be null or empty.");
        
        UserId = userId;
        Nickname = nickname;
    }
}

