using FluentAssertions;
using FpsServer.Domain.MMR;
using FpsServer.Domain.Matchmaking;
using FpsServer.Infrastructure.MMR;
using FpsServer.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Xunit;
using DomainMMR = FpsServer.Domain.Matchmaking.MMR;

namespace FpsServer.Infrastructure.Tests.MMR;

[Trait("Feature", "MMR")]
public class EfPlayerMMRRepositoryTests
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
    public async Task FindByPlayerIdAsync_플레이어ID로_MMR을_조회해야_한다()
    {
        // Arrange
        using var context = CreateDbContext();
        var repository = new EfPlayerMMRRepository(context);
        var playerId = Guid.NewGuid();
        var playerMMR = new PlayerMMR(playerId, new DomainMMR(1500));
        await context.PlayerMMRs.AddAsync(playerMMR);
        await context.SaveChangesAsync();
        
        // Act
        var found = await repository.FindByPlayerIdAsync(playerId);
        
        // Assert
        found.Should().NotBeNull();
        found!.PlayerId.Should().Be(playerId);
        found.CurrentMMR.Value.Should().Be(1500);
    }
    
    [Fact]
    [Trait("Category", "EF Core 저장소")]
    public async Task FindByPlayerIdAsync_존재하지_않는_플레이어면_null을_반환해야_한다()
    {
        // Arrange
        using var context = CreateDbContext();
        var repository = new EfPlayerMMRRepository(context);
        
        // Act
        var found = await repository.FindByPlayerIdAsync(Guid.NewGuid());
        
        // Assert
        found.Should().BeNull();
    }
    
    [Fact]
    [Trait("Category", "EF Core 저장소")]
    public async Task FindMultipleByPlayerIdsAsync_여러_플레이어_MMR을_조회해야_한다()
    {
        // Arrange
        using var context = CreateDbContext();
        var repository = new EfPlayerMMRRepository(context);
        var playerId1 = Guid.NewGuid();
        var playerId2 = Guid.NewGuid();
        var playerId3 = Guid.NewGuid();
        
        await context.PlayerMMRs.AddRangeAsync(
            new PlayerMMR(playerId1, new DomainMMR(1500)),
            new PlayerMMR(playerId2, new DomainMMR(1600)),
            new PlayerMMR(playerId3, new DomainMMR(1700)));
        await context.SaveChangesAsync();
        
        // Act
        var found = await repository.FindMultipleByPlayerIdsAsync(new[] { playerId1, playerId2 });
        
        // Assert
        found.Should().HaveCount(2);
        found.Select(p => p.PlayerId).Should().Contain(playerId1);
        found.Select(p => p.PlayerId).Should().Contain(playerId2);
        found.Select(p => p.PlayerId).Should().NotContain(playerId3);
    }
    
    [Fact]
    [Trait("Category", "EF Core 저장소")]
    public async Task SaveAsync_새_플레이어_MMR을_생성해야_한다()
    {
        // Arrange
        using var context = CreateDbContext();
        var repository = new EfPlayerMMRRepository(context);
        var playerId = Guid.NewGuid();
        var playerMMR = new PlayerMMR(playerId, new DomainMMR(1500));
        
        // Act
        await repository.SaveAsync(playerMMR);
        
        // Assert
        var saved = await context.PlayerMMRs.FirstOrDefaultAsync(p => p.PlayerId == playerId);
        saved.Should().NotBeNull();
        saved!.CurrentMMR.Value.Should().Be(1500);
    }
    
    [Fact]
    [Trait("Category", "EF Core 저장소")]
    public async Task SaveAsync_기존_플레이어_MMR을_업데이트해야_한다()
    {
        // Arrange
        using var context = CreateDbContext();
        var repository = new EfPlayerMMRRepository(context);
        var playerId = Guid.NewGuid();
        var initialMMR = new PlayerMMR(playerId, new DomainMMR(1500));
        await repository.SaveAsync(initialMMR);
        
        // Act
        initialMMR.UpdateMMR(new DomainMMR(1600));
        await repository.SaveAsync(initialMMR);
        
        // Assert
        var updated = await context.PlayerMMRs.FirstOrDefaultAsync(p => p.PlayerId == playerId);
        updated.Should().NotBeNull();
        updated!.CurrentMMR.Value.Should().Be(1600);
    }
    
    [Fact]
    [Trait("Category", "EF Core 저장소")]
    public async Task SaveMultipleAsync_여러_플레이어_MMR을_일괄_저장해야_한다()
    {
        // Arrange
        using var context = CreateDbContext();
        var repository = new EfPlayerMMRRepository(context);
        var playerId1 = Guid.NewGuid();
        var playerId2 = Guid.NewGuid();
        
        var playerMMRs = new List<PlayerMMR>
        {
            new PlayerMMR(playerId1, new DomainMMR(1500)),
            new PlayerMMR(playerId2, new DomainMMR(1600))
        };
        
        // Act
        await repository.SaveMultipleAsync(playerMMRs);
        
        // Assert
        var saved = await context.PlayerMMRs
            .Where(p => p.PlayerId == playerId1 || p.PlayerId == playerId2)
            .ToListAsync();
        saved.Should().HaveCount(2);
        saved.First(p => p.PlayerId == playerId1).CurrentMMR.Value.Should().Be(1500);
        saved.First(p => p.PlayerId == playerId2).CurrentMMR.Value.Should().Be(1600);
    }
    
    [Fact]
    [Trait("Category", "EF Core 저장소")]
    public async Task SaveMultipleAsync_기존_플레이어와_신규_플레이어를_혼합_저장해야_한다()
    {
        // Arrange
        using var context = CreateDbContext();
        var repository = new EfPlayerMMRRepository(context);
        var playerId1 = Guid.NewGuid();
        var playerId2 = Guid.NewGuid();
        
        // 기존 플레이어 1명 생성
        await repository.SaveAsync(new PlayerMMR(playerId1, new DomainMMR(1500)));
        
        // Act
        var playerMMRs = new List<PlayerMMR>
        {
            new PlayerMMR(playerId1, new DomainMMR(1600)), // 업데이트
            new PlayerMMR(playerId2, new DomainMMR(1700))  // 신규 생성
        };
        await repository.SaveMultipleAsync(playerMMRs);
        
        // Assert
        var saved = await context.PlayerMMRs
            .Where(p => p.PlayerId == playerId1 || p.PlayerId == playerId2)
            .ToListAsync();
        saved.Should().HaveCount(2);
        saved.First(p => p.PlayerId == playerId1).CurrentMMR.Value.Should().Be(1600);
        saved.First(p => p.PlayerId == playerId2).CurrentMMR.Value.Should().Be(1700);
    }
    
    [Fact]
    [Trait("Category", "EF Core 저장소")]
    public async Task MMR_Complex_Type이_제대로_매핑되어야_한다()
    {
        // Arrange
        using var context = CreateDbContext();
        var repository = new EfPlayerMMRRepository(context);
        var playerId = Guid.NewGuid();
        var playerMMR = new PlayerMMR(playerId, new DomainMMR(2000));
        await repository.SaveAsync(playerMMR);
        
        // Act
        var found = await repository.FindByPlayerIdAsync(playerId);
        
        // Assert
        found.Should().NotBeNull();
        found!.CurrentMMR.Value.Should().Be(2000);
        // Complex Type이 제대로 저장되고 조회되는지 검증
    }
}

