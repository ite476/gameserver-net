using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using FpsServer.Application.MatchSession.Ports;
using FpsServer.Infrastructure.Persistence;

namespace FpsServer.Infrastructure.MatchSession;

/// <summary>
/// 매치 세션 Infrastructure 레이어 DI 확장 메서드
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// 매치 세션 Infrastructure 서비스 등록
    /// </summary>
    /// <param name="services">서비스 컬렉션</param>
    /// <param name="connectionString">데이터베이스 연결 문자열 (선택적, 없으면 In-Memory 사용)</param>
    /// <returns>서비스 컬렉션 (체이닝 지원)</returns>
    public static IServiceCollection AddMatchSessionInfrastructure(
        this IServiceCollection services,
        string? connectionString = null)
    {
        // DbContext 등록
        if (!string.IsNullOrEmpty(connectionString))
        {
            // EF Core 기반 Repository 사용
            services.AddDbContext<FpsDbContext>(options =>
                options.UseSqlServer(connectionString));
            services.AddScoped<IMatchSessionRepository, EfMatchSessionRepository>();
        }
        else
        {
            // In-Memory Repository 사용 (MVP)
            services.AddSingleton<IMatchSessionRepository, InMemoryMatchSessionRepository>();
        }
        
        return services;
    }
}

