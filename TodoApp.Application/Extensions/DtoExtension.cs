using TodoApp.Application.Auth.Commands;
using TodoApp.Domain.UserAggregate;

namespace TodoApp.Application.Extensions;


public static class DtoExtension
{
    public static LoginReuqest ToLoginDto(this LoginCommand command)
    {
        return new LoginReuqest(command.Username, command.Password);
    }
    public static RegisterRequest ToRegisterDto(this RegisterUserCommand command)
    {
        return new RegisterRequest()
        {
            FirstName = command.FirstName,
            Lastname = command.LastName,
            UserName = command.UserName,
            Email = command.Email,
            Password = command.Password,
            ConfirmPassword = command.ConfirmPassword
        };
    }
}