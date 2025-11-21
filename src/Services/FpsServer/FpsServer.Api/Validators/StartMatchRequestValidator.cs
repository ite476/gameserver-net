using FpsServer.Application.MatchSession.DTOs;
using FluentValidation;

namespace FpsServer.Api.Validators;

/// <summary>
/// StartMatchRequest 입력 검증
/// </summary>
public class StartMatchRequestValidator : AbstractValidator<StartMatchRequest>
{
    /// <summary>
    /// 생성자
    /// </summary>
    public StartMatchRequestValidator()
    {
        RuleFor(x => x.MatchId)
            .NotEmpty()
            .WithMessage("MatchId는 필수입니다.");
        
        RuleFor(x => x.PlayerIds)
            .NotNull()
            .WithMessage("PlayerIds는 필수입니다.")
            .NotEmpty()
            .WithMessage("PlayerIds는 최소 1명 이상이어야 합니다.");
        
        RuleForEach(x => x.PlayerIds)
            .NotEmpty()
            .WithMessage("PlayerId는 빈 값일 수 없습니다.");
    }
}

