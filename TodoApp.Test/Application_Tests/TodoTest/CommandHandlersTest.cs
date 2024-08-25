using FluentAssertions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using TodoApp.Application.Services;
using TodoApp.Application.TodoCQRS.Commands;
using TodoApp.Domain.TodoAggregate;
using TodoApp.Domain.TodoAggregate.ValueObjects;
using TodoApp.Infrastructure.Context;
using TodoApp.Infrastructure.TodoRepository;

namespace TodoApp.Test.Application_Tests.TodoTest;


public class CommandHandlersTest
{
    private readonly AppDbContext _context;
    private readonly TodoService _service;
    private readonly CancellationToken _source;
    public CommandHandlersTest()
    {
        _context = new AppDbContext(new DbContextOptionsBuilder<AppDbContext>().UseInMemoryDatabase("todo").Options);
        _service = new TodoService(_context);
        _source = new CancellationTokenSource().Token;
    }

    [Fact]
    public async Task CreateTodoAsync_Should_Create_Todo()
    {

        // Arrange
        // Act
        var command = new CreateTodoCommand("Test", "Test");
        var handler = new CreateTodoQueryHandler(_service);
        var result = await handler.Handle(command, _source);
        var wasTodoSaved = await _context.Todos.FirstOrDefaultAsync(t => t.Identifier == "Test");

        // Assert
        result.Should().NotBeNull();
        result.Should().BeOfType<Todo>();
        wasTodoSaved.Should().NotBeNull();
        _context.Todos.Remove((await _context.Todos.FirstOrDefaultAsync(t => t.Title == "Test"))!);
        _context.SaveChanges();
    }

    [Fact]
    public async Task DeleteTodoAsync_Should_Delete_Todo()
    {
        var db = new AppDbContext(new DbContextOptionsBuilder<AppDbContext>().UseInMemoryDatabase("todos").Options);
        var ser = new TodoService(db);
        var todo = await ser.CreateTodoAsync("Test1", "Test1");
        //db.Entry(todo).State = EntityState.Added;
        var a = await db.Todos.FirstOrDefaultAsync(t => t.Identifier == "Test1");
        System.Console.WriteLine(a);
        // Act
        var command = new DeleteTodoCommand(todo.Value.Identifier);
        var handler = new DeleteTodoCommandHandler(ser);
        var result = await handler.Handle(command, _source);
        var wasTodoDeleted = await db.Todos.FirstOrDefaultAsync(t => t.Title == "Test1");

        // Assert
        result.Value.Should().NotBeEmpty();
        wasTodoDeleted.Should().BeNull();
        //collection.Dispose();
    }

    [Fact]
    public async Task UpdateTodoAsync_Should_Update_Todo()
    {
        // Arrange
        var db = new AppDbContext(new DbContextOptionsBuilder<AppDbContext>().UseInMemoryDatabase("todos").Options);
        var ser = new TodoService(db);
        var todo = await ser.CreateTodoAsync("Test1", "Test1");
        //db.Entry(todo).State = EntityState.Added;
        var a = await db.Todos.FirstOrDefaultAsync(t => t.Title == "Test1");
        // Act
        // var newTodo = new Todo(TodoId.CreateUnique(null), "Test", "Test", Guid.NewGuid());
        var command = new UpdateTodoCommand(todo.Value.Identifier, "Test", "Test");
        var handler = new UpdateTodoCommandHandler(ser);
        var result = await handler.Handle(command, _source);
        var wasTodoUpdated = await db.Todos.FirstOrDefaultAsync(t => t.Title == "Test1");
        // Assert
        result.Value.Should().NotBeNull();
        result.Value.Should().BeOfType<Todo>();
        wasTodoUpdated.Should().NotBeNull();
        Assert.Equal("Test2", wasTodoUpdated!.Title);
        Assert.Equal("Test2", wasTodoUpdated.Description);
        //collection.Dispose();
    }

}