using FpsServer.Application.MMR.DTOs;
using FpsServer.Domain.MMR;
using DomainMMR = FpsServer.Domain.Matchmaking.MMR;

namespace FpsServer.Application.MMR.Mappers;

/// <summary>
/// MMR DTO ↔ Domain 변환 Mapper
/// </summary>
public static class MMRMapper
{
    /// <summary>
    /// UpdateMMRRequest → DomainMMR 변환 (상대방 평균 MMR)
    /// </summary>
    public static DomainMMR ToDomainMMR(UpdateMMRRequest request)
    {
        return new DomainMMR(request.OpponentAverageMMR);
    }
    
    /// <summary>
    /// PlayerMMR → UpdateMMRResponse 변환
    /// </summary>
    public static UpdateMMRResponse ToUpdateMMRResponse(PlayerMMR playerMMR, int oldMMR)
    {
        return new UpdateMMRResponse
        {
            PlayerId = playerMMR.PlayerId,
            OldMMR = oldMMR,
            NewMMR = playerMMR.CurrentMMR.Value
        };
    }
    
    /// <summary>
    /// PlayerMMRUpdateInfo → DomainMMR 변환 (상대방 평균 MMR)
    /// </summary>
    public static DomainMMR ToDomainMMR(PlayerMMRUpdateInfo updateInfo)
    {
        return new DomainMMR(updateInfo.OpponentAverageMMR);
    }
}

