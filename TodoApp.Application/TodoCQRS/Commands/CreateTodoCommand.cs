using ErrorOr;
using MediatR;
using TodoApp.Domain.TodoAggregate;

namespace TodoApp.Application.TodoCQRS.Commands;

public sealed record CreateTodoCommand(string Title, string Desciption) : IRequest<ErrorOr<Todo>>;