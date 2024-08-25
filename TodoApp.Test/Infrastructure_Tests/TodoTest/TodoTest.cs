using ErrorOr;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using TodoApp.Application.Services;
using TodoApp.Domain.TodoAggregate;
using TodoApp.Domain.TodoAggregate.ValueObjects;
using TodoApp.Infrastructure.Context;
using TodoApp.Infrastructure.TodoRepository;
namespace TodoApp.Test.Infrastructure_Tests.TodoTest;

public class TodoTest
{
    private readonly ITodoService _service;
    private readonly AppDbContext _dbContext;
    public TodoTest()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>();
        options.UseInMemoryDatabase("test");
        _dbContext = new AppDbContext(options.Options);
        _service = new TodoService(_dbContext);
    }
    [Fact]
    public async void TestGetTodo_ReturnsNullOnFailure()
    {
        var result = await _service.GetTodoAsync(Guid.NewGuid().ToString());
        result.Should().BeOfType<ErrorOr<Todo>>();
        result.IsError.Should().BeTrue();
    }

    [Fact]
    public async void TestGetTodo_ReturnsTodo()
    {
        var identifier = Guid.NewGuid().ToString();
        // Arrange
        await this._dbContext.Todos.
        AddAsync(
        new Domain.TodoAggregate.Todo(TodoId.CreateUnique(null), "Test", "Test", identifier));
        await this._dbContext.SaveChangesAsync();

        // Act
        var result = await this._service.GetTodoAsync(identifier);

        // Assert
        result.Value.Should().NotBeNull();
        result.Value.Should().BeOfType<Todo>();
        result.IsError.Should().BeFalse();
        // Clean
        this._dbContext.Todos.RemoveRange(this._dbContext.Todos);
        await this._dbContext.SaveChangesAsync();
    }

    [Fact]
    public async void TestCreateTodo_ReturnTodoAndSavedInDatabase()
    {
        // Arrange
        // Act
        var result = await this._service.CreateTodoAsync("Test", "Test");
        // Assert
        result.Should().NotBeNull();
        result.Should().BeOfType<ErrorOr<Todo>>();
        var dbResult = await this._dbContext.Todos.ToListAsync();
        Assert.Single(dbResult);
        dbResult.Count.Should().Be(1);

        // Clean
        this._dbContext.Todos.RemoveRange(this._dbContext.Todos);
        await this._dbContext.SaveChangesAsync();
    }

    [Fact]
    public async void TestUpdateTodo_ReturnsNullOnFailure()
    {
        var identifier = Guid.NewGuid().ToString();
        // Act
        var result = await this._service.UpdateTodoAsync(Guid.NewGuid().ToString(), "Test", "Test");
        // Assert
        result.IsError.Should().BeTrue();

    }

    [Fact]
    public async void TestUpdateTodo_ReturnTodoAndSavedInDatabase()
    {
        // Arrange
        var todo = await _service.CreateTodoAsync("Test", "Test");
        // Act
        var result = await this._service.UpdateTodoAsync(todo.Value.Identifier, "Test1", "Test1");
        // Assert
        result.Value.Should().BeOfType<Todo>();
        var dbResult = await this._dbContext.Todos.ToListAsync();
        dbResult.Count.Should().Be(1);
        dbResult[0].Title.Should().Be("Test1");
        // Clean
        this._dbContext.Todos.RemoveRange(this._dbContext.Todos);
        await this._dbContext.SaveChangesAsync();
    }

    [Fact]
    public async void TestDeleteTodo_GeneratesErrorOnFailure()
    {
        // Arrange
        // Act
        var result = await this._service.DeleteTodoAsync(Guid.NewGuid().ToString());
        // Assert
        result.IsError.Should().BeTrue();
    }

    [Fact]
    public async void TestDeleteTodo_ReturnTodoAndSavedInDatabase()
    {
        // Arrange
        var todo = await _service.CreateTodoAsync("Test1", "Test1");
        // Act
        var result = await _service.DeleteTodoAsync(todo.Value.Identifier);
        // Assert
        result.Value.Should().Be(todo.Value.Identifier);
        var dbResult = await this._dbContext.Todos.ToListAsync();
        dbResult.Should().BeEmpty();
    }
}