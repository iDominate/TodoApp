using System.Collections.Immutable;
using ErrorOr;
using Microsoft.EntityFrameworkCore;
using TodoApp.Application.Services;
using TodoApp.Domain.TodoAggregate;
using TodoApp.Domain.TodoAggregate.ValueObjects;
using TodoApp.Infrastructure.Context;

namespace TodoApp.Infrastructure.TodoRepository;


public sealed class TodoService : ITodoService
{
    private readonly AppDbContext _context;

    public TodoService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<ErrorOr<Todo>> CreateTodoAsync(string title, string description)
    {
        var todo = new Todo(TodoId.CreateUnique(null), title, description, Guid.NewGuid().ToString());
        await this._context.Todos.AddAsync(todo);
        await this._context.SaveChangesAsync();
        return todo;
    }

    public async Task<ErrorOr<string>> DeleteTodoAsync(string identifier)
    {
        var existingTodo = await this._context.Todos
        .FirstOrDefaultAsync(t => Guid.Equals(t.Identifier, identifier));
        if (existingTodo is null)
        {
            return Error.NotFound("Todo.Not_Found", $"A Todo with identifier: {identifier} was not found");
        }
        this._context.Todos.Remove(existingTodo);
        await this._context.SaveChangesAsync();
        return existingTodo.Identifier;
    }

    public async Task<ErrorOr<Todo>> GetTodoAsync(string Identifer)
    {
        var existingTodo = await this._context.Todos.AsQueryable().
        AsNoTracking().FirstOrDefaultAsync(t => t.Identifier == Identifer);

        if (existingTodo is null)
        {
            return Error.NotFound("Todo.Not_Found", $"A Todo with identifier: {Identifer} was not found");
        }
        return existingTodo;
    }

    public async Task<IImmutableList<Domain.TodoAggregate.Todo>> GetTodosAsync()
    {
        return (await this._context.Todos.AsQueryable().AsNoTracking().ToListAsync()).ToImmutableList();
    }

    public async Task<ErrorOr<Todo>> UpdateTodoAsync(string identifier, string title, string description)
    {
        var existingTodo = await this._context.Todos.FirstOrDefaultAsync(t => t.Identifier == identifier);


        if (existingTodo is null)
        {

            return Error.NotFound("Todo.Not_Found", $"A Todo with identifier: {identifier} was not found");
        }
        existingTodo.ChangeDescription(description);
        existingTodo.ChangeTitle(title);
        await this._context.SaveChangesAsync();
        return existingTodo;

    }
}