namespace TodoApp.Api.Requests;

public sealed record LoginUserRequest(string UserName, string Password);