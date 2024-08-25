using FluentValidation;
using TodoApp.Application.TodoCQRS.Commands;

namespace TodoApp.Application.TodoCQRS.Validators;

public sealed class UpdateTodoValidator : AbstractValidator<UpdateTodoCommand>
{
    public UpdateTodoValidator()
    {
        RuleFor(u => u.Identifier).NotEmpty();
        RuleFor(u => u.Title).NotEmpty();
        RuleFor(u => u.Description).NotEmpty();
    }
}