using FluentValidation;
using TodoApp.Application.TodoCQRS.Commands;

namespace TodoApp.Application.TodoCQRS.Validators;

public sealed class CreateTodoValidator : AbstractValidator<CreateTodoCommand>
{
    public CreateTodoValidator()
    {
        RuleFor(c => c.Title).NotEmpty().WithName("Title");
        RuleFor(c => c.Desciption).NotEmpty().WithName("Description");
    }
}