using FpsServer.Application.Matchmaking.DTOs;
using FpsServer.Application.Matchmaking.UseCases;
using FpsServer.Domain.Matchmaking;
using FpsServer.Domain.Matchmaking.Exceptions;
using Microsoft.AspNetCore.Mvc;

namespace FpsServer.Api.Controllers;

/// <summary>
/// 매치메이킹 API 컨트롤러
/// </summary>
[ApiController]
[Route("api/fps/matchmaking")]
public class MatchmakingController : ControllerBase
{
    private readonly JoinMatchmakingQueueUseCase _joinUseCase;
    private readonly CancelMatchmakingUseCase _cancelUseCase;
    
    /// <summary>
    /// 생성자
    /// </summary>
    public MatchmakingController(
        JoinMatchmakingQueueUseCase joinUseCase,
        CancelMatchmakingUseCase cancelUseCase)
    {
        _joinUseCase = joinUseCase ?? throw new ArgumentNullException(nameof(joinUseCase));
        _cancelUseCase = cancelUseCase ?? throw new ArgumentNullException(nameof(cancelUseCase));
    }
    
    /// <summary>
    /// 매치메이킹 큐에 진입
    /// </summary>
    /// <param name="request">큐 진입 요청</param>
    /// <param name="cancellationToken">취소 토큰</param>
    /// <returns>큐 진입 응답</returns>
    [HttpPost("join")]
    [ProducesResponseType(typeof(JoinMatchmakingResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Join(
        [FromBody] JoinMatchmakingRequest request,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var response = await _joinUseCase.ExecuteAsync(request, cancellationToken);
            return Ok(response);
        }
        catch (PlayerAlreadyInQueueException ex)
        {
            return Conflict(new { error = ex.Message });
        }
        catch (InvalidMatchmakingRequestException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }
    
    /// <summary>
    /// 매치메이킹 큐에서 취소
    /// </summary>
    /// <param name="gameMode">게임 모드</param>
    /// <param name="playerId">플레이어 ID</param>
    /// <param name="cancellationToken">취소 토큰</param>
    /// <returns>204 No Content</returns>
    [HttpDelete("cancel")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Cancel(
        [FromQuery] MatchmakingMode gameMode,
        [FromQuery] Guid playerId,
        CancellationToken cancellationToken = default)
    {
        try
        {
            await _cancelUseCase.ExecuteAsync(gameMode, playerId, cancellationToken);
            return NoContent();
        }
        catch (PlayerNotInQueueException ex)
        {
            return NotFound(new { error = ex.Message });
        }
    }
    
    /// <summary>
    /// 매치메이킹 큐 상태 조회
    /// </summary>
    /// <param name="gameMode">게임 모드</param>
    /// <param name="playerId">플레이어 ID (선택)</param>
    /// <returns>큐 상태 정보</returns>
    [HttpGet("status")]
    [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
    public IActionResult GetStatus(
        [FromQuery] MatchmakingMode gameMode,
        [FromQuery] Guid? playerId = null)
    {
        // TODO: 큐 상태 조회 로직 구현 (선택 사항)
        // 현재는 기본 응답만 반환
        return Ok(new
        {
            gameMode = gameMode.ToString(),
            playerId = playerId,
            message = "Queue status endpoint - implementation pending"
        });
    }
}

