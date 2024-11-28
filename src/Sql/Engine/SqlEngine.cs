using lotus.src.Database;
using lotus.src.Database.Factories;
using lotus.src.Database.Models;
using lotus.src.Database.Utils;
using lotus.src.Sql.Models;
using lotus.src.Sql.Utils;

namespace lotus.src.Sql.Engine;

public sealed partial class SqlEngine
{
    private readonly DatabaseEngine DatabaseEngine;
    private readonly SqlErrorHandler _errorHandler;

    private readonly DatabaseColumnFactory _columnFactory = new();
    private readonly DatabaseRowFactory _rowFactory = new();
    private readonly DatabaseTableFactory _tableFactory = new();
    private readonly DatabaseFactory _dbFactory = new();

    private readonly List<SqlStatement> Statements = [];
    public readonly List<string> ExecutionErrors = [];

    private int Current = 0;

    private List<DatabaseTable> Tables => DatabaseEngine.ActiveDatabase!.Tables;    
    public SqlEngine(List<SqlStatement> statements, DatabaseEngine databaseEngine)
    { 
        DatabaseEngine = databaseEngine;
        Statements = statements;

        _errorHandler = new(ExecutionErrors);
    }

    public ExecutionResult ExecuteStatements()
    {
        var results = new List<QueryResult<List<DatabaseRow>>>();
        while (Current < Statements.Count)
        {
            results.Add(Execute(Statements[Current]));

            if (ExecutionErrors.Count > 0)
                return new ExecutionResult(results, ExecutionErrors);

            Current++;
        }

        return new ExecutionResult(results, ExecutionErrors);
    }

    private QueryResult<List<DatabaseRow>> Execute(SqlStatement stmt)
    {
        var result = stmt switch
        {
            _ when stmt is SelectStatement select           => ExecuteSelectStmt(select),
            _ when stmt is CreateTableStatement createTb    => ExecuteCreateTableStmt(createTb),
            _ when stmt is CreateDatabaseStatement createDb => ExecuteCreateDatabaseStmt(createDb),
            _ when stmt is InsertIntoStatement insert       => ExecuteInsertStmt(insert),
            _ when stmt is DropTableStatement dropTb        => ExecuteDropTableStmt(dropTb),
            _ when stmt is DropDatabaseStatement dropDb     => ExecuteDropDatabaseStmt(dropDb),
            _ when stmt is AlterTableStatement alter        => ExecuteAlterTableStmt(alter),
            _ when stmt is DeleteFromStmt delete            => ExecuteDeleteFromStmt(delete),
            _ when stmt is UseStmt use                      => ExecuteUseStmt(use),
            _ => _errorHandler.UnknownExpression(stmt.ToString() ?? "")
        };

        return result;
    }

    private QueryResult<List<DatabaseRow>> ExecuteSelectStmt(SelectStatement selectStmt)
    {
        var columns = selectStmt.Values.ToList();

        var table = GetTable(selectStmt.FromStmt.Table);
        Current++;

        if (table is null)
        {
            return _errorHandler.TableDoesNotExist(selectStmt.FromStmt.Table);
        }

        List<DatabaseColumn> columnsResult = table.Columns
            .Where(col => columns.Contains(col.Title)).ToList();
        if (columns[0] == "*") columnsResult = table.Columns;

        List<DatabaseRow> rows = table.Rows;
        var tableResult = _tableFactory.Create(table.Name, columnsResult, rows);

        var limitStmt = selectStmt.FromStmt.LimitStmt;
        if (limitStmt is not null) 
        {
            var isDigit = int.TryParse(limitStmt.Count, out int count);
            if (!isDigit || count < 0) 
            {
                return _errorHandler.InvalidExpression(limitStmt.Count);
            }

            rows = rows.Take(count).ToList();
        }

        if (selectStmt.IsDistinct)
        {
            List<DatabaseRow> rowsToKeep = [];
            var seen = new HashSet<string>();

            foreach (var row in rows)
            {
                var key = string.Join("|", columnsResult.Select(col =>
                    row.Values.TryGetValue(col.Title, out var value) ? value.ToString() : "NULL")
                );

                if (!seen.Contains(key))
                {
                    rowsToKeep.Add(row);
                    seen.Add(key);
                }
            }

            rows = rowsToKeep;
        }


        return QueryResult<List<DatabaseRow>>.Ok(rows, tableAffected: tableResult);
    }

    private QueryResult<List<DatabaseRow>> ExecuteInsertStmt(InsertIntoStatement insertStmt)
    {
        if (ActiveDbIsNull()) return _errorHandler.NoActiveDbSelected();

        var table = GetTable(insertStmt.TableName);
        if (table is null)
        {
            return _errorHandler.TableDoesNotExist(insertStmt.TableName);
        }

        var dbRow = _rowFactory.Create([]);

        var providedColumns = new HashSet<string>(insertStmt.Columns);

        foreach (var column in table.Columns)
        {
            var columnIndex = insertStmt.Columns.IndexOf(column.Title);

            if (columnIndex >= 0)
            {
                if (!IsDataTypeMatch(column.DataType, insertStmt.Values[columnIndex].Type)) 
                {
                    return ExecutionError($"cannot insert data of type '{insertStmt.Values[columnIndex].Type}' into cell row of type '{column.DataType}'.");
                }

                var value = insertStmt.Values[columnIndex];
                dbRow.Values[column.Title] = value.Literal;
            }
            else
            {
                dbRow.Values[column.Title] = null;
            }
        }

        table.Rows.Add(dbRow);

        return QueryResult<List<DatabaseRow>>.Ok(tableAffected: table);
    }

    private QueryResult<List<DatabaseRow>> ExecuteDeleteFromStmt(DeleteFromStmt deleteFromStmt)
    {
        if (ActiveDbIsNull()) return _errorHandler.NoActiveDbSelected();

        var table = GetTable(deleteFromStmt.TableName);
        if (table is null)
        {
            return _errorHandler.TableDoesNotExist(deleteFromStmt.TableName);
        }

        table.Rows.Clear();

        return QueryResult<List<DatabaseRow>>.Ok(tableAffected: table);
    }
}