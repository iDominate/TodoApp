using FluentValidation;
using TodoApp.Application.TodoCQRS.Queries;

namespace TodoApp.Application.TodoCQRS.Validators;

public sealed class GetTodoValidator : AbstractValidator<GetTodoByIdQuery>
{
    public GetTodoValidator()
    {
        RuleFor(g => g.Identifier).NotEmpty();
    }
}