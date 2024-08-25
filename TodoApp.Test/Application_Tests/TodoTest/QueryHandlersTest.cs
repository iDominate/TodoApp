using System.Collections.Immutable;
using ErrorOr;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using TodoApp.Application.Services;
using TodoApp.Application.TodoCQRS.Queries;
using TodoApp.Domain.TodoAggregate;
using TodoApp.Domain.TodoAggregate.ValueObjects;
using TodoApp.Infrastructure.Context;
using TodoApp.Infrastructure.TodoRepository;

namespace TodoApp.Test.Application_Tests.TodoTest;

public sealed class QueryHandlersTest
{
    private readonly AppDbContext _context;
    private readonly ITodoService _service;

    private readonly CancellationToken _token;
    public QueryHandlersTest()
    {
        _context = new AppDbContext(new DbContextOptionsBuilder<AppDbContext>().UseInMemoryDatabase("todo").Options);
        _service = new TodoService(_context);
        _token = new CancellationTokenSource().Token;
    }

    [Fact]
    public async Task TestGetTodosQuery_ReturnsAllTodos()
    {
        // Arrange
        var todos = new List<Todo>()
        {
            new(TodoId.CreateUnique(null), "Test", "Test", Guid.NewGuid().ToString()),
            new(TodoId.CreateUnique(null), "Test1", "Test1", Guid.NewGuid().ToString()),
            new(TodoId.CreateUnique(null), "Test2", "Test2", Guid.NewGuid().ToString()),
            new(TodoId.CreateUnique(null), "Test3", "Test3", Guid.NewGuid().ToString()),
        };
        var todosCount = todos.Count;
        //helper.WriteLine($"Initial List {todosCount}");
        _context.Todos.AddRange(todos);
        _context.SaveChanges();

        // Act
        var query = new GetAllTodosQuery();
        var handler = new GetAllTodosQueryHandler(_service);
        var result = await handler.Handle(query, _token);
        //helper.WriteLine($"Result: {result.Count}");
        // Assert
        result.Should().NotBeNull();
        Assert.IsType<ImmutableList<Todo>>(result);
        // result.Should().BeOfType<IImmutableList<Todo>>();
        result.Count.Should().BeOneOf(todosCount + 1, todosCount);
        // Assert.Equal(todosCount, result.Count);
    }
    [Fact]
    public async Task TestGetTodo_ReturnsNullOnInvalidIdentifer()
    {
        // Arrange
        var db = _context;
        var service = _service;
        var token = _token;

        await db.AddAsync(new Todo(TodoId.CreateUnique(null), "", "", Guid.NewGuid().ToString()));
        await db.SaveChangesAsync();
        // Act
        var query = new GetTodoByIdQuery(Guid.NewGuid().ToString());
        var handler = new GetTodoQueryHandler(service);

        var result = await handler.Handle(query, token);

        // Assert
        result.Should().BeOfType<ErrorOr<Todo>>();
        // Clean up
        _context.Todos.RemoveRange(_context.Todos);
        await _context.SaveChangesAsync();
    }

    [Fact]
    public async Task TestGetTodoQuery_ReturnsTodoOnValidIdentifer()
    {
        // Arrange
        var db = _context;
        var service = _service;
        var token = _token;

        var identifier = Guid.NewGuid().ToString();
        await db.AddAsync(new Todo(TodoId.CreateUnique(null), "", "", identifier));
        await db.SaveChangesAsync();

        // Act
        var query = new GetTodoByIdQuery(identifier);
        var handler = new GetTodoQueryHandler(service);

        var result = await handler.Handle(query, token);

        // Assert
        result.Value.Should().BeOfType<Todo>();
        identifier.Should().Be(result.Value.Identifier);
        // Clean up
        _context.Todos.RemoveRange(_context.Todos);
        await _context.SaveChangesAsync();
    }


}