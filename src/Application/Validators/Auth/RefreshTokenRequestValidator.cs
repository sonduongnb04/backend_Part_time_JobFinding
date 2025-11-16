using FluentValidation;
using PTJ.Application.DTOs.Requests.Auth;

namespace PTJ.Application.Validators.Auth;

public class RefreshTokenRequestValidator : AbstractValidator<RefreshTokenRequest>
{
    public RefreshTokenRequestValidator()
    {
        RuleFor(x => x.RefreshToken)
            .NotEmpty().WithMessage("Refresh token là bắt buộc");
    }
}
