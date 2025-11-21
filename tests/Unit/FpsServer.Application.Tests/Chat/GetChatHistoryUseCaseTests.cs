using FluentAssertions;
using FpsServer.Application.Chat.DTOs;
using FpsServer.Application.Chat.Ports;
using FpsServer.Application.Chat.UseCases;
using FpsServer.Domain.Chat;
using FpsServer.Domain.Chat.Exceptions;
using Moq;
using Xunit;

namespace FpsServer.Application.Tests.Chat;

[Trait("Feature", "실시간 채팅")]
public class GetChatHistoryUseCaseTests
{
    private readonly Mock<IChatRepository> _repositoryMock;
    private readonly GetChatHistoryUseCase _useCase;
    
    public GetChatHistoryUseCaseTests()
    {
        _repositoryMock = new Mock<IChatRepository>();
        _useCase = new GetChatHistoryUseCase(_repositoryMock.Object);
    }
    
    [Fact]
    [Trait("Category", "채팅 히스토리 조회 유스케이스")]
    public async Task 존재하는_채팅방이면_메시지_목록을_반환해야_한다()
    {
        // Arrange
        var request = new GetChatHistoryRequest
        {
            RoomId = "room-1",
            Skip = 0,
            Take = 50
        };
        
        var room = new ChatRoom("room-1", "Test Room");
        var sender1 = new ChatUser(Guid.NewGuid(), "User1");
        var sender2 = new ChatUser(Guid.NewGuid(), "User2");
        var message1 = new ChatMessage("room-1", sender1, "Message 1");
        var message2 = new ChatMessage("room-1", sender2, "Message 2");
        room.AddMessage(message1);
        room.AddMessage(message2);
        
        _repositoryMock
            .Setup(r => r.FindRoomAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(room);
        _repositoryMock
            .Setup(r => r.GetMessagesAsync(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<ChatMessage> { message1, message2 });
        
        // Act
        var result = await _useCase.ExecuteAsync(request);
        
        // Assert
        result.Should().NotBeNull();
        result.RoomId.Should().Be("room-1");
        result.Messages.Should().HaveCount(2);
        result.TotalCount.Should().Be(2);
        result.Messages[0].Content.Should().Be("Message 1");
        result.Messages[1].Content.Should().Be("Message 2");
        
        _repositoryMock.Verify(
            r => r.FindRoomAsync("room-1", It.IsAny<CancellationToken>()), 
            Times.Once);
        _repositoryMock.Verify(
            r => r.GetMessagesAsync("room-1", 0, 50, It.IsAny<CancellationToken>()), 
            Times.Once);
    }
    
    [Fact]
    [Trait("Category", "채팅 히스토리 조회 유스케이스")]
    public async Task 존재하지_않는_채팅방이면_ChatRoomNotFoundException을_발생시켜야_한다()
    {
        // Arrange
        var request = new GetChatHistoryRequest
        {
            RoomId = "room-999",
            Skip = 0,
            Take = 50
        };
        
        _repositoryMock
            .Setup(r => r.FindRoomAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((ChatRoom?)null);
        
        // Act & Assert
        var act = async () => await _useCase.ExecuteAsync(request);
        var exception = await act.Should().ThrowAsync<ChatRoomNotFoundException>();
        exception.Which.RoomId.Should().Be("room-999");
        
        _repositoryMock.Verify(
            r => r.GetMessagesAsync(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<CancellationToken>()), 
            Times.Never);
    }
    
    [Fact]
    [Trait("Category", "채팅 히스토리 조회 유스케이스")]
    public async Task 페이지네이션_파라미터가_주어지면_적용해야_한다()
    {
        // Arrange
        var request = new GetChatHistoryRequest
        {
            RoomId = "room-1",
            Skip = 10,
            Take = 20
        };
        
        var room = new ChatRoom("room-1", "Test Room");
        _repositoryMock
            .Setup(r => r.FindRoomAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(room);
        _repositoryMock
            .Setup(r => r.GetMessagesAsync(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<ChatMessage>());
        
        // Act
        var result = await _useCase.ExecuteAsync(request);
        
        // Assert
        result.Should().NotBeNull();
        
        _repositoryMock.Verify(
            r => r.GetMessagesAsync("room-1", 10, 20, It.IsAny<CancellationToken>()), 
            Times.Once);
    }
}

