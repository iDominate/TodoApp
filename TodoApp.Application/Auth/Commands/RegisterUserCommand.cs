using ErrorOr;
using MediatR;
using TodoApp.Domain.UserAggregate;

namespace TodoApp.Application.Auth.Commands;

public sealed record RegisterUserCommand(
    string FirstName,
    string LastName,
    string UserName,
    string Email,
    string Password,
    string ConfirmPassword
) : IRequest<ErrorOr<AuthModel>>;