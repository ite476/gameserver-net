using FpsServer.Application.Chat.DTOs;
using FpsServer.Application.Chat.Mappers;
using FpsServer.Application.Chat.Ports;
using FpsServer.Domain.Chat.Exceptions;

namespace FpsServer.Application.Chat.UseCases;

/// <summary>
/// 채팅 히스토리 조회 유스케이스
/// </summary>
public class GetChatHistoryUseCase
{
    private readonly IChatRepository _repository;
    
    /// <summary>
    /// 생성자
    /// </summary>
    /// <param name="repository">채팅 저장소</param>
    public GetChatHistoryUseCase(IChatRepository repository)
    {
        _repository = repository ?? throw new ArgumentNullException(nameof(repository));
    }
    
    /// <summary>
    /// 채팅 히스토리 조회
    /// </summary>
    /// <param name="request">히스토리 조회 요청</param>
    /// <param name="cancellationToken">취소 토큰</param>
    /// <returns>채팅 히스토리 응답</returns>
    /// <exception cref="ChatRoomNotFoundException">채팅방을 찾을 수 없는 경우</exception>
    public async Task<GetChatHistoryResponse> ExecuteAsync(
        GetChatHistoryRequest request,
        CancellationToken cancellationToken = default)
    {
        // 1. 채팅방 존재 여부 확인
        var room = await _repository.FindRoomAsync(request.RoomId, cancellationToken);
        if (room == null)
            throw new ChatRoomNotFoundException(request.RoomId);
        
        // 2. 메시지 목록 조회 (페이지네이션)
        var messages = await _repository.GetMessagesAsync(
            request.RoomId, 
            request.Skip, 
            request.Take, 
            cancellationToken);
        
        // 3. DTO로 변환
        var messageDtos = ChatMapper.ToDtoList(messages);
        
        // 4. 응답 생성
        return new GetChatHistoryResponse
        {
            RoomId = request.RoomId,
            Messages = messageDtos,
            TotalCount = room.Messages.Count
        };
    }
}

