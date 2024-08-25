using ErrorOr;
using MediatR;
using TodoApp.Application.Services;

namespace TodoApp.Application.TodoCQRS.Commands;

public sealed class DeleteTodoCommandHandler : IRequestHandler<DeleteTodoCommand, ErrorOr<string>>
{
    private readonly ITodoService service;

    public DeleteTodoCommandHandler(ITodoService service)
    {
        this.service = service;
    }

    public Task<ErrorOr<string>> Handle(DeleteTodoCommand request, CancellationToken cancellationToken)
    {
        return service.DeleteTodoAsync(request.Identifier.ToString());
    }
}