using System.Collections.Concurrent;
using FpsServer.Application.Chat.Ports;
using FpsServer.Domain.Chat;

namespace FpsServer.Infrastructure.Chat;

/// <summary>
/// 메모리 기반 채팅 저장소 구현
/// MVP 단계에서 사용하며, 향후 DB 기반으로 확장 가능합니다.
/// </summary>
public class InMemoryChatRepository : IChatRepository
{
    private readonly ConcurrentDictionary<string, ChatRoom> _rooms = new();
    private readonly ConcurrentDictionary<string, List<ChatMessage>> _messages = new();
    private readonly object _lockObject = new();
    
    /// <summary>
    /// 채팅방을 가져오거나 생성합니다.
    /// </summary>
    public Task<ChatRoom> GetOrCreateRoomAsync(string roomId, string roomName, CancellationToken cancellationToken = default)
    {
        var room = _rooms.GetOrAdd(roomId, _ => new ChatRoom(roomId, roomName));
        return Task.FromResult(room);
    }
    
    /// <summary>
    /// 채팅방을 조회합니다.
    /// </summary>
    public Task<ChatRoom?> FindRoomAsync(string roomId, CancellationToken cancellationToken = default)
    {
        if (_rooms.TryGetValue(roomId, out var room))
            return Task.FromResult<ChatRoom?>(room);
        
        return Task.FromResult<ChatRoom?>(null);
    }
    
    /// <summary>
    /// 메시지를 저장합니다.
    /// </summary>
    public Task SaveMessageAsync(ChatRoom room, ChatMessage message, CancellationToken cancellationToken = default)
    {
        var messages = _messages.GetOrAdd(room.RoomId, _ => new List<ChatMessage>());
        
        lock (_lockObject)
        {
            messages.Add(message);
            
            // 최근 1000개 메시지만 유지 (메모리 관리)
            if (messages.Count > 1000)
            {
                messages.RemoveAt(0);
            }
        }
        
        return Task.CompletedTask;
    }
    
    /// <summary>
    /// 채팅방의 메시지 목록을 조회합니다 (페이지네이션).
    /// </summary>
    public Task<IReadOnlyList<ChatMessage>> GetMessagesAsync(
        string roomId, 
        int skip = 0, 
        int take = 50, 
        CancellationToken cancellationToken = default)
    {
        if (!_messages.TryGetValue(roomId, out var messages))
            return Task.FromResult<IReadOnlyList<ChatMessage>>(Array.Empty<ChatMessage>());
        
        lock (_lockObject)
        {
            var result = messages
                .OrderByDescending(m => m.SentAt)
                .Skip(skip)
                .Take(take)
                .OrderBy(m => m.SentAt) // 시간순 정렬로 반환
                .ToList();
            
            return Task.FromResult<IReadOnlyList<ChatMessage>>(result);
        }
    }
}

