using FpsServer.Application.Matchmaking.DTOs;
using FluentValidation;

namespace FpsServer.Api.Validators;

/// <summary>
/// JoinMatchmakingRequest 입력 검증
/// </summary>
public class JoinMatchmakingRequestValidator : AbstractValidator<JoinMatchmakingRequest>
{
    /// <summary>
    /// 생성자
    /// </summary>
    public JoinMatchmakingRequestValidator()
    {
        RuleFor(x => x.PlayerId)
            .NotEmpty()
            .WithMessage("PlayerId는 필수입니다.");
        
        RuleFor(x => x.GameMode)
            .IsInEnum()
            .WithMessage("유효한 게임 모드를 입력해주세요.");
        
        RuleFor(x => x.MMR)
            .GreaterThanOrEqualTo(0)
            .WithMessage("MMR은 0 이상이어야 합니다.");
    }
}

