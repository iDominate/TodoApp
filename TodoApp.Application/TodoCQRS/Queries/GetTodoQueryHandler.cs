using ErrorOr;
using MediatR;
using TodoApp.Application.Services;
using TodoApp.Domain.TodoAggregate;

namespace TodoApp.Application.TodoCQRS.Queries;

public sealed class GetTodoQueryHandler(ITodoService service) : IRequestHandler<GetTodoByIdQuery, ErrorOr<Todo>>
{
    public Task<ErrorOr<Todo>> Handle(GetTodoByIdQuery request, CancellationToken cancellationToken)
    {
        return service.GetTodoAsync(request.Identifier.ToString());
    }
}