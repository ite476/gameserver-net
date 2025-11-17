using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.DependencyInjection;
using FpsServer.Application.Matchmaking.Ports;
using FpsServer.Domain.Matchmaking;

namespace FpsServer.Infrastructure.Matchmaking;

/// <summary>
/// 매치메이킹 Infrastructure 레이어 DI 확장 메서드
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// 매치메이킹 Infrastructure 서비스 등록
    /// </summary>
    /// <param name="services">서비스 컬렉션</param>
    /// <returns>서비스 컬렉션 (체이닝 지원)</returns>
    public static IServiceCollection AddMatchmakingInfrastructure(this IServiceCollection services)
    {
        // Repository 구현 등록
        services.AddSingleton<IMatchmakingRepository, InMemoryMatchmakingRepository>();
        
        // Notifier 구현 등록
        services.AddScoped<IMatchmakingNotifier, SignalRMatchmakingNotifier>();
        
        // Domain Service 등록 (Singleton으로 등록하여 상태 유지)
        services.AddSingleton<MatchmakingDomainService>();
        
        // Background Service 등록
        services.AddHostedService<MatchmakingBackgroundService>();
        
        // SignalR Hub는 API 레이어에서 등록합니다.
        // services.AddSignalR(); // API 레이어에서 등록
        
        return services;
    }
}

