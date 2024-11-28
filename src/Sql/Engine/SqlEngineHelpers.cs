using lotus.src.Database.Enums;
using lotus.src.Database.Models;
using lotus.src.Sql.Enums;
using lotus.src.Sql.Utils;

namespace lotus.src.Sql.Engine;

public sealed partial class SqlEngine
{
    private bool IsDataTypeMatch(DataColumnType columnType, SqlTokenType tokenType)
    {
        Dictionary<DataColumnType, SqlTokenType> DataTypeMap = new()
        {
            { DataColumnType.VarChar, SqlTokenType.String },
            { DataColumnType.Int, SqlTokenType.Integer },
            { DataColumnType.Float, SqlTokenType.Float },
        };

        return DataTypeMap[columnType] == tokenType;
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
