namespace TodoApp.Api.Requests;

public sealed record CreateTodoRequest(string Title, string Description);