using System.Runtime.CompilerServices;

namespace TodoApp.Domain.TodoAggregate.ValueObjects;

public sealed record TodoId
{
    public TodoId(Guid value)
    {
        Value = value;
    }

    public Guid Value { get; private set; }
    public static TodoId CreateUnique(Guid? guid) => new(guid ?? Guid.NewGuid());
};