using Microsoft.EntityFrameworkCore;
using DomainMatchSession = FpsServer.Domain.MatchSession.MatchSession;
using DomainMatchResult = FpsServer.Domain.MatchSession.MatchResult;
using DomainPlayerResult = FpsServer.Domain.MatchSession.PlayerResult;
using FpsServer.Domain.MMR;
using FpsServer.Domain.Matchmaking;

namespace FpsServer.Infrastructure.Persistence;

/// <summary>
/// FPS 서버 DbContext
/// </summary>
public class FpsDbContext : DbContext
{
    /// <summary>
    /// 매치 세션 DbSet
    /// </summary>
    public DbSet<DomainMatchSession> MatchSessions { get; set; } = null!;
    
    /// <summary>
    /// 매치 결과 DbSet
    /// </summary>
    public DbSet<DomainMatchResult> MatchResults { get; set; } = null!;
    
    /// <summary>
    /// 플레이어 MMR DbSet
    /// </summary>
    public DbSet<PlayerMMR> PlayerMMRs { get; set; } = null!;
    
    /// <summary>
    /// 생성자
    /// </summary>
    /// <param name="options">DbContext 옵션</param>
    public FpsDbContext(DbContextOptions<FpsDbContext> options)
        : base(options)
    {
    }
    
    /// <summary>
    /// 모델 구성 (Fluent API)
    /// </summary>
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        
        // MatchSession 엔티티 매핑
        modelBuilder.Entity<DomainMatchSession>(entity =>
        {
            entity.ToTable("MatchSessions");
            entity.HasKey(e => e.SessionId);
            
            entity.Property(e => e.SessionId)
                .HasColumnName("SessionId");
            
            entity.Property(e => e.MatchId)
                .HasColumnName("MatchId")
                .IsRequired();
            
            entity.Property(e => e.Status)
                .HasColumnName("Status")
                .HasConversion<int>()
                .IsRequired();
            
            entity.Property(e => e.StartedAt)
                .HasColumnName("StartedAt");
            
            entity.Property(e => e.EndedAt)
                .HasColumnName("EndedAt");
            
            entity.Property(e => e.GameMode)
                .HasColumnName("GameMode")
                .HasConversion<int>()
                .IsRequired();
            
            // PlayerIds를 JSON으로 저장 (EF Core 8 Complex Type 대신)
            entity.Property(e => e.PlayerIds)
                .HasColumnName("PlayerIds")
                .HasConversion(
                    v => string.Join(",", v),
                    v => v.Split(',', StringSplitOptions.RemoveEmptyEntries)
                        .Select(Guid.Parse)
                        .ToList())
                .IsRequired();
            
            // MatchResult와의 관계는 MatchId를 통해 연결 (별도 엔티티)
            // MatchSession의 Result 속성은 private 필드이므로 EF Core가 자동으로 매핑하지 않음
            // 필요 시 MatchId로 조회하여 사용
            
            // MatchId에 인덱스 추가
            entity.HasIndex(e => e.MatchId)
                .IsUnique();
        });
        
        // MatchResult 엔티티 매핑
        modelBuilder.Entity<DomainMatchResult>(entity =>
        {
            entity.ToTable("MatchResults");
            entity.HasKey(e => e.ResultId);
            
            entity.Property(e => e.ResultId)
                .HasColumnName("ResultId");
            
            entity.Property(e => e.MatchId)
                .HasColumnName("MatchId")
                .IsRequired();
            
            entity.Property(e => e.WinnerId)
                .HasColumnName("WinnerId");
            
            entity.Property(e => e.EndedAt)
                .HasColumnName("EndedAt")
                .IsRequired();
            
            // PlayerResults를 JSON으로 저장
            entity.Property(e => e.PlayerResults)
                .HasColumnName("PlayerResults")
                .HasConversion(
                    v => System.Text.Json.JsonSerializer.Serialize(v, (System.Text.Json.JsonSerializerOptions?)null),
                    v => System.Text.Json.JsonSerializer.Deserialize<IReadOnlyList<DomainPlayerResult>>(v, (System.Text.Json.JsonSerializerOptions?)null) ?? new List<DomainPlayerResult>())
                .IsRequired();
            
            // MatchId에 인덱스 추가
            entity.HasIndex(e => e.MatchId)
                .IsUnique();
        });
        
        // PlayerMMR 엔티티 매핑
        modelBuilder.Entity<PlayerMMR>(entity =>
        {
            entity.ToTable("PlayerMMRs");
            entity.HasKey(e => e.PlayerId);
            
            entity.Property(e => e.PlayerId)
                .HasColumnName("PlayerId");
            
            // MMR을 Complex Type으로 매핑 (EF Core 8)
            entity.ComplexProperty(e => e.CurrentMMR, mmr =>
            {
                mmr.Property(m => m.Value)
                    .HasColumnName("MMR")
                    .IsRequired();
            });
            
            entity.Property(e => e.UpdatedAt)
                .HasColumnName("UpdatedAt")
                .IsRequired();
        });
    }
}

