using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using FpsServer.Application.MMR.Ports;
using FpsServer.Domain.MMR;
using FpsServer.Infrastructure.Persistence;

namespace FpsServer.Infrastructure.MMR;

/// <summary>
/// MMR Infrastructure 레이어 DI 확장 메서드
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// MMR Infrastructure 서비스 등록
    /// </summary>
    /// <param name="services">서비스 컬렉션</param>
    /// <param name="connectionString">데이터베이스 연결 문자열 (선택적, 없으면 In-Memory 사용)</param>
    /// <returns>서비스 컬렉션 (체이닝 지원)</returns>
    public static IServiceCollection AddMMRInfrastructure(
        this IServiceCollection services,
        string? connectionString = null)
    {
        // Repository 구현 등록
        if (!string.IsNullOrEmpty(connectionString))
        {
            // EF Core 기반 Repository 사용
            // DbContext는 AddMatchSessionInfrastructure에서 이미 등록되었을 수 있으므로 확인 필요
            services.AddScoped<IPlayerMMRRepository, EfPlayerMMRRepository>();
        }
        else
        {
            // In-Memory Repository 사용 (MVP)
            services.AddSingleton<IPlayerMMRRepository, InMemoryPlayerMMRRepository>();
        }
        
        // MMR Calculator 등록 (Domain Service)
        services.AddSingleton<IMMRCalculator, EloRatingCalculator>();
        
        return services;
    }
}

