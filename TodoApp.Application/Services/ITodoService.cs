using System.Collections.Immutable;
using ErrorOr;
using TodoApp.Domain.TodoAggregate;

namespace TodoApp.Application.Services;

public interface ITodoService
{
    public Task<IImmutableList<Todo>> GetTodosAsync();
    public Task<ErrorOr<Todo>> GetTodoAsync(string Identifer);
    public Task<ErrorOr<Todo>> CreateTodoAsync(string title, string description);
    public Task<ErrorOr<Todo>> UpdateTodoAsync(string identifier, string title, string description);
    public Task<ErrorOr<string>> DeleteTodoAsync(string identifier);

}