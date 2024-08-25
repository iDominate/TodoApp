using TodoApp.Api.Requests;
using TodoApp.Application.Auth.Commands;
using TodoApp.Application.TodoCQRS.Commands;

namespace TodoApp.Api;

public static class Extension
{
    public static CreateTodoCommand AsTodoCommand(this CreateTodoRequest request) => new(request.Title, request.Description);
    public static UpdateTodoCommand AsTodoCommand(this UpdateTodoRequest request) => new(request.Identifier, request.Title, request.Description);

    public static RegisterUserCommand AsRegisterUserCommand(this RegisterUserRequest register) =>
    new(register.FirstName, register.LastName, register.UserName, register.Email, register.Password, register.PasswordConfirmation);

    public static LoginCommand AsLoginCommand(this LoginUserRequest login) => new(login.UserName, login.Password);
}