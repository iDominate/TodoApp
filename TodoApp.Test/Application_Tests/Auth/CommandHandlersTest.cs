using FluentAssertions;
using MediatR;
using Moq;
using TodoApp.Application.Auth.Commands;
using TodoApp.Application.Auth.Commands.Handlers;
using TodoApp.Application.Services;
using TodoApp.Domain.UserAggregate;

namespace TodoApp.Test.Application_Tests.Auth;


public class CommandHandlersTest
{
    private readonly CancellationTokenSource _source;
    private readonly Mock<IAuthService> _authService;
    private readonly ApplicationUser _user;
    public CommandHandlersTest()
    {
        _source = new CancellationTokenSource();

        _user = new ApplicationUser("Test", "Test")
        {
            Email = "test@example.com",
            UserName = "test"
        };
        _authService = new Mock<IAuthService>();
    }

    [Fact]
    public async void TestLoginCommandHandler_RetrunsUnauthenticatedOnUserNotFound()
    {
        // Arrange

        _authService.Setup(x => x.LoginAsync(It.IsAny<LoginReuqest>())).ReturnsAsync(new AuthModel()
        {
            IsAuthenticated = false,
            Message = "Invalid User"
        });

        // Act
        var command = new LoginCommand("test", "123");
        var handler = new LoginCommandHandler(_authService.Object);
        var result = await handler.Handle(command, _source.Token);

        // Assert
        result.Value.Should().BeOfType<AuthModel>();
        result.Value.IsAuthenticated.Should().BeTrue();
        result.Value.IsAuthenticated.Should().BeFalse();
    }

    [Fact]
    public async void TestLoginCommandHandler_ReturnsUser()
    {
        // Arrange
        _authService.Setup(x => x.LoginAsync(It.IsAny<LoginReuqest>())).ReturnsAsync(new AuthModel()
        {
            Email = _user.Email!,
            HasError = false,
            Token = "123",
            IsAuthenticated = true,
            Message = "Invalid User"
        });

        // Act
        var command = new LoginCommand("test", "123");
        var handler = new LoginCommandHandler(_authService.Object);
        var result = await handler.Handle(command, _source.Token);

        // Assert
        result.Value.Should().BeOfType<AuthModel>();
        result.Value.IsAuthenticated.Should().BeTrue();
        result.IsError.Should().BeFalse();
        result.Value.Token.Should().NotBeEmpty();
    }

    [Fact]
    public async void TestRegisterUserAsync_ReturnsErrorOnPasswordsNotMatching()
    {
        // Arrange
        _authService.Setup(u => u.RegisterAsync(It.IsAny<RegisterRequest>()))
        .ReturnsAsync(new AuthModel()
        {
            HasError = true,
            Message = "Passwords do not match"
        });
        var command = new RegisterUserCommand("Test", "Test", "test", "test@example.com", "123", "1234");

        // Act 
        var handler = new RegisterUserCommandHandler(_authService.Object);
        var result = await handler.Handle(command, _source.Token);
        // Assert
        result.Value.Should().BeOfType<AuthModel>();
        result.IsError.Should().BeTrue();
    }

    [Fact]
    public async void TestRegisterUserAsync_ReturnsSuccess()
    {

        _authService.Setup(u => u.RegisterAsync(It.IsAny<RegisterRequest>()))
        .ReturnsAsync(new AuthModel()
        {
            HasError = false,
            UserName = _user.UserName!,
            Email = _user.Email!,
            Message = "Success",
            IsAuthenticated = true

        });
        // Arrange
        var command = new RegisterUserCommand("Test", "Test", "test", "test@example.com", "123", "123");

        // Act 
        var handler = new RegisterUserCommandHandler(_authService.Object);
        var result = await handler.Handle(command, _source.Token);
        // Assert
        result.Value.Should().BeOfType<AuthModel>();
        result.IsError.Should().BeFalse();
        result.Value.IsAuthenticated.Should().BeTrue();
        result.Value.Message.Should().Be("Success");
    }
}