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
public class SendMessageUseCaseTests
{
    private readonly Mock<IChatRepository> _repositoryMock;
    private readonly Mock<IChatNotifier> _notifierMock;
    private readonly ChatDomainService _domainService;
    private readonly SendMessageUseCase _useCase;
    
    public SendMessageUseCaseTests()
    {
        _repositoryMock = new Mock<IChatRepository>();
        _notifierMock = new Mock<IChatNotifier>();
        _domainService = new ChatDomainService();
        _useCase = new SendMessageUseCase(
            _repositoryMock.Object,
            _domainService,
            _notifierMock.Object
        );
    }
    
    [Fact]
    [Trait("Category", "메시지 전송 유스케이스")]
    public async Task 유효한_요청이면_메시지를_전송해야_한다()
    {
        // Arrange
        var request = new SendMessageRequest
        {
            RoomId = "room-1",
            UserId = Guid.NewGuid(),
            Nickname = "User1",
            Content = "Hello, World!"
        };
        
        var room = new ChatRoom("room-1", "Test Room");
        _repositoryMock
            .Setup(r => r.GetOrCreateRoomAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(room);
        _repositoryMock
            .Setup(r => r.SaveMessageAsync(It.IsAny<ChatRoom>(), It.IsAny<ChatMessage>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);
        _notifierMock
            .Setup(n => n.NotifyMessageSentAsync(It.IsAny<string>(), It.IsAny<ChatMessage>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);
        
        // Act
        var result = await _useCase.ExecuteAsync(request);
        
        // Assert
        result.Should().NotBeNull();
        result.MessageId.Should().NotBeEmpty();
        result.SentAt.Should().BeCloseTo(DateTimeOffset.UtcNow, TimeSpan.FromSeconds(1));
        
        _repositoryMock.Verify(
            r => r.GetOrCreateRoomAsync("room-1", It.IsAny<string>(), It.IsAny<CancellationToken>()), 
            Times.Once);
        _repositoryMock.Verify(
            r => r.SaveMessageAsync(It.IsAny<ChatRoom>(), It.IsAny<ChatMessage>(), It.IsAny<CancellationToken>()), 
            Times.Once);
        _notifierMock.Verify(
            n => n.NotifyMessageSentAsync("room-1", It.IsAny<ChatMessage>(), It.IsAny<CancellationToken>()), 
            Times.Once);
    }
    
    [Fact]
    [Trait("Category", "메시지 전송 유스케이스")]
    public async Task 메시지가_유효하지_않으면_InvalidChatMessageException을_발생시켜야_한다()
    {
        // Arrange
        var request = new SendMessageRequest
        {
            RoomId = "room-1",
            UserId = Guid.NewGuid(),
            Nickname = "User1",
            Content = new string('a', 501) // 최대 길이 초과
        };
        
        var room = new ChatRoom("room-1", "Test Room");
        _repositoryMock
            .Setup(r => r.GetOrCreateRoomAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(room);
        
        // Act & Assert
        var act = async () => await _useCase.ExecuteAsync(request);
        await act.Should().ThrowAsync<InvalidChatMessageException>();
        
        _repositoryMock.Verify(
            r => r.SaveMessageAsync(It.IsAny<ChatRoom>(), It.IsAny<ChatMessage>(), It.IsAny<CancellationToken>()), 
            Times.Never);
    }
    
    [Fact]
    [Trait("Category", "메시지 전송 유스케이스")]
    public async Task 금지어가_포함된_메시지는_필터링되어야_한다()
    {
        // Arrange
        var request = new SendMessageRequest
        {
            RoomId = "room-1",
            UserId = Guid.NewGuid(),
            Nickname = "User1",
            Content = "This is a spam message"
        };
        
        var room = new ChatRoom("room-1", "Test Room");
        _repositoryMock
            .Setup(r => r.GetOrCreateRoomAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(room);
        _repositoryMock
            .Setup(r => r.SaveMessageAsync(It.IsAny<ChatRoom>(), It.IsAny<ChatMessage>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);
        
        // Act
        var result = await _useCase.ExecuteAsync(request);
        
        // Assert
        result.Should().NotBeNull();
        
        // 저장된 메시지가 필터링되었는지 확인
        _repositoryMock.Verify(
            r => r.SaveMessageAsync(
                It.IsAny<ChatRoom>(), 
                It.Is<ChatMessage>(m => m.Content.Contains("***") && !m.Content.Contains("spam")), 
                It.IsAny<CancellationToken>()), 
            Times.Once);
    }
    
    [Fact]
    [Trait("Category", "메시지 전송 유스케이스")]
    public async Task 공백이_포함된_메시지는_앞뒤_공백이_제거되어야_한다()
    {
        // Arrange
        var request = new SendMessageRequest
        {
            RoomId = "room-1",
            UserId = Guid.NewGuid(),
            Nickname = "User1",
            Content = "  Hello, World!  "
        };
        
        var room = new ChatRoom("room-1", "Test Room");
        _repositoryMock
            .Setup(r => r.GetOrCreateRoomAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(room);
        _repositoryMock
            .Setup(r => r.SaveMessageAsync(It.IsAny<ChatRoom>(), It.IsAny<ChatMessage>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);
        
        // Act
        var result = await _useCase.ExecuteAsync(request);
        
        // Assert
        result.Should().NotBeNull();
        
        // 저장된 메시지의 공백이 제거되었는지 확인
        _repositoryMock.Verify(
            r => r.SaveMessageAsync(
                It.IsAny<ChatRoom>(), 
                It.Is<ChatMessage>(m => m.Content == "Hello, World!"), 
                It.IsAny<CancellationToken>()), 
            Times.Once);
    }
}

