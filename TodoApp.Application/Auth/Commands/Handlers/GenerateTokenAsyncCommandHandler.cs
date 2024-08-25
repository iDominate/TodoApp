using ErrorOr;
using MediatR;
using TodoApp.Application.Services;
using TodoApp.Domain.UserAggregate;

namespace TodoApp.Application.Auth.Commands.Handlers;

public sealed class GenerateTokenAsyncCommandHandler : IRequestHandler<GenerateTokenAsyncCommand, ErrorOr<AuthModel>>
{
    private readonly IAuthService _authService;

    public GenerateTokenAsyncCommandHandler(IAuthService authService)
    {
        _authService = authService;
    }

    public async Task<ErrorOr<AuthModel>> Handle(GenerateTokenAsyncCommand request, CancellationToken cancellationToken)
    {
        return await this._authService.GenerateTokenAsync(request.Token);
    }
}