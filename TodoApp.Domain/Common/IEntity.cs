namespace TodoApp.Domain.Common;

public class IEntity<TId> where TId : notnull
{
    public TId Id { get; private set; }

    public IEntity(TId Id)
    {
        this.Id = Id;
    }
}