using Microsoft.AspNetCore.SignalR;
using FpsServer.Application.Matchmaking.Ports;
using FpsServer.Domain.Matchmaking;

namespace FpsServer.Infrastructure.Matchmaking;

/// <summary>
/// 매치메이킹 SignalR Hub
/// 클라이언트 연결 관리 및 매칭 알림 전송에 사용됩니다.
/// 실제 구현은 API 레이어에서 확장할 수 있습니다.
/// </summary>
public class MatchmakingHub : Hub
{
    // Hub는 기본 구현만 사용하며, 
    // 실제 알림은 SignalRMatchmakingNotifier를 통해 전송됩니다.
}

/// <summary>
/// SignalR 기반 매칭 성공 알림 구현
/// SignalR Hub를 통해 클라이언트에 실시간 알림을 전송합니다.
/// </summary>
public class SignalRMatchmakingNotifier : IMatchmakingNotifier
{
    private readonly IHubContext<MatchmakingHub> _hubContext;
    
    /// <summary>
    /// 생성자
    /// </summary>
    /// <param name="hubContext">SignalR Hub Context</param>
    public SignalRMatchmakingNotifier(IHubContext<MatchmakingHub> hubContext)
    {
        _hubContext = hubContext ?? throw new ArgumentNullException(nameof(hubContext));
    }
    
    /// <summary>
    /// 매칭 성공 알림 전송
    /// </summary>
    public async Task NotifyMatchFoundAsync(Match match, CancellationToken cancellationToken = default)
    {
        // 매칭된 모든 플레이어에게 알림 전송
        var tasks = match.Players.Select(player => 
            _hubContext.Clients.User(player.PlayerId.ToString())
                .SendAsync("MatchFound", new
                {
                    MatchId = match.MatchId,
                    Players = match.Players.Select(p => p.PlayerId).ToList(),
                    GameMode = match.GameMode.ToString()
                }, cancellationToken)
        );
        
        await Task.WhenAll(tasks);
    }
}

