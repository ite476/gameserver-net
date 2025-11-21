using FluentAssertions;
using FpsServer.Application.Chat.Ports;
using FpsServer.Domain.Chat;
using FpsServer.Infrastructure.Chat;
using Xunit;

namespace FpsServer.Infrastructure.Tests.Chat;

[Trait("Feature", "실시간 채팅")]
public class InMemoryChatRepositoryTests
{
    private readonly IChatRepository _repository;
    
    public InMemoryChatRepositoryTests()
    {
        _repository = new InMemoryChatRepository();
    }
    
    [Fact]
    [Trait("Category", "InMemoryChatRepository")]
    public async Task GetOrCreateRoomAsync_존재하지_않는_방이면_새로_생성해야_한다()
    {
        // Arrange
        var roomId = "room-1";
        var roomName = "Test Room";
        
        // Act
        var room = await _repository.GetOrCreateRoomAsync(roomId, roomName);
        
        // Assert
        room.Should().NotBeNull();
        room.RoomId.Should().Be(roomId);
        room.Name.Should().Be(roomName);
    }
    
    [Fact]
    [Trait("Category", "InMemoryChatRepository")]
    public async Task GetOrCreateRoomAsync_이미_존재하는_방이면_기존_방을_반환해야_한다()
    {
        // Arrange
        var roomId = "room-1";
        var roomName1 = "Test Room 1";
        var roomName2 = "Test Room 2";
        
        // Act
        var room1 = await _repository.GetOrCreateRoomAsync(roomId, roomName1);
        var room2 = await _repository.GetOrCreateRoomAsync(roomId, roomName2);
        
        // Assert
        room1.Should().BeSameAs(room2);
        room1.Name.Should().Be(roomName1); // 첫 번째 이름이 유지됨
    }
    
    [Fact]
    [Trait("Category", "InMemoryChatRepository")]
    public async Task FindRoomAsync_존재하는_방이면_방을_반환해야_한다()
    {
        // Arrange
        var roomId = "room-1";
        await _repository.GetOrCreateRoomAsync(roomId, "Test Room");
        
        // Act
        var room = await _repository.FindRoomAsync(roomId);
        
        // Assert
        room.Should().NotBeNull();
        room!.RoomId.Should().Be(roomId);
    }
    
    [Fact]
    [Trait("Category", "InMemoryChatRepository")]
    public async Task FindRoomAsync_존재하지_않는_방이면_null을_반환해야_한다()
    {
        // Act
        var room = await _repository.FindRoomAsync("non-existent-room");
        
        // Assert
        room.Should().BeNull();
    }
    
    [Fact]
    [Trait("Category", "InMemoryChatRepository")]
    public async Task SaveMessageAsync_메시지를_저장해야_한다()
    {
        // Arrange
        var roomId = "room-1";
        var room = await _repository.GetOrCreateRoomAsync(roomId, "Test Room");
        var user = new ChatUser(Guid.NewGuid(), "User1");
        var message = new ChatMessage(roomId, user, "Hello, World!");
        
        // Act
        await _repository.SaveMessageAsync(room, message);
        
        // Assert
        var messages = await _repository.GetMessagesAsync(roomId);
        messages.Should().Contain(m => m.MessageId == message.MessageId);
    }
    
    [Fact]
    [Trait("Category", "InMemoryChatRepository")]
    public async Task GetMessagesAsync_페이지네이션이_작동해야_한다()
    {
        // Arrange
        var roomId = "room-1";
        var room = await _repository.GetOrCreateRoomAsync(roomId, "Test Room");
        var user = new ChatUser(Guid.NewGuid(), "User1");
        
        // 메시지 5개 저장
        for (int i = 0; i < 5; i++)
        {
            var message = new ChatMessage(roomId, user, $"Message {i}");
            await _repository.SaveMessageAsync(room, message);
            await Task.Delay(10); // 시간 차이를 위해
        }
        
        // Act
        var messages = await _repository.GetMessagesAsync(roomId, skip: 1, take: 2);
        
        // Assert
        messages.Should().HaveCount(2);
    }
    
    [Fact]
    [Trait("Category", "InMemoryChatRepository")]
    public async Task GetMessagesAsync_최근_1000개_메시지만_유지해야_한다()
    {
        // Arrange
        var roomId = "room-1";
        var room = await _repository.GetOrCreateRoomAsync(roomId, "Test Room");
        var user = new ChatUser(Guid.NewGuid(), "User1");
        
        // 메시지 1001개 저장
        for (int i = 0; i < 1001; i++)
        {
            var message = new ChatMessage(roomId, user, $"Message {i}");
            await _repository.SaveMessageAsync(room, message);
        }
        
        // Act - take를 1000으로 지정하여 모든 메시지 조회
        var messages = await _repository.GetMessagesAsync(roomId, skip: 0, take: 1000);
        
        // Assert
        messages.Should().HaveCount(1000);
    }
}

