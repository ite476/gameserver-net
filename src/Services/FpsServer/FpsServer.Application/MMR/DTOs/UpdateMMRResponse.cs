namespace FpsServer.Application.MMR.DTOs;

/// <summary>
/// MMR 업데이트 응답 DTO
/// </summary>
public record UpdateMMRResponse
{
    /// <summary>
    /// 플레이어 ID
    /// </summary>
    public required Guid PlayerId { get; init; }
    
    /// <summary>
    /// 이전 MMR 값
    /// </summary>
    public required int OldMMR { get; init; }
    
    /// <summary>
    /// 새로운 MMR 값
    /// </summary>
    public required int NewMMR { get; init; }
}

