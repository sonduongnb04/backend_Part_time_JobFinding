using FluentValidation;
using PTJ.Application.DTOs.Requests.Auth;

namespace PTJ.Application.Validators.Auth;

public class RegisterRequestValidator : AbstractValidator<RegisterRequest>
{
    public RegisterRequestValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email là bắt buộc")
            .EmailAddress().WithMessage("Email không hợp lệ")
            .MaximumLength(256).WithMessage("Email không được vượt quá 256 ký tự");

        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("Mật khẩu là bắt buộc")
            .MinimumLength(8).WithMessage("Mật khẩu phải có ít nhất 8 ký tự")
            .MaximumLength(100).WithMessage("Mật khẩu không được vượt quá 100 ký tự");

        RuleFor(x => x.FullName)
            .MaximumLength(200).WithMessage("Tên không được vượt quá 200 ký tự")
            .When(x => !string.IsNullOrEmpty(x.FullName));

        RuleFor(x => x.PhoneNumber)
            .MaximumLength(32).WithMessage("Số điện thoại không được vượt quá 32 ký tự")
            .Matches(@"^[0-9+\-\s()]+$").WithMessage("Số điện thoại không hợp lệ")
            .When(x => !string.IsNullOrEmpty(x.PhoneNumber));
    }
}
