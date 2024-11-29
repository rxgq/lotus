using lotus.src.Database.Enums;
using lotus.src.Database.Models;
using lotus.src.Sql.Enums;
using lotus.src.Sql.Utils;

namespace lotus.src.Sql.Engine;

public sealed partial class SqlEngine
{
    private static bool IsDataTypeMatch(DataColumnType columnType, SqlTokenType tokenType)
    {
        Dictionary<DataColumnType, HashSet<SqlTokenType>> DataTypeMap = new()
        {
            { DataColumnType.VarChar, new HashSet<SqlTokenType> { SqlTokenType.String } },
            { DataColumnType.Int,     new HashSet<SqlTokenType> { SqlTokenType.Integer } },
            { DataColumnType.Float,   new HashSet<SqlTokenType> { SqlTokenType.Float } },
            { DataColumnType.Bool,    new HashSet<SqlTokenType> { SqlTokenType.True, SqlTokenType.False } },
        };

        return DataTypeMap.TryGetValue(columnType, out var validTokens) && validTokens.Contains(tokenType);
    }


    private QueryResult<List<DatabaseRow>> ExecutionError(string message)
    {
        ExecutionErrors.Add(message);
        return QueryResult<List<DatabaseRow>>.Err(message);
    }

    private DatabaseTable? GetTable(string name)
    {
        return Tables.FirstOrDefault(x => x.Name == name);
    }

    private DatabaseModel? GetDatabase(string name)
    {
        return DatabaseEngine.Databases.FirstOrDefault(x => x.Name == name);
    }

    private bool ActiveDbIsNull() 
    {
        return DatabaseEngine.ActiveDatabase is null;
    }
}
