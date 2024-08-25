namespace TodoApp.Api.Common;

public sealed record TodoResponse<T> where T : notnull
{
    public TodoResponse(T data, string status = ResponseStatus.Success)
    {
        Status = status;

        Data = data;
    }

    public string Status { get; private set; } = ResponseStatus.Success;
    public T Data { get; private set; } = default!;
    public void SetData(T data)
    {
        if (data is null or 0)
        {
            throw new ArgumentNullException(nameof(data));
        }
        Data = data;
    }


    public void SetStatus(string status)
    {
        if (status is not ResponseStatus.Success or ResponseStatus.Error)
        {
            return;
        }
    }

}