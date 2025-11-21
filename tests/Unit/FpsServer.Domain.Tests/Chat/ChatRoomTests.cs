using FluentAssertions;
using FpsServer.Domain.Chat;
using FpsServer.Domain.Chat.Exceptions;
using Xunit;

namespace FpsServer.Domain.Tests.Chat;

[Trait("Feature", "실시간 채팅")]
public class ChatRoomTests
{
    [Fact]
    [Trait("Category", "채팅방")]
    public void 채팅방_ID와_이름이_주어지면_채팅방을_생성해야_한다()
    {
        // Arrange & Act
        var roomId = "room-1";
        var roomName = "Test Room";
        var room = new ChatRoom(roomId, roomName);
        
        // Assert
        room.RoomId.Should().Be(roomId);
        room.Name.Should().Be(roomName);
        room.Messages.Should().BeEmpty();
        room.ConnectedUserIds.Should().BeEmpty();
        room.CreatedAt.Should().BeCloseTo(DateTimeOffset.UtcNow, TimeSpan.FromSeconds(1));
    }
    
    [Fact]
    [Trait("Category", "채팅방")]
    public void null_또는_빈_방_ID면_ArgumentNullException을_발생시켜야_한다()
    {
        // Arrange & Act & Assert
        var act1 = () => new ChatRoom(null!, "Test Room");
        act1.Should().Throw<ArgumentNullException>()
            .And.ParamName.Should().Be("roomId");
        
        var act2 = () => new ChatRoom("", "Test Room");
        act2.Should().Throw<ArgumentNullException>()
            .And.ParamName.Should().Be("roomId");
        
        var act3 = () => new ChatRoom("   ", "Test Room");
        act3.Should().Throw<ArgumentNullException>()
            .And.ParamName.Should().Be("roomId");
    }
    
    [Fact]
    [Trait("Category", "채팅방")]
    public void null_또는_빈_방_이름이면_ArgumentNullException을_발생시켜야_한다()
    {
        // Arrange & Act & Assert
        var act1 = () => new ChatRoom("room-1", null!);
        act1.Should().Throw<ArgumentNullException>()
            .And.ParamName.Should().Be("name");
        
        var act2 = () => new ChatRoom("room-1", "");
        act2.Should().Throw<ArgumentNullException>()
            .And.ParamName.Should().Be("name");
    }
    
    [Fact]
    [Trait("Category", "채팅방")]
    public void 유효한_메시지면_채팅방에_추가해야_한다()
    {
        // Arrange
        var room = new ChatRoom("room-1", "Test Room");
        var sender = new ChatUser(Guid.NewGuid(), "User1");
        var message = new ChatMessage("room-1", sender, "Hello");
        
        // Act
        room.AddMessage(message);
        
        // Assert
        room.Messages.Should().HaveCount(1);
        room.Messages[0].Should().Be(message);
        room.Messages[0].Content.Should().Be("Hello");
    }
    
    [Fact]
    [Trait("Category", "채팅방")]
    public void null_메시지면_ArgumentNullException을_발생시켜야_한다()
    {
        // Arrange
        var room = new ChatRoom("room-1", "Test Room");
        
        // Act
        var act = () => room.AddMessage(null!);
        
        // Assert
        act.Should().Throw<ArgumentNullException>()
            .And.ParamName.Should().Be("message");
    }
    
    [Fact]
    [Trait("Category", "채팅방")]
    public void 메시지의_방_ID가_채팅방_ID와_일치하지_않으면_InvalidOperationException을_발생시켜야_한다()
    {
        // Arrange
        var room = new ChatRoom("room-1", "Test Room");
        var sender = new ChatUser(Guid.NewGuid(), "User1");
        var message = new ChatMessage("room-2", sender, "Hello"); // 다른 방 ID
        
        // Act
        var act = () => room.AddMessage(message);
        
        // Assert
        act.Should().Throw<InvalidOperationException>()
            .WithMessage("*RoomId*");
    }
    
    [Fact]
    [Trait("Category", "채팅방")]
    public void 여러_메시지를_추가하면_순서를_유지해야_한다()
    {
        // Arrange
        var room = new ChatRoom("room-1", "Test Room");
        var sender1 = new ChatUser(Guid.NewGuid(), "User1");
        var sender2 = new ChatUser(Guid.NewGuid(), "User2");
        var message1 = new ChatMessage("room-1", sender1, "Message 1");
        var message2 = new ChatMessage("room-1", sender2, "Message 2");
        
        // Act
        room.AddMessage(message1);
        room.AddMessage(message2);
        
        // Assert
        room.Messages.Should().HaveCount(2);
        room.Messages[0].Should().Be(message1);
        room.Messages[1].Should().Be(message2);
    }
    
    [Fact]
    [Trait("Category", "채팅방")]
    public void 사용자를_연결하면_연결된_사용자_목록에_추가해야_한다()
    {
        // Arrange
        var room = new ChatRoom("room-1", "Test Room");
        var userId = Guid.NewGuid();
        
        // Act
        room.ConnectUser(userId);
        
        // Assert
        room.ConnectedUserIds.Should().Contain(userId);
        room.IsUserConnected(userId).Should().BeTrue();
    }
    
    [Fact]
    [Trait("Category", "채팅방")]
    public void 사용자를_연결_해제하면_연결된_사용자_목록에서_제거해야_한다()
    {
        // Arrange
        var room = new ChatRoom("room-1", "Test Room");
        var userId = Guid.NewGuid();
        room.ConnectUser(userId);
        
        // Act
        room.DisconnectUser(userId);
        
        // Assert
        room.ConnectedUserIds.Should().NotContain(userId);
        room.IsUserConnected(userId).Should().BeFalse();
    }
    
    [Fact]
    [Trait("Category", "채팅방")]
    public void 존재하지_않는_사용자를_연결_해제해도_예외가_발생하지_않아야_한다()
    {
        // Arrange
        var room = new ChatRoom("room-1", "Test Room");
        var userId = Guid.NewGuid();
        
        // Act
        var act = () => room.DisconnectUser(userId);
        
        // Assert
        act.Should().NotThrow();
        room.ConnectedUserIds.Should().NotContain(userId);
    }
}

