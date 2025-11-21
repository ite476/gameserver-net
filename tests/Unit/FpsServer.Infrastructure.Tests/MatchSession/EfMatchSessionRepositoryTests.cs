using FluentAssertions;
using FpsServer.Domain.Matchmaking;
using FpsServer.Domain.MatchSession;
using FpsServer.Infrastructure.MatchSession;
using FpsServer.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using DomainMatchSession = FpsServer.Domain.MatchSession.MatchSession;
using Xunit;

namespace FpsServer.Infrastructure.Tests.MatchSession;

[Trait("Feature", "매치 세션")]
public class EfMatchSessionRepositoryTests
{
    private FpsDbContext CreateDbContext()
    {
        var options = new DbContextOptionsBuilder<FpsDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;
        return new FpsDbContext(options);
    }
    
    [Fact]
    [Trait("Category", "EF Core 저장소")]
    public async Task CreateAsync_세션을_생성해야_한다()
    {
        // Arrange
        using var context = CreateDbContext();
        var repository = new EfMatchSessionRepository(context);
        var matchId = Guid.NewGuid();
        var playerIds = new List<Guid> { Guid.NewGuid(), Guid.NewGuid() };
        var session = new DomainMatchSession(matchId, playerIds, MatchmakingMode.Solo);
        
        // Act
        await repository.CreateAsync(session);
        
        // Assert
        var saved = await context.MatchSessions.FirstOrDefaultAsync(s => s.SessionId == session.SessionId);
        saved.Should().NotBeNull();
        saved!.MatchId.Should().Be(matchId);
        saved.PlayerIds.Should().BeEquivalentTo(playerIds);
        saved.GameMode.Should().Be(MatchmakingMode.Solo);
        saved.Status.Should().Be(MatchStatus.Matched);
    }
    
    [Fact]
    [Trait("Category", "EF Core 저장소")]
    public async Task FindByIdAsync_세션ID로_조회해야_한다()
    {
        // Arrange
        using var context = CreateDbContext();
        var repository = new EfMatchSessionRepository(context);
        var matchId = Guid.NewGuid();
        var playerIds = new List<Guid> { Guid.NewGuid() };
        var session = new DomainMatchSession(matchId, playerIds, MatchmakingMode.Solo);
        await repository.CreateAsync(session);
        
        // Act
        var found = await repository.FindByIdAsync(session.SessionId);
        
        // Assert
        found.Should().NotBeNull();
        found!.SessionId.Should().Be(session.SessionId);
        found.MatchId.Should().Be(matchId);
    }
    
    [Fact]
    [Trait("Category", "EF Core 저장소")]
    public async Task FindByIdAsync_존재하지_않는_세션이면_null을_반환해야_한다()
    {
        // Arrange
        using var context = CreateDbContext();
        var repository = new EfMatchSessionRepository(context);
        
        // Act
        var found = await repository.FindByIdAsync(Guid.NewGuid());
        
        // Assert
        found.Should().BeNull();
    }
    
    [Fact]
    [Trait("Category", "EF Core 저장소")]
    public async Task FindByMatchIdAsync_매치ID로_조회해야_한다()
    {
        // Arrange
        using var context = CreateDbContext();
        var repository = new EfMatchSessionRepository(context);
        var matchId = Guid.NewGuid();
        var playerIds = new List<Guid> { Guid.NewGuid() };
        var session = new DomainMatchSession(matchId, playerIds, MatchmakingMode.Solo);
        await repository.CreateAsync(session);
        
        // Act
        var found = await repository.FindByMatchIdAsync(matchId);
        
        // Assert
        found.Should().NotBeNull();
        found!.MatchId.Should().Be(matchId);
        found.SessionId.Should().Be(session.SessionId);
    }
    
    [Fact]
    [Trait("Category", "EF Core 저장소")]
    public async Task FindByMatchIdAsync_존재하지_않는_매치ID면_null을_반환해야_한다()
    {
        // Arrange
        using var context = CreateDbContext();
        var repository = new EfMatchSessionRepository(context);
        
        // Act
        var found = await repository.FindByMatchIdAsync(Guid.NewGuid());
        
        // Assert
        found.Should().BeNull();
    }
    
    [Fact]
    [Trait("Category", "EF Core 저장소")]
    public async Task UpdateAsync_세션을_업데이트해야_한다()
    {
        // Arrange
        using var context = CreateDbContext();
        var repository = new EfMatchSessionRepository(context);
        var matchId = Guid.NewGuid();
        var playerIds = new List<Guid> { Guid.NewGuid() };
        var session = new DomainMatchSession(matchId, playerIds, MatchmakingMode.Solo);
        await repository.CreateAsync(session);
        
        // Act
        session.Start(); // 상태 전이: Matched → InProgress
        await repository.UpdateAsync(session);
        
        // Assert
        var updated = await context.MatchSessions.FirstOrDefaultAsync(s => s.SessionId == session.SessionId);
        updated.Should().NotBeNull();
        updated!.Status.Should().Be(MatchStatus.InProgress);
        updated.StartedAt.Should().NotBeNull();
    }
}

