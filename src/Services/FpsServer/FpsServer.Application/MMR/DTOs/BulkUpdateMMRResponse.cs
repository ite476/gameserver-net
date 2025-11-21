namespace FpsServer.Application.MMR.DTOs;

/// <summary>
/// 여러 플레이어 MMR 일괄 업데이트 응답 DTO
/// </summary>
public record BulkUpdateMMRResponse
{
    /// <summary>
    /// 업데이트된 플레이어별 결과 목록
    /// </summary>
    public required IReadOnlyList<UpdateMMRResponse> Results { get; init; }
}

