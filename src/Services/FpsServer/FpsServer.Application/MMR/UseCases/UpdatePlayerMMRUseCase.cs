using FpsServer.Application.MMR.DTOs;
using FpsServer.Application.MMR.Mappers;
using FpsServer.Application.MMR.Ports;
using FpsServer.Domain.MMR;
using DomainMMR = FpsServer.Domain.Matchmaking.MMR;
using System.Threading;
using System.Threading.Tasks;

namespace FpsServer.Application.MMR.UseCases;

/// <summary>
/// 플레이어 MMR 업데이트 유스케이스
/// 매치 종료 후 승/패 결과에 따라 플레이어의 MMR을 계산하고 업데이트합니다.
/// </summary>
public class UpdatePlayerMMRUseCase
{
    private readonly IPlayerMMRRepository _repository;
    private readonly IMMRCalculator _calculator;

    /// <summary>
    /// 생성자
    /// </summary>
    /// <param name="repository">플레이어 MMR 저장소</param>
    /// <param name="calculator">MMR 계산기</param>
    public UpdatePlayerMMRUseCase(
        IPlayerMMRRepository repository,
        IMMRCalculator calculator)
    {
        _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        _calculator = calculator ?? throw new ArgumentNullException(nameof(calculator));
    }

    /// <summary>
    /// 플레이어 MMR 업데이트를 처리합니다.
    /// </summary>
    /// <param name="request">MMR 업데이트 요청</param>
    /// <param name="cancellationToken">취소 토큰</param>
    /// <returns>MMR 업데이트 응답</returns>
    public async Task<UpdateMMRResponse> ExecuteAsync(
        UpdateMMRRequest request,
        CancellationToken cancellationToken = default)
    {
        // 1. 기존 플레이어 MMR 조회 또는 생성
        var playerMMR = await _repository.FindByPlayerIdAsync(request.PlayerId, cancellationToken);
        
        if (playerMMR == null)
        {
            // 플레이어가 없으면 초기 MMR로 생성
            playerMMR = new PlayerMMR(request.PlayerId);
        }
        
        var oldMMR = playerMMR.CurrentMMR.Value;
        
        // 2. 상대방 평균 MMR 값 객체 생성
        var opponentAverageMMR = MMRMapper.ToDomainMMR(request);
        
        // 3. 실제 점수 계산 (1.0: 승리, 0.0: 패배)
        var actualScore = request.IsWinner ? 1.0 : 0.0;
        
        // 4. 새로운 MMR 계산
        var newMMR = _calculator.CalculateNewMMR(
            playerMMR.CurrentMMR,
            opponentAverageMMR,
            actualScore,
            request.KFactor);
        
        // 5. MMR 업데이트
        playerMMR.UpdateMMR(newMMR);
        
        // 6. 저장
        await _repository.SaveAsync(playerMMR, cancellationToken);
        
        // 7. 응답 반환
        return MMRMapper.ToUpdateMMRResponse(playerMMR, oldMMR);
    }
}

