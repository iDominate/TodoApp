using FluentValidation;
using TodoApp.Application.Auth.Commands;

namespace TodoApp.Application.Auth.Validators;

public sealed class RegisterCommadValidator : AbstractValidator<RegisterUserCommand>
{
    public RegisterCommadValidator()
    {
        RuleFor(r => r.Email).NotEmpty().EmailAddress();
        RuleFor(r => r.FirstName).NotEmpty();
        RuleFor(r => r.LastName).NotEmpty();
        RuleFor(r => r.Password).NotEmpty();
        RuleFor(r => r.UserName).NotEmpty();

        RuleFor(r => r.ConfirmPassword).NotEmpty().Equal(r => r.Password)
        .WithMessage("Passwords do not match");
    }
}