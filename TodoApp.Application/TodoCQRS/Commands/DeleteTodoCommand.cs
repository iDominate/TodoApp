using ErrorOr;
using MediatR;

namespace TodoApp.Application.TodoCQRS.Commands;

public sealed record DeleteTodoCommand(string Identifier) : IRequest<ErrorOr<string>>;