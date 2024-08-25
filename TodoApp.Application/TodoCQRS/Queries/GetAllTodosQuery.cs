using System.Collections.Immutable;
using MediatR;
using TodoApp.Domain.TodoAggregate;

namespace TodoApp.Application.TodoCQRS.Queries;

public sealed record GetAllTodosQuery() : IRequest<IImmutableList<Todo>>;