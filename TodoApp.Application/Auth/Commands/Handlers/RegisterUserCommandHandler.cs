using ErrorOr;
using MediatR;
using TodoApp.Application.Extensions;
using TodoApp.Application.Services;
using TodoApp.Domain.UserAggregate;

namespace TodoApp.Application.Auth.Commands.Handlers;

public sealed class RegisterUserCommandHandler : IRequestHandler<RegisterUserCommand, ErrorOr<AuthModel>>
{
    private readonly IAuthService _authService;

    public RegisterUserCommandHandler(IAuthService authService)
    {
        _authService = authService;
    }

    public Task<ErrorOr<AuthModel>> Handle(RegisterUserCommand request, CancellationToken cancellationToken)
    {
        return _authService.RegisterAsync(request.ToRegisterDto());
    }
}