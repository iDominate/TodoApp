using ErrorOr;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Serilog;
using TodoApp.Api.Common;
using TodoApp.Api.Requests;
using TodoApp.Application.Auth.Commands;
using TodoApp.Domain.UserAggregate;

namespace TodoApp.Api.Controllers;

[ApiController]
[Route("api/v1/auth")]
public sealed class AuthenticationController(ISender _sender) : ControllerBase
{
    [HttpPost("register")]
    public async Task<IActionResult> RegisterAsync(RegisterUserRequest request)
    {
        Log.Information($"Attempting to register user with username {request.UserName} and email {request.Email}");
        var command = request.AsRegisterUserCommand();
        var result = await _sender.Send(command);
        if (result.IsError is true)
        {
            if (result.FirstError.Type == ErrorOr.ErrorType.Failure)
            {
                Log.Error("Failed registering user. The user is already registered");
                return Unauthorized(new TodoResponse<string>(result.FirstError.Description, status: ResponseStatus.Error));
            }
            else if (result.FirstError.Type == ErrorOr.ErrorType.Validation)
            {
                Log.Error($"Validation error has occurred");
                return BadRequest(new TodoResponse<IEnumerable<string>>(result.Errors.ConvertAll(e => e.Description)));
            }
            else
            {
                return Unauthorized(new TodoResponse<IEnumerable<string>>(result.Errors.ConvertAll(e => e.Description)));
            }
        }
        Log.Information("Registration successful. Returning tokens...");
        return Ok(new TodoResponse<AuthModel>(result.Value, status: ResponseStatus.Success));
    }

    [HttpGet("test")]
    public IActionResult TestAsync()
    {
        Log.Debug("Hello World!");
        return Ok("Success");
    }

    [HttpPost("login")]
    public async Task<IActionResult> LoginAsync(LoginUserRequest request)
    {
        Log.Information($"Attempting to login user with username {request.UserName}");
        var command = request.AsLoginCommand();
        var result = await _sender.Send(command);
        if (result.IsError is true)
        {
            if (result.FirstError.Type is ErrorOr.ErrorType.Failure)
            {

                Log.Error("Login failed. username does not exist in the database");
                return Unauthorized(new TodoResponse<string>(result.FirstError.Description, status: ResponseStatus.Error));
            }
            else
            {
                Log.Error("Validation error has occurred");
                return BadRequest(new TodoResponse<IEnumerable<string>>(result.Errors.ConvertAll(e => e.Description)));
            }
        }
        Log.Error("Login Successful. Returning user tokens...");
        return Ok(result.Value);
    }

    [HttpPost]
    public async Task<IActionResult> RefreshTokenAsync(string token)
    {
        Log.Information($"Attempting to refresh token with token {token}...");
        var command = new GenerateTokenAsyncCommand(token);
        var result = await _sender.Send(command);
        if (result.IsError is true)
        {
            if (result.FirstError.Type is ErrorType.Forbidden)
            {
                Log.Error("Refresh token failed. Invalid token");
                return Unauthorized(new TodoResponse<string>(result.FirstError.Description, status: ResponseStatus.Error));
            }
        }
        Log.Information("Refresh token successful. Returning user tokens...");
        return Ok(new TodoResponse<AuthModel>(result.Value, status: ResponseStatus.Success));
    }
}