using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.DependencyInjection;
using FpsServer.Application.Chat.Ports;
using FpsServer.Domain.Chat;

namespace FpsServer.Infrastructure.Chat;

/// <summary>
/// 채팅 Infrastructure 레이어 DI 확장 메서드
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// 채팅 Infrastructure 서비스 등록
    /// </summary>
    /// <param name="services">서비스 컬렉션</param>
    /// <returns>서비스 컬렉션 (체이닝 지원)</returns>
    public static IServiceCollection AddChatInfrastructure(this IServiceCollection services)
    {
        // Repository 구현 등록
        services.AddSingleton<IChatRepository, InMemoryChatRepository>();
        
        // Notifier 구현 등록
        services.AddScoped<IChatNotifier, SignalRChatNotifier>();
        
        // Domain Service 등록 (Singleton으로 등록하여 상태 유지)
        services.AddSingleton<ChatDomainService>();
        
        // SignalR Hub는 API 레이어에서 등록합니다.
        // services.AddSignalR(); // API 레이어에서 등록
        
        return services;
    }
}

