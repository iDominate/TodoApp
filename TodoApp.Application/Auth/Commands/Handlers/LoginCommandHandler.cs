using ErrorOr;
using MediatR;
using TodoApp.Application.Extensions;
using TodoApp.Application.Services;
using TodoApp.Domain.UserAggregate;

namespace TodoApp.Application.Auth.Commands.Handlers;

public sealed class LoginCommandHandler : IRequestHandler<LoginCommand, ErrorOr<AuthModel>>
{
    private readonly IAuthService _authService;
    public LoginCommandHandler(IAuthService authService)
    {
        this._authService = authService;
    }
    public async Task<ErrorOr<AuthModel>> Handle(LoginCommand request, CancellationToken cancellationToken)
    {
        return await this._authService.LoginAsync(request.ToLoginDto());
    }
}