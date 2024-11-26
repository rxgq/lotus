using lotus.src.Models;

namespace lotus.src.Sql.Utils;

public sealed class QueryResult<T>
{
    public readonly bool IsSuccess;
    public readonly string? Message;
    public readonly T? Value;

    public readonly DatabaseTable? TableAffected;

    private QueryResult(bool isSuccess, string? message, T? value, DatabaseTable? tableAffected)
    {
        IsSuccess = isSuccess;
        Message = message;
        Value = value;
        TableAffected = tableAffected;
    }

    public static QueryResult<T> Ok(T? t = default, string? message = null, DatabaseTable? tableAffected = null)
    {
        return new(true, message, t, tableAffected);
    }

    public static QueryResult<T> Err(string message)
    {
        return new(false, message, default, null);
    }
}
