using ErrorOr;
using MediatR;
using TodoApp.Domain.UserAggregate;

namespace TodoApp.Application.Auth.Commands;

public sealed record LoginCommand(string Username, string Password) : IRequest<ErrorOr<AuthModel>>;
