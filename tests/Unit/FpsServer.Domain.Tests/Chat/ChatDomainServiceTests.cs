using FluentAssertions;
using FpsServer.Domain.Chat;
using FpsServer.Domain.Chat.Exceptions;
using Xunit;

namespace FpsServer.Domain.Tests.Chat;

[Trait("Feature", "실시간 채팅")]
public class ChatDomainServiceTests
{
    private readonly ChatDomainService _domainService;
    
    public ChatDomainServiceTests()
    {
        _domainService = new ChatDomainService();
    }
    
    [Fact]
    [Trait("Category", "메시지 검증")]
    public void 유효한_메시지면_검증을_통과해야_한다()
    {
        // Arrange
        var content = "Hello, World!";
        
        // Act
        var result = _domainService.ValidateAndFilterMessage(content);
        
        // Assert
        result.Should().Be("Hello, World!");
    }
    
    [Fact]
    [Trait("Category", "메시지 검증")]
    public void null_메시지면_ArgumentNullException을_발생시켜야_한다()
    {
        // Arrange & Act
        var act = () => _domainService.ValidateAndFilterMessage(null!);
        
        // Assert
        act.Should().Throw<ArgumentNullException>()
            .And.ParamName.Should().Be("content");
    }
    
    [Fact]
    [Trait("Category", "메시지 검증")]
    public void 빈_메시지면_InvalidChatMessageException을_발생시켜야_한다()
    {
        // Arrange
        var content = "   "; // 공백만 있는 메시지
        
        // Act
        var act = () => _domainService.ValidateAndFilterMessage(content);
        
        // Assert
        act.Should().Throw<InvalidChatMessageException>()
            .WithMessage("*cannot be empty*");
    }
    
    [Fact]
    [Trait("Category", "메시지 검증")]
    public void 최대_길이를_초과하는_메시지면_InvalidChatMessageException을_발생시켜야_한다()
    {
        // Arrange
        var content = new string('a', 501); // 501자 (최대 500자)
        
        // Act
        var act = () => _domainService.ValidateAndFilterMessage(content);
        
        // Assert
        act.Should().Throw<InvalidChatMessageException>()
            .WithMessage("*exceeds maximum*");
    }
    
    [Fact]
    [Trait("Category", "메시지 검증")]
    public void 공백이_포함된_메시지는_앞뒤_공백을_제거해야_한다()
    {
        // Arrange
        var content = "  Hello, World!  ";
        
        // Act
        var result = _domainService.ValidateAndFilterMessage(content);
        
        // Assert
        result.Should().Be("Hello, World!");
    }
    
    [Fact]
    [Trait("Category", "금지어 필터")]
    public void 금지어가_포함된_메시지는_필터링되어야_한다()
    {
        // Arrange
        var content = "This is a spam message";
        
        // Act
        var result = _domainService.ValidateAndFilterMessage(content);
        
        // Assert
        result.Should().Contain("***");
        result.Should().NotContain("spam");
    }
    
    [Fact]
    [Trait("Category", "금지어 필터")]
    public void 금지어는_대소문자_구분_없이_필터링되어야_한다()
    {
        // Arrange
        var content1 = "This is a SPAM message";
        var content2 = "This is a Spam message";
        
        // Act
        var result1 = _domainService.ValidateAndFilterMessage(content1);
        var result2 = _domainService.ValidateAndFilterMessage(content2);
        
        // Assert
        result1.Should().Contain("***");
        result1.Should().NotContain("SPAM");
        result2.Should().Contain("***");
        result2.Should().NotContain("Spam");
    }
}

