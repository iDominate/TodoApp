using FluentValidation;
using TodoApp.Application.TodoCQRS.Commands;

namespace TodoApp.Application.TodoCQRS.Validators;

public sealed class DeleteTodoValidator : AbstractValidator<DeleteTodoCommand>
{
    public DeleteTodoValidator()
    {
        RuleFor(d => d.Identifier).NotEmpty();
    }
}