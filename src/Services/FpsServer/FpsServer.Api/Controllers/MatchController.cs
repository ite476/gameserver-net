using FpsServer.Application.MatchSession.DTOs;
using FpsServer.Application.MatchSession.UseCases;
using FpsServer.Application.MMR.DTOs;
using FpsServer.Application.MMR.UseCases;
using FpsServer.Application.MMR.Ports;
using FpsServer.Domain.Matchmaking;
using FpsServer.Domain.MatchSession.Exceptions;
using Microsoft.AspNetCore.Mvc;

namespace FpsServer.Api.Controllers;

/// <summary>
/// 매치 세션 API 컨트롤러
/// </summary>
[ApiController]
[Route("api/fps/matches")]
public class MatchController : ControllerBase
{
    private readonly StartMatchUseCase _startMatchUseCase;
    private readonly EndMatchUseCase _endMatchUseCase;
    private readonly UpdatePlayerMMRUseCase _updatePlayerMMRUseCase;
    private readonly IPlayerMMRRepository _playerMMRRepository;
    
    /// <summary>
    /// 생성자
    /// </summary>
    public MatchController(
        StartMatchUseCase startMatchUseCase,
        EndMatchUseCase endMatchUseCase,
        UpdatePlayerMMRUseCase updatePlayerMMRUseCase,
        IPlayerMMRRepository playerMMRRepository)
    {
        _startMatchUseCase = startMatchUseCase ?? throw new ArgumentNullException(nameof(startMatchUseCase));
        _endMatchUseCase = endMatchUseCase ?? throw new ArgumentNullException(nameof(endMatchUseCase));
        _updatePlayerMMRUseCase = updatePlayerMMRUseCase ?? throw new ArgumentNullException(nameof(updatePlayerMMRUseCase));
        _playerMMRRepository = playerMMRRepository ?? throw new ArgumentNullException(nameof(playerMMRRepository));
    }
    
    /// <summary>
    /// 매치 시작
    /// </summary>
    /// <param name="matchId">매치 ID</param>
    /// <param name="request">매치 시작 요청</param>
    /// <param name="gameMode">게임 모드 (쿼리 파라미터)</param>
    /// <param name="cancellationToken">취소 토큰</param>
    /// <returns>매치 시작 응답</returns>
    [HttpPost("{matchId}/start")]
    [ProducesResponseType(typeof(StartMatchResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> StartMatch(
        [FromRoute] Guid matchId,
        [FromBody] StartMatchRequest request,
        [FromQuery] MatchmakingMode gameMode = MatchmakingMode.Solo,
        CancellationToken cancellationToken = default)
    {
        try
        {
            // Route의 matchId와 Body의 MatchId 일치 확인
            if (request.MatchId != matchId)
            {
                return BadRequest(new { error = "Route의 matchId와 Body의 MatchId가 일치하지 않습니다." });
            }
            
            var response = await _startMatchUseCase.ExecuteAsync(request, gameMode, cancellationToken);
            return Ok(response);
        }
        catch (MatchSessionNotFoundException ex)
        {
            return NotFound(new { error = ex.Message });
        }
        catch (InvalidMatchSessionStateException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }
    
    /// <summary>
    /// 매치 종료 (MMR 업데이트 자동 트리거)
    /// </summary>
    /// <param name="matchId">매치 ID</param>
    /// <param name="request">매치 종료 요청</param>
    /// <param name="cancellationToken">취소 토큰</param>
    /// <returns>매치 종료 응답</returns>
    [HttpPost("{matchId}/end")]
    [ProducesResponseType(typeof(EndMatchResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> EndMatch(
        [FromRoute] Guid matchId,
        [FromBody] EndMatchRequest request,
        CancellationToken cancellationToken = default)
    {
        try
        {
            // Route의 matchId와 Body의 MatchId 일치 확인
            if (request.MatchId != matchId)
            {
                return BadRequest(new { error = "Route의 matchId와 Body의 MatchId가 일치하지 않습니다." });
            }
            
            // 1. 매치 종료 처리
            var endMatchResponse = await _endMatchUseCase.ExecuteAsync(request, cancellationToken);
            
            // 2. MMR 업데이트 (동기 호출)
            await UpdatePlayerMMRsAsync(request, cancellationToken);
            
            return Ok(endMatchResponse);
        }
        catch (MatchSessionNotFoundException ex)
        {
            return NotFound(new { error = ex.Message });
        }
        catch (InvalidMatchSessionStateException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }
    
    /// <summary>
    /// 플레이어 MMR 업데이트 (매치 종료 후 자동 호출)
    /// </summary>
    /// <param name="request">매치 종료 요청</param>
    /// <param name="cancellationToken">취소 토큰</param>
    private async Task UpdatePlayerMMRsAsync(
        EndMatchRequest request,
        CancellationToken cancellationToken)
    {
        if (request.Results == null || !request.Results.Any())
            return;
        
        // 1. 모든 플레이어의 현재 MMR 조회
        var playerIds = request.Results.Select(r => r.PlayerId).ToList();
        var playerMMRs = await _playerMMRRepository.FindMultipleByPlayerIdsAsync(playerIds, cancellationToken);
        
        // 2. 플레이어별 MMR 맵 생성 (없으면 기본값 1500)
        var playerMMRMap = playerMMRs.ToDictionary(
            pm => pm.PlayerId,
            pm => pm.CurrentMMR.Value);
        
        foreach (var playerId in playerIds)
        {
            if (!playerMMRMap.ContainsKey(playerId))
                playerMMRMap[playerId] = 1500; // 기본 MMR
        }
        
        // 3. 승리 팀과 패배 팀 구분
        var winners = request.Results.Where(r => r.IsWinner).Select(r => r.PlayerId).ToList();
        var losers = request.Results.Where(r => !r.IsWinner).Select(r => r.PlayerId).ToList();
        
        // 4. 각 플레이어에 대해 상대방 평균 MMR 계산 및 업데이트
        foreach (var result in request.Results)
        {
            // 상대방 팀의 평균 MMR 계산
            var opponentTeam = result.IsWinner ? losers : winners;
            var opponentAverageMMR = opponentTeam.Any()
                ? (int)opponentTeam.Average(pid => playerMMRMap[pid])
                : 1500; // 상대방이 없으면 기본값
            
            // MMR 업데이트 요청 생성
            var updateMMRRequest = new UpdateMMRRequest
            {
                PlayerId = result.PlayerId,
                MatchId = request.MatchId,
                IsWinner = result.IsWinner,
                OpponentAverageMMR = opponentAverageMMR,
                KFactor = 32 // 기본 K 값
            };
            
            // MMR 업데이트 실행
            await _updatePlayerMMRUseCase.ExecuteAsync(updateMMRRequest, cancellationToken);
        }
    }
}

