namespace TodoApp.Api.Requests;

public sealed record RegisterUserRequest(string FirstName, string LastName, string Email, string Password, string PasswordConfirmation,
    string UserName);