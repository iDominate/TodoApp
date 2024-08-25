using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using FluentAssertions;
using MediatR;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using TodoApp.Api.Common;
using TodoApp.Api.Controllers;
using TodoApp.Api.Requests;
using TodoApp.Application.Auth.Commands;
using TodoApp.Domain.UserAggregate;
using Xunit.Priority;

namespace TodoApp.Test.Integration;


[Collection("Authentication integration")]
public sealed class AuthenticationTest : IClassFixture<TodoAppFixture<Program>>
{
    private readonly AuthenticationController _controller;
    private readonly TodoAppFixture<Program> _fixture;
    private readonly HttpClient _client;

    public AuthenticationTest(TodoAppFixture<Program> fixture)
    {
        _fixture = fixture;
        using var scope = _fixture.Services.CreateScope();
        var sender = scope.ServiceProvider.GetRequiredService<ISender>();
        _controller = new AuthenticationController(sender);
        _client = _fixture.CreateClient();
    }

    [Fact]
    [Priority(0)]
    public async void TestRegisterAsyncWithInvalidData_ReturnValidationError()
    {
        // Arrange
        var command = new RegisterUserCommand("", "", "", "", "", "");
        var json = JsonContent.Create(command);
        // Act
        var response = await _client.PostAsync("/api/v1/auth/register", json);
        // Assert
        response.StatusCode.Should().Be(System.Net.HttpStatusCode.BadRequest);
    }

    [Fact]
    [Priority(1)]
    public async void TestRegisterAsyncWithProperData_ReturnsSuccessfulReponse()
    {
        // Arrange
        var command = new RegisterUserRequest("test", "test", "test@example.com", "test", "test", "test");
        //var json = JsonContent.Create(command);
        // Act
        var response = await _client.PostAsJsonAsync<RegisterUserRequest>("/api/v1/auth/register", command);

        var options = new JsonSerializerOptions(JsonSerializerDefaults.Web);
        options.Converters.Add(new JsonStringEnumConverter());
        var auth = await response.Content.ReadFromJsonAsync<TodoResponse<AuthModel>>(options);
        //auth1!.Email.Should().NotBeNull();
        // Assert

        response.StatusCode.Should().Be(System.Net.HttpStatusCode.OK);
    }
}