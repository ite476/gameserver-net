using Microsoft.Extensions.DependencyInjection;
using FpsServer.Application.MatchSession.Ports;

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
    /// <returns>서비스 컬렉션 (체이닝 지원)</returns>
    public static IServiceCollection AddMatchSessionInfrastructure(this IServiceCollection services)
    {
        // Repository 구현 등록 (In-Memory, MVP)
        services.AddSingleton<IMatchSessionRepository, InMemoryMatchSessionRepository>();
        
        return services;
    }
}

