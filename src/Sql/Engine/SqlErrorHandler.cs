using lotus.src.Database.Models;
using lotus.src.Sql.Models;
using lotus.src.Sql.Utils;
using System.Data.Common;

namespace lotus.src.Sql.Engine;

public sealed class SqlErrorHandler(List<string> errors)
{
    public readonly List<string> Errors = errors;

    public QueryResult<List<DatabaseRow>> TableDoesNotExist(string table) 
    {
        var message = $"table '{table}' does not exist.";

        Errors.Add(message);
        return QueryResult<List<DatabaseRow>>.Err(message);
    }

    public QueryResult<List<DatabaseRow>> TableAlreadyExists(string table)
    {
        var message = $"table '{table}' already exists.";

        Errors.Add(message);
        return QueryResult<List<DatabaseRow>>.Err(message);
    }

    public QueryResult<List<DatabaseRow>> ColumnDoesNotExist(string column)
    {
        var message = $"column '{column}' does not exist.";

        Errors.Add(message);
        return QueryResult<List<DatabaseRow>>.Err(message);
    }

    public QueryResult<List<DatabaseRow>> ColumnAlreadyExists(string column)
    {
        var message = $"column '{column}' already exists.";

        Errors.Add(message);
        return QueryResult<List<DatabaseRow>>.Err(message);
    }

    public QueryResult<List<DatabaseRow>> InvalidExpression(object expression)
    {
        var message = $"'{expression}' is not valid in this expression.";

        Errors.Add(message);
        return QueryResult<List<DatabaseRow>>.Err(message);
    }

    public QueryResult<List<DatabaseRow>> UnknownExpression(string expression)
    {
        var message = $"Unknown expression: '{expression}'";

        Errors.Add(message);
        return QueryResult<List<DatabaseRow>>.Err(message);
    }

    public QueryResult<List<DatabaseRow>> DatabaseDoesNotExist(string database)
    {
        var message = $"Database '{database}' does not exist.";

        Errors.Add(message);
        return QueryResult<List<DatabaseRow>>.Err(message);
    }

    public QueryResult<List<DatabaseRow>> DatabaseAlreadyExists(string database)
    {
        var message = $"Database '{database}' already exists.";

        Errors.Add(message);
        return QueryResult<List<DatabaseRow>>.Err(message);
    }

    public QueryResult<List<DatabaseRow>> NoActiveDbSelected()
    {
        var message = $"No active database is currently selected.";

        Errors.Add(message);
        return QueryResult<List<DatabaseRow>>.Err(message);
    }
}
