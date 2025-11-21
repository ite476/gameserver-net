using DomainMMR = FpsServer.Domain.Matchmaking.MMR;

namespace FpsServer.Domain.MMR;

/// <summary>
/// MMR 계산기 인터페이스
/// 다양한 MMR 계산 알고리즘을 지원하기 위한 인터페이스입니다.
/// MVP에서는 EloRatingCalculator만 제공하며, 향후 다른 알고리즘(Glicko, TrueSkill 등) 추가 가능합니다.
/// </summary>
public interface IMMRCalculator
{
    /// <summary>
    /// 새로운 MMR 계산
    /// </summary>
    /// <param name="currentMMR">현재 MMR</param>
    /// <param name="opponentAverageMMR">상대방 평균 MMR (팀 매칭의 경우 팀 평균)</param>
    /// <param name="actualScore">실제 점수 (1.0: 승리, 0.5: 무승부, 0.0: 패배)</param>
    /// <param name="kFactor">K 값 (기본값: 32, 조정 가능)</param>
    /// <returns>계산된 새로운 MMR</returns>
    DomainMMR CalculateNewMMR(
        DomainMMR currentMMR,
        DomainMMR opponentAverageMMR,
        double actualScore,
        int kFactor = 32);
}

