using ErrorOr;
using MediatR;
using TodoApp.Application.Services;
using TodoApp.Domain.TodoAggregate;

namespace TodoApp.Application.TodoCQRS.Commands;

public sealed class CreateTodoQueryHandler(ITodoService service) : IRequestHandler<CreateTodoCommand, ErrorOr<Todo>>
{
    public async Task<ErrorOr<Todo>> Handle(CreateTodoCommand request, CancellationToken cancellationToken)
    {
        return await service.CreateTodoAsync(request.Title, request.Desciption);
    }
}