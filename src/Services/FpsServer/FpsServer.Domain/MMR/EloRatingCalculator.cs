using DomainMMR = FpsServer.Domain.Matchmaking.MMR;

namespace FpsServer.Domain.MMR;

/// <summary>
/// Elo 레이팅 시스템 기반 MMR 계산기
/// 
/// Elo 레이팅 시스템은 체스 레이팅 시스템으로 널리 사용되며, 게임 매칭 시스템에서도 표준으로 사용됩니다.
/// 
/// 공식:
/// - Expected Score = 1 / (1 + 10^((Opponent MMR - Player MMR) / 400))
/// - New MMR = Old MMR + K * (Actual Score - Expected Score)
/// 
/// 참고 자료:
/// - [엘로 평점 시스템 (위키백과)](https://ko.wikipedia.org/wiki/엘로_평점_시스템)
/// - [Elo 레이팅 (나무위키)](https://namu.wiki/w/Elo%20레이팅)
/// </summary>
public class EloRatingCalculator : IMMRCalculator
{
    /// <summary>
    /// 새로운 MMR 계산
    /// </summary>
    /// <param name="currentMMR">현재 MMR</param>
    /// <param name="opponentAverageMMR">상대방 평균 MMR (팀 매칭의 경우 팀 평균)</param>
    /// <param name="actualScore">실제 점수 (1.0: 승리, 0.5: 무승부, 0.0: 패배)</param>
    /// <param name="kFactor">K 값 (기본값: 32, 조정 가능)</param>
    /// <returns>계산된 새로운 MMR</returns>
    /// <exception cref="ArgumentException">actualScore가 0.0~1.0 범위를 벗어나거나, kFactor가 0 이하인 경우</exception>
    public DomainMMR CalculateNewMMR(
        DomainMMR currentMMR,
        DomainMMR opponentAverageMMR,
        double actualScore,
        int kFactor = 32)
    {
        if (actualScore < 0.0 || actualScore > 1.0)
            throw new ArgumentException("Actual score must be between 0.0 and 1.0", nameof(actualScore));
        
        if (kFactor <= 0)
            throw new ArgumentException("K factor must be greater than 0", nameof(kFactor));
        
        // Expected Score 계산: 1 / (1 + 10^((Opponent MMR - Player MMR) / 400))
        var ratingDifference = opponentAverageMMR.Value - currentMMR.Value;
        var expectedScore = 1.0 / (1.0 + Math.Pow(10, ratingDifference / 400.0));
        
        // New MMR 계산: Old MMR + K * (Actual Score - Expected Score)
        var mmrChange = (int)Math.Round(kFactor * (actualScore - expectedScore));
        var newMMRValue = currentMMR.Value + mmrChange;
        
        // MMR은 0 이상이어야 함
        if (newMMRValue < 0)
            newMMRValue = 0;
        
        return new DomainMMR(newMMRValue);
    }
}

