using ErrorOr;
using MediatR;
using TodoApp.Domain.UserAggregate;

namespace TodoApp.Application.Auth.Commands;

public sealed record GenerateTokenAsyncCommand(string Token) : IRequest<ErrorOr<AuthModel>>;