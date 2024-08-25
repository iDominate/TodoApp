using FluentValidation;
using TodoApp.Application.Auth.Commands;

namespace TodoApp.Application.Auth.Validators;

public sealed class LoginCommandValidator : AbstractValidator<LoginCommand>
{
    public LoginCommandValidator()
    {
        RuleFor(l => l.Username).NotEmpty()
        .WithMessage("Invalid credentials");

        RuleFor(l => l.Password).NotEmpty()
        .WithMessage("Invalid credentials");
    }
}