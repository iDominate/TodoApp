using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using FluentAssertions;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using TodoApp.Api.Common;
using TodoApp.Api.Controllers;
using TodoApp.Api.Requests;
using TodoApp.Application.Auth.Commands;
using TodoApp.Domain.TodoAggregate;
using TodoApp.Domain.TodoAggregate.ValueObjects;
using TodoApp.Domain.UserAggregate;
using TodoApp.Infrastructure.Context;
using Xunit.Priority;

namespace TodoApp.Test.Integration;

[TestCaseOrderer(PriorityOrderer.Name, PriorityOrderer.Assembly)]
[Collection("Todo Integration")] //Preventing parallelism which produces flaky results
public sealed class TestTodo : IClassFixture<TodoAppFixture<Program>>
{
    private readonly HttpClient _client;
    private readonly ISender _sender;
    private readonly TodoAppFixture<Program> _fixture;

    private readonly TodoController _controller;
    public TestTodo(TodoAppFixture<Program> fixture)
    {

        _fixture = fixture;
        _client = _fixture.CreateClient();
        using var scope = _fixture.Services.CreateScope();
        _sender = scope.ServiceProvider.GetRequiredService<ISender>();
        _controller = new TodoController(_sender);
        //_client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Test");
    }
    private async Task<string> RegisterUserAsync()
    {
        var request = new RegisterUserRequest("test", "test", "test@gmail.com", "P@ssword123", "P@ssword123", "test");
        var rsp = await _client.PostAsJsonAsync("/api/v1/auth/register", request);
        var result = await rsp.Content.ReadFromJsonAsync<TodoResponse<AuthModel>>();
        _client.DefaultRequestHeaders.Add("Authorization", "Bearer " + result!.Data.Token);
        return result!.Data.Token;
    }

    // private async Task LoginUserAsync()
    // {
    //     var request = new LoginUserRequest("test", "P@ssword123");
    //     var rsp = await _client.PostAsJsonAsync("/api/v1/auth/login", request);
    //     // var result = await rsp.Content.ReadFromJsonAsync<TodoResponse<AuthModel>>();
    //     // return result!;
    // }

    [Fact]
    [Priority(0)]
    public async void TestGetAllTodos_ReturnsPopulatedList()
    {
        // Arrange
        var token = await RegisterUserAsync();
        // Act
        var rsp = await _client.GetAsync("/api/v1/todos");
        var list = await rsp.Content.ReadFromJsonAsync<TodoResponse<IEnumerable<Todo>>>();
        // Assert
        rsp.StatusCode.Should().Be(HttpStatusCode.OK);
        list!.Data.ToList().Count.Should().Be(10);

    }

    // [Fact]
    // [Priority(1)]
    // public async void TestTestAsync_ReturnsSuccess()
    // {
    //     // Arrange
    //     // Act
    //     var rsp = await _client.GetAsync("/api/v1/auth/test");
    //     // Assert
    //     // str.Should().NotBeNull();
    //     rsp.StatusCode.Should().Be(System.Net.HttpStatusCode.OK);
    // }

    [Fact]
    [Priority(1)]
    public async void TestGetTodo_ReturnsNotfound()
    {
        // Arrange
        // Act
        var res = _client.GetAsync
        ($"/api/v1/todos/{Guid.NewGuid()}").Result;
        var rsp = await res.Content.ReadFromJsonAsync<TodoResponse<string>>();
        // Assert
        res!.StatusCode.Should().Be(System.Net.HttpStatusCode.NotFound);
    }


    [Fact]
    [Priority(2)]
    public async void TestCreateTodoWithNoData_ReturnsTodo()
    {

        // Arrange
        using var scope = _fixture.Services.CreateScope();
        var todo = new Todo(TodoId.CreateUnique(Guid.NewGuid()), "Test", "Test", Guid.NewGuid().ToString());
        var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        dbContext.Todos.Add(todo);
        dbContext.SaveChanges();

        // Act
        var res = await _client.GetFromJsonAsync<TodoResponse<Todo>>
        ($"/api/v1/todos/{todo.Identifier}");
        // Assert
        res!.Data.Title.Should().Be("Test");
    }

    [Fact]
    [Priority(3)]
    public async void TestCreateTodoWithNoData_ReturnsValidationError()
    {

        // Arrange
        var request = new CreateTodoRequest("", "");
        var json = JsonContent.Create(request);
        // Act
        var res = await _client.PostAsync("/api/v1/todos", json);
        // Assert
        // resultTodo.Should().BeOfType<Todo>();
        // resultTodo.Identifier.Should().Be(todo.Identifier);
        res.StatusCode.Should().Be(System.Net.HttpStatusCode.BadRequest);

    }

    [Fact]
    [Priority(4)]
    public async void TestUpdateTodoWithNotData_ReturnsValidationError()
    {
        // Arrange
        var request = new UpdateTodoRequest("", "", "");
        var json = JsonContent.Create(request);
        // Act
        var res = await _client.PutAsync("/api/v1/todos", json);
        // Assert
        res.StatusCode.Should().Be(System.Net.HttpStatusCode.BadRequest);
    }

    [Fact]
    [Priority(5)]
    public async void TestUpdatetodoWithData_ReturnsTodo()
    {
        // Arrange
        var todo = new Todo(TodoId.CreateUnique(Guid.NewGuid()), "Test", "Test", Guid.NewGuid().ToString());
        using var scope = _fixture.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        dbContext.Todos.Add(todo);
        dbContext.SaveChanges();
        var request = new UpdateTodoRequest(todo.Identifier, todo.Title, todo.Description);
        var json = JsonContent.Create(request);
        // Act
        var res = await _client.PutAsync("/api/v1/todos", json);
        // Assert
        res.StatusCode.Should().Be(System.Net.HttpStatusCode.OK);
    }

    [Fact]
    [Priority(6)]
    public async void TestDeleteTodoWithInvalidData_ReturnsValidationError()
    {
        // Arrange

        // Act
        var res = await _client.DeleteAsync($"/api/v1/todos/{Guid.NewGuid().ToString()}");
        // Assert
        res.StatusCode.Should().Be(System.Net.HttpStatusCode.BadRequest);
    }

    [Fact]
    [Priority(7)]
    public async void TestDeleteTodoWithValidData_ReturnsTodo()
    {
        // Arrange
        var todo = new Todo(TodoId.CreateUnique(Guid.NewGuid()), "Test", "Test", Guid.NewGuid().ToString());
        using var scope = _fixture.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        dbContext.Todos.Add(todo);
        dbContext.SaveChanges();
        // Act
        var res = await _client.DeleteAsync($"/api/v1/todos/{todo.Identifier}");
        // Assert
        res.StatusCode.Should().Be(System.Net.HttpStatusCode.NoContent);
    }
}