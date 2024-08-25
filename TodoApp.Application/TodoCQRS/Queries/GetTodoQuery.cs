using ErrorOr;
using MediatR;
using TodoApp.Domain.TodoAggregate;

namespace TodoApp.Application.TodoCQRS.Queries;

public sealed record GetTodoByIdQuery(string Identifier) : IRequest<ErrorOr<Todo>>;