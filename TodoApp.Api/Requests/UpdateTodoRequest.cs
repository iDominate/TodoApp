namespace TodoApp.Api.Requests;

public sealed record UpdateTodoRequest(string Identifier, string Title, string Description);