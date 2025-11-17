using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using FpsServer.Application.Matchmaking.Ports;
using FpsServer.Domain.Matchmaking;

namespace FpsServer.Infrastructure.Matchmaking;

/// <summary>
/// 매치메이킹 백그라운드 서비스
/// 주기적으로 큐를 스캔하여 매칭을 시도합니다.
/// </summary>
public class MatchmakingBackgroundService : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<MatchmakingBackgroundService> _logger;
    private readonly TimeSpan _scanInterval = TimeSpan.FromSeconds(1); // 1초 주기
    
    /// <summary>
    /// 생성자
    /// </summary>
    /// <param name="serviceProvider">서비스 프로바이더 (Repository, DomainService, Notifier 주입용)</param>
    /// <param name="logger">로거</param>
    public MatchmakingBackgroundService(
        IServiceProvider serviceProvider,
        ILogger<MatchmakingBackgroundService> logger)
    {
        _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }
    
    /// <summary>
    /// 백그라운드 작업 실행
    /// </summary>
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("MatchmakingBackgroundService started");
        
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await ProcessMatchmakingAsync(stoppingToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while processing matchmaking");
            }
            
            await Task.Delay(_scanInterval, stoppingToken);
        }
        
        _logger.LogInformation("MatchmakingBackgroundService stopped");
    }
    
    /// <summary>
    /// 매칭 처리
    /// </summary>
    private async Task ProcessMatchmakingAsync(CancellationToken cancellationToken)
    {
        using var scope = _serviceProvider.CreateScope();
        var repository = scope.ServiceProvider.GetRequiredService<IMatchmakingRepository>();
        var notifier = scope.ServiceProvider.GetRequiredService<IMatchmakingNotifier>();
        var domainService = scope.ServiceProvider.GetRequiredService<MatchmakingDomainService>();
        
        // 모든 게임 모드에 대해 매칭 시도
        foreach (MatchmakingMode gameMode in Enum.GetValues<MatchmakingMode>())
        {
            try
            {
                var queue = await repository.GetOrCreateQueueAsync(gameMode, cancellationToken);
                
                // 큐에 플레이어가 2명 이상인 경우에만 매칭 시도
                if (queue.Requests.Count < 2)
                    continue;
                
                // 매칭 시도
                var match = domainService.TryMatch(queue);
                
                if (match != null)
                {
                    // 매칭 성공 시 큐에서 제거
                    var requestIds = match.Players.Select(p => p.RequestId).ToList();
                    await repository.DequeueAsync(queue, requestIds, cancellationToken);
                    
                    // 매칭 성공 알림 전송
                    await notifier.NotifyMatchFoundAsync(match, cancellationToken);
                    
                    _logger.LogInformation(
                        "Match found: MatchId={MatchId}, GameMode={GameMode}, PlayerCount={PlayerCount}",
                        match.MatchId, match.GameMode, match.Players.Count);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing matchmaking for GameMode={GameMode}", gameMode);
            }
        }
    }
}

