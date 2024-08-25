using FluentValidation;
using TodoApp.Application.Auth.Commands;

namespace TodoApp.Application.Auth.Validators;

public sealed class GenerateTokenAsyncCommandValidator : AbstractValidator<GenerateTokenAsyncCommand>
{
    public GenerateTokenAsyncCommandValidator()
    {
        RuleFor(g => g.Token).NotEmpty().WithMessage("Token is required");
    }
}