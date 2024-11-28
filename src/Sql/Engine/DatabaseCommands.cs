using lotus.src.Database.Models;
using lotus.src.Sql.Models;
using lotus.src.Sql.Utils;

namespace lotus.src.Sql.Engine;

public sealed partial class SqlEngine
{
    private QueryResult<List<DatabaseRow>> ExecuteUseStmt(UseStmt useStmt)
    {
        var db = GetDatabase(useStmt.DatabaseName);
        if (db is null)
        {
            return _errorHandler.DatabaseDoesNotExist(useStmt.DatabaseName);
        }

        DatabaseEngine.ActiveDatabase = db;

        return QueryResult<List<DatabaseRow>>.Ok(databaseAffected: db);
    }

    private QueryResult<List<DatabaseRow>> ExecuteCreateDatabaseStmt(CreateDatabaseStatement createDbStmt)
    {
        var db = GetDatabase(createDbStmt.DatabaseName);
        if (db is not null)
        {
            return _errorHandler.DatabaseAlreadyExists(createDbStmt.DatabaseName);
        }

        var newDb = _dbFactory.Create(createDbStmt.DatabaseName);
        DatabaseEngine.Databases.Add(newDb);

        return QueryResult<List<DatabaseRow>>.Ok(databaseAffected: db);
    }

    private QueryResult<List<DatabaseRow>> ExecuteDropDatabaseStmt(DropDatabaseStatement dropDbStmt)
    {
        var db = GetDatabase(dropDbStmt.DatabaseName);
        if (db is null)
        {
            return _errorHandler.DatabaseDoesNotExist(dropDbStmt.DatabaseName);
        }

        DatabaseEngine.Databases.Remove(db);

        return QueryResult<List<DatabaseRow>>.Ok(databaseAffected: db);
    }
}
