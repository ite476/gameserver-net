using Microsoft.Extensions.DependencyInjection;
using FpsServer.Application.MMR.Ports;
using FpsServer.Domain.MMR;

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
    /// <returns>서비스 컬렉션 (체이닝 지원)</returns>
    public static IServiceCollection AddMMRInfrastructure(this IServiceCollection services)
    {
        // Repository 구현 등록 (In-Memory, MVP)
        services.AddSingleton<IPlayerMMRRepository, InMemoryPlayerMMRRepository>();
        
        // MMR Calculator 등록 (Domain Service)
        services.AddSingleton<IMMRCalculator, EloRatingCalculator>();
        
        return services;
    }
}

