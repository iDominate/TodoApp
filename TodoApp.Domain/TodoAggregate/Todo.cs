using Microsoft.EntityFrameworkCore.ChangeTracking;
using TodoApp.Domain.Common;
using TodoApp.Domain.TodoAggregate.ValueObjects;

namespace TodoApp.Domain.TodoAggregate;

public class Todo : IEntity<TodoId>
{
    public string Identifier { get; private set; }
    public string Title { get; set; }
    public string Description { get; set; }
    public Todo(TodoId todoId, string title, string description, string identifier) : base(todoId)
    {
        Identifier = identifier;
        Title = title;
        Description = description;
    }
#pragma warning disable CS8618
    public Todo() : base(TodoId.CreateUnique(null))
    {

    }
#pragma warning restore CS8618
    public void ChangeTitle(string title)
    {
        ArgumentNullException.ThrowIfNullOrEmpty(title, "titles cannot be null");
        Title = title;
    }
    public void ChangeDescription(string description)
    {
        ArgumentNullException.ThrowIfNullOrEmpty(description, "description cannot be null");
        Description = description;
    }

}