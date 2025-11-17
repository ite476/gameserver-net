namespace FpsServer.Domain.Matchmaking;

/// <summary>
/// 매치메이킹 도메인 서비스
/// MMR 기반 매칭 로직을 수행합니다.
/// </summary>
public class MatchmakingDomainService
{
    private const int MMR_TOLERANCE = 100; // MMR 허용 범위 (±100)
    private const int REQUIRED_PLAYERS = 2; // MVP는 2명 매칭 (Solo 기준)
    
    /// <summary>
    /// 큐에서 매칭 가능한 플레이어 그룹을 찾아 Match를 생성합니다.
    /// </summary>
    /// <param name="queue">매치메이킹 큐</param>
    /// <returns>매칭 성공 시 Match 엔티티, 실패 시 null</returns>
    /// <exception cref="ArgumentNullException">큐가 null인 경우</exception>
    public Match? TryMatch(MatchmakingQueue queue)
    {
        if (queue == null)
            throw new ArgumentNullException(nameof(queue));
        
        var requests = queue.Requests.OrderBy(r => r.EnqueuedAt).ToList();
        
        // MMR 기반 매칭 로직
        var matchedPlayers = FindMatchingPlayers(requests);
        
        if (matchedPlayers.Count < REQUIRED_PLAYERS)
            return null;
        
        // 매칭된 플레이어들을 큐에서 제거
        foreach (var player in matchedPlayers)
        {
            queue.Cancel(player.PlayerId);
        }
        
        return new Match(queue.GameMode, matchedPlayers);
    }
    
    /// <summary>
    /// MMR 기반으로 매칭 가능한 플레이어 그룹을 찾습니다.
    /// </summary>
    /// <param name="requests">큐 진입 시간순으로 정렬된 플레이어 요청 목록</param>
    /// <returns>매칭 가능한 플레이어 그룹</returns>
    private List<PlayerMatchRequest> FindMatchingPlayers(List<PlayerMatchRequest> requests)
    {
        if (requests.Count < REQUIRED_PLAYERS)
            return new List<PlayerMatchRequest>();
        
        var matched = new List<PlayerMatchRequest>();
        var firstPlayer = requests[0];
        matched.Add(firstPlayer);
        
        // 첫 번째 플레이어의 MMR 기준으로 허용 범위 내 플레이어 찾기
        for (int i = 1; i < requests.Count && matched.Count < REQUIRED_PLAYERS; i++)
        {
            var candidate = requests[i];
            var mmrDiff = MMR.AbsoluteDifference(candidate.PlayerMMR, firstPlayer.PlayerMMR);
            
            if (mmrDiff <= MMR_TOLERANCE)
            {
                matched.Add(candidate);
            }
        }
        
        return matched.Count >= REQUIRED_PLAYERS ? matched : new List<PlayerMatchRequest>();
    }
}

