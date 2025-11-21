using FpsServer.Domain.Chat.Exceptions;

namespace FpsServer.Domain.Chat;

/// <summary>
/// 채팅 도메인 서비스
/// 메시지 검증, 금지어 필터 등의 비즈니스 로직을 수행합니다.
/// </summary>
public class ChatDomainService
{
    private const int MAX_MESSAGE_LENGTH = 500; // 최대 메시지 길이
    private const int MIN_MESSAGE_LENGTH = 1; // 최소 메시지 길이
    
    // MVP 단계에서는 간단한 금지어 목록 사용
    // 향후 확장 시 외부 설정 파일 또는 DB에서 로드 가능
    private static readonly HashSet<string> _bannedWords = new(StringComparer.OrdinalIgnoreCase)
    {
        // 예시 금지어 (실제 운영 시에는 더 체계적인 관리 필요)
        "spam", "test"
    };
    
    /// <summary>
    /// 메시지 내용을 검증하고 필터링합니다.
    /// </summary>
    /// <param name="content">원본 메시지 내용</param>
    /// <returns>검증 및 필터링된 메시지 내용</returns>
    /// <exception cref="ArgumentNullException">메시지가 null인 경우</exception>
    /// <exception cref="InvalidChatMessageException">메시지가 유효하지 않은 경우</exception>
    public string ValidateAndFilterMessage(string content)
    {
        if (content == null)
            throw new ArgumentNullException(nameof(content));
        
        // 공백 제거 후 검증
        var trimmedContent = content.Trim();
        
        // 길이 검증
        if (trimmedContent.Length < MIN_MESSAGE_LENGTH)
            throw new InvalidChatMessageException("Message cannot be empty.");
        
        if (trimmedContent.Length > MAX_MESSAGE_LENGTH)
            throw new InvalidChatMessageException(
                $"Message length exceeds maximum allowed length ({MAX_MESSAGE_LENGTH} characters).");
        
        // 금지어 필터링
        var filteredContent = FilterBannedWords(trimmedContent);
        
        return filteredContent;
    }
    
    /// <summary>
    /// 메시지에서 금지어를 필터링합니다.
    /// MVP 단계에서는 단순 문자열 치환을 사용합니다.
    /// </summary>
    /// <param name="content">원본 메시지 내용</param>
    /// <returns>금지어가 필터링된 메시지 내용</returns>
    private string FilterBannedWords(string content)
    {
        var filtered = content;
        
        foreach (var bannedWord in _bannedWords)
        {
            // 대소문자 구분 없이 금지어를 ***로 치환
            filtered = System.Text.RegularExpressions.Regex.Replace(
                filtered, 
                bannedWord, 
                "***", 
                System.Text.RegularExpressions.RegexOptions.IgnoreCase);
        }
        
        return filtered;
    }
}

