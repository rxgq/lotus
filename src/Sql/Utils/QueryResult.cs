using lotus.src.Database.Models;

namespace lotus.src.Sql.Utils;

public sealed class QueryResult<T>
{
    public readonly bool IsSuccess;
    public readonly string? Message;
    public readonly T? Value;

    // holds the data to display in the query result tab
    public readonly DatabaseTable? TableResult;

    public readonly DatabaseModel? DatabaseResult;

    private QueryResult(bool isSuccess, string? message, T? value, DatabaseTable? tableResult, DatabaseModel? databaseResult)
    {
        IsSuccess = isSuccess;
        Message = message;
        Value = value;
        TableResult = tableResult;
        DatabaseResult = databaseResult;
    }

    public static QueryResult<T> Ok(T? t = default, string? message = null, DatabaseTable? tableAffected = null, DatabaseModel? databaseAffected = null)
    {
        return new(true, message, t, tableAffected, databaseAffected);
    }

    public static QueryResult<T> Err(string message)
    {
        return new(false, message, default, null, null);
    }
}
