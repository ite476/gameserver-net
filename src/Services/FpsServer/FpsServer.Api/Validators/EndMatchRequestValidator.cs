using FpsServer.Application.MatchSession.DTOs;
using FluentValidation;

namespace FpsServer.Api.Validators;

/// <summary>
/// EndMatchRequest 입력 검증
/// </summary>
public class EndMatchRequestValidator : AbstractValidator<EndMatchRequest>
{
    /// <summary>
    /// 생성자
    /// </summary>
    public EndMatchRequestValidator()
    {
        RuleFor(x => x.MatchId)
            .NotEmpty()
            .WithMessage("MatchId는 필수입니다.");
        
        RuleFor(x => x.Results)
            .NotNull()
            .WithMessage("Results는 필수입니다.")
            .NotEmpty()
            .WithMessage("Results는 최소 1명 이상이어야 합니다.");
        
        RuleForEach(x => x.Results)
            .SetValidator(new PlayerResultDtoValidator());
    }
}

/// <summary>
/// PlayerResultDto 입력 검증
/// </summary>
public class PlayerResultDtoValidator : AbstractValidator<PlayerResultDto>
{
    /// <summary>
    /// 생성자
    /// </summary>
    public PlayerResultDtoValidator()
    {
        RuleFor(x => x.PlayerId)
            .NotEmpty()
            .WithMessage("PlayerId는 필수입니다.");
        
        // Score는 선택 사항이므로 검증하지 않음
        // IsWinner는 bool이므로 자동 검증됨
    }
}

