using FpsServer.Application.Chat.DTOs;
using FpsServer.Application.Chat.Mappers;
using FpsServer.Application.Chat.Ports;
using FpsServer.Domain.Chat;
using FpsServer.Domain.Chat.Exceptions;

namespace FpsServer.Application.Chat.UseCases;

/// <summary>
/// 메시지 전송 유스케이스
/// </summary>
public class SendMessageUseCase
{
    private readonly IChatRepository _repository;
    private readonly ChatDomainService _domainService;
    
    /// <summary>
    /// 생성자
    /// </summary>
    /// <param name="repository">채팅 저장소</param>
    /// <param name="domainService">채팅 도메인 서비스</param>
    public SendMessageUseCase(
        IChatRepository repository,
        ChatDomainService domainService)
    {
        _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        _domainService = domainService ?? throw new ArgumentNullException(nameof(domainService));
    }
    
    /// <summary>
    /// 메시지 전송
    /// </summary>
    /// <param name="request">메시지 전송 요청</param>
    /// <param name="cancellationToken">취소 토큰</param>
    /// <returns>메시지 전송 응답</returns>
    /// <exception cref="InvalidChatMessageException">메시지가 유효하지 않은 경우</exception>
    /// <exception cref="ChatRoomNotFoundException">채팅방을 찾을 수 없는 경우</exception>
    public async Task<SendMessageResponse> ExecuteAsync(
        SendMessageRequest request,
        CancellationToken cancellationToken = default)
    {
        // 1. 채팅방 가져오기 또는 생성
        var room = await _repository.GetOrCreateRoomAsync(
            request.RoomId, 
            $"Room-{request.RoomId}", 
            cancellationToken);
        
        // 2. 도메인 서비스를 통한 메시지 검증 및 필터링
        var validatedContent = _domainService.ValidateAndFilterMessage(request.Content);
        
        // 3. ChatUser 값 객체 생성
        var sender = ChatMapper.ToChatUser(request);
        
        // 4. ChatMessage 엔티티 생성
        var message = new ChatMessage(room.RoomId, sender, validatedContent);
        
        // 5. 채팅방에 메시지 추가 (비즈니스 규칙 검증 포함)
        room.AddMessage(message);
        
        // 6. Repository에 저장
        await _repository.SaveMessageAsync(room, message, cancellationToken);
        
        // 7. 응답 반환
        return ChatMapper.ToSendMessageResponse(message);
    }
}

