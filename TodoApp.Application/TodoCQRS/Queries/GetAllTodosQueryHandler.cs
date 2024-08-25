using System.Collections.Immutable;
using MediatR;
using TodoApp.Application.Services;
using TodoApp.Domain.TodoAggregate;

namespace TodoApp.Application.TodoCQRS.Queries;

public sealed class GetAllTodosQueryHandler : IRequestHandler<GetAllTodosQuery, IImmutableList<Todo>>
{
    private readonly ITodoService _service;

    public GetAllTodosQueryHandler(ITodoService service)
    {
        _service = service;
    }

    public async Task<IImmutableList<Todo>> Handle(GetAllTodosQuery request, CancellationToken cancellationToken)
    {
        return await this._service.GetTodosAsync();
    }
}