using ErrorOr;
using MediatR;
using TodoApp.Domain.TodoAggregate;

namespace TodoApp.Application.TodoCQRS.Commands;

public sealed record UpdateTodoCommand(string Identifier, string Title, string Description) : IRequest<ErrorOr<Todo>>;