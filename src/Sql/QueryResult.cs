namespace lotus.src.Sql;

public sealed class QueryResult<T>
{
    public readonly bool IsSuccess;
    public readonly string? Message;
    public readonly T? Value;

    private QueryResult(bool isSuccess, string? message, T? value) 
    {
        IsSuccess = isSuccess;
        Message = message;
        Value = value;
    }

    public static QueryResult<T> Ok(T t, string? message = null) 
    {
        return new(true, message, t);
    }

    public static QueryResult<T> Err(string message)
    {
        return new(false, message, default);
    }
}
