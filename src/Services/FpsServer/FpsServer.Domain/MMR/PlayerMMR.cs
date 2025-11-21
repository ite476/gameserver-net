using DomainMMR = FpsServer.Domain.Matchmaking.MMR;

namespace FpsServer.Domain.MMR;

/// <summary>
/// 플레이어 MMR 엔티티
/// 플레이어의 현재 MMR 값을 관리합니다.
/// </summary>
public class PlayerMMR
{
    /// <summary>
    /// 플레이어 ID
    /// </summary>
    public Guid PlayerId { get; private set; }
    
    /// <summary>
    /// 현재 MMR 값
    /// </summary>
    public DomainMMR CurrentMMR { get; private set; }
    
    /// <summary>
    /// MMR 업데이트 시간
    /// </summary>
    public DateTimeOffset UpdatedAt { get; private set; }
    
    // EF Core용 private 생성자
    private PlayerMMR()
    {
        CurrentMMR = new DomainMMR(1500); // 기본값
    }
    
    /// <summary>
    /// 플레이어 MMR 생성
    /// </summary>
    /// <param name="playerId">플레이어 ID</param>
    /// <param name="initialMMR">초기 MMR 값 (기본값: 1500)</param>
    /// <exception cref="ArgumentException">플레이어 ID가 비어있는 경우</exception>
    public PlayerMMR(Guid playerId, DomainMMR? initialMMR = null)
    {
        if (playerId == Guid.Empty)
            throw new ArgumentException("PlayerId cannot be empty", nameof(playerId));
        
        PlayerId = playerId;
        CurrentMMR = initialMMR ?? new DomainMMR(1500); // 기본 MMR: 1500
        UpdatedAt = DateTimeOffset.UtcNow;
    }
    
    /// <summary>
    /// MMR 업데이트
    /// </summary>
    /// <param name="newMMR">새로운 MMR 값</param>
    /// <exception cref="ArgumentNullException">MMR 값이 null인 경우</exception>
    public void UpdateMMR(DomainMMR newMMR)
    {
        if (newMMR.Value < 0)
            throw new ArgumentException("MMR cannot be negative", nameof(newMMR));
        
        CurrentMMR = newMMR;
        UpdatedAt = DateTimeOffset.UtcNow;
    }
}

