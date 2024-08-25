using ErrorOr;
using MediatR;
using TodoApp.Application.Services;
using TodoApp.Domain.TodoAggregate;

namespace TodoApp.Application.TodoCQRS.Commands;

public sealed class UpdateTodoCommandHandler(ITodoService service) : IRequestHandler<UpdateTodoCommand, ErrorOr<Todo>>
{
    public Task<ErrorOr<Todo>> Handle(UpdateTodoCommand request, CancellationToken cancellationToken)
    {
        return service.UpdateTodoAsync(request.Identifier, request.Title, request.Description);

    }
}