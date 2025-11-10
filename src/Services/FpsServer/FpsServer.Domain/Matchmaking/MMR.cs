namespace FpsServer.Domain.Matchmaking;

/// <summary>
/// Matchmaking Rating 값 객체 (Elo 레이팅 기반)
/// EF Core 8 복합 형식(Complex Type)으로 지원되며, 불변성을 보장합니다.
/// </summary>
/// <remarks>
/// ADR-0001, ADR-0003 참고: Elo 레이팅 시스템 기반
/// </remarks>
public readonly record struct MMR
{
    /// <summary>
    /// MMR 값 (0 이상의 정수)
    /// </summary>
    public int Value { get; init; }
    
    /// <summary>
    /// MMR 값 객체 생성
    /// </summary>
    /// <param name="value">MMR 값 (0 이상)</param>
    /// <exception cref="ArgumentException">MMR 값이 음수인 경우</exception>
    public MMR(int value)
    {
        if (value < 0)
            throw new ArgumentException("MMR cannot be negative", nameof(value));
        Value = value;
    }
    
    /// <summary>
    /// MMR 값에 델타를 더한 새로운 MMR 반환
    /// </summary>
    public static MMR operator +(MMR left, int delta) => new(left.Value + delta);
    
    /// <summary>
    /// MMR 값에서 델타를 뺀 새로운 MMR 반환
    /// </summary>
    public static MMR operator -(MMR left, int delta) => new(left.Value - delta);
    
    /// <summary>
    /// 두 MMR 값의 차이를 반환
    /// </summary>
    public static int operator -(MMR left, MMR right) => left.Value - right.Value;
    
    /// <summary>
    /// MMR 값의 절대 차이를 반환
    /// </summary>
    public static int AbsoluteDifference(MMR left, MMR right) => Math.Abs(left.Value - right.Value);
}

