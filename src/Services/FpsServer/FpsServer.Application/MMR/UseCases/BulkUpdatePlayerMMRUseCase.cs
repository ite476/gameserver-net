using FpsServer.Application.MMR.DTOs;
using FpsServer.Application.MMR.Mappers;
using FpsServer.Application.MMR.Ports;
using FpsServer.Domain.MMR;
using DomainMMR = FpsServer.Domain.Matchmaking.MMR;
using System.Threading;
using System.Threading.Tasks;

namespace FpsServer.Application.MMR.UseCases;

/// <summary>
/// 여러 플레이어 MMR 일괄 업데이트 유스케이스
/// 매치 종료 후 여러 플레이어의 MMR을 동시에 계산하고 업데이트합니다.
/// </summary>
public class BulkUpdatePlayerMMRUseCase
{
    private readonly IPlayerMMRRepository _repository;
    private readonly IMMRCalculator _calculator;

    /// <summary>
    /// 생성자
    /// </summary>
    /// <param name="repository">플레이어 MMR 저장소</param>
    /// <param name="calculator">MMR 계산기</param>
    public BulkUpdatePlayerMMRUseCase(
        IPlayerMMRRepository repository,
        IMMRCalculator calculator)
    {
        _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        _calculator = calculator ?? throw new ArgumentNullException(nameof(calculator));
    }

    /// <summary>
    /// 여러 플레이어 MMR 일괄 업데이트를 처리합니다.
    /// </summary>
    /// <param name="request">일괄 MMR 업데이트 요청</param>
    /// <param name="cancellationToken">취소 토큰</param>
    /// <returns>일괄 MMR 업데이트 응답</returns>
    public async Task<BulkUpdateMMRResponse> ExecuteAsync(
        BulkUpdateMMRRequest request,
        CancellationToken cancellationToken = default)
    {
        // 1. 모든 플레이어 ID 추출
        var playerIds = request.PlayerUpdates.Select(p => p.PlayerId).ToList();
        
        // 2. 기존 플레이어 MMR 목록 조회
        var existingPlayerMMRs = await _repository.FindMultipleByPlayerIdsAsync(playerIds, cancellationToken);
        var playerMMRMap = existingPlayerMMRs.ToDictionary(p => p.PlayerId);
        
        // 3. 각 플레이어별로 MMR 계산 및 업데이트
        var updatedPlayerMMRs = new List<PlayerMMR>();
        var results = new List<UpdateMMRResponse>();
        
        foreach (var updateInfo in request.PlayerUpdates)
        {
            // 기존 MMR 조회 또는 생성
            if (!playerMMRMap.TryGetValue(updateInfo.PlayerId, out var playerMMR))
            {
                playerMMR = new PlayerMMR(updateInfo.PlayerId);
                playerMMRMap[updateInfo.PlayerId] = playerMMR;
            }
            
            var oldMMR = playerMMR.CurrentMMR.Value;
            
            // 상대방 평균 MMR 값 객체 생성
            var opponentAverageMMR = MMRMapper.ToDomainMMR(updateInfo);
            
            // 실제 점수 계산 (1.0: 승리, 0.0: 패배)
            var actualScore = updateInfo.IsWinner ? 1.0 : 0.0;
            
            // 새로운 MMR 계산
            var newMMR = _calculator.CalculateNewMMR(
                playerMMR.CurrentMMR,
                opponentAverageMMR,
                actualScore,
                request.KFactor);
            
            // MMR 업데이트
            playerMMR.UpdateMMR(newMMR);
            
            updatedPlayerMMRs.Add(playerMMR);
            results.Add(MMRMapper.ToUpdateMMRResponse(playerMMR, oldMMR));
        }
        
        // 4. 일괄 저장
        await _repository.SaveMultipleAsync(updatedPlayerMMRs, cancellationToken);
        
        // 5. 응답 반환
        return new BulkUpdateMMRResponse
        {
            Results = results
        };
    }
}

