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

    private QueryResult<List<DatabaseRow>> ExecuteCreateTableStmt(CreateTableStatement createStmt)
    {
        if (ActiveDbIsNull()) return _errorHandler.NoActiveDbSelected();

        var tableExists = GetTable(createStmt.TableName) is not null;
        if (tableExists)
        {
            return _errorHandler.TableAlreadyExists(createStmt.TableName);
        }

        List<DatabaseColumn> columns = [];
        foreach (var column in createStmt.Columns)
        {
            var dbColumn = _columnFactory.Create(column.ColumnName, column.DataType);
            columns.Add(dbColumn);
        }

        var table = _tableFactory.Create(createStmt.TableName, columns, []);
        Tables.Add(table);

        return QueryResult<List<DatabaseRow>>.Ok(
            message: $"created table '{createStmt.TableName}'.",
            tableAffected: table
        );
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

    private QueryResult<List<DatabaseRow>> ExecuteDropTableStmt(DropTableStatement dropStmt)
    {
        if (ActiveDbIsNull()) return _errorHandler.NoActiveDbSelected();

        var table = GetTable(dropStmt.TableName);
        if (table is null)
        {
            return _errorHandler.TableDoesNotExist(dropStmt.TableName);
        }

        // creates a new reference to the table before it is deleted
        // this allows the 'tableAffected' to be set accordingly
        var tableAffected = _tableFactory.Create(table.Name, [], []);

        Tables.Remove(table);

        return QueryResult<List<DatabaseRow>>.Ok(tableAffected: tableAffected);
    }

    private QueryResult<List<DatabaseRow>> ExecuteAlterTableStmt(AlterTableStatement alterStmt)
    {
        if (ActiveDbIsNull()) return _errorHandler.NoActiveDbSelected();

        var table = GetTable(alterStmt.TableName);
        if (table is null)
        {
            return _errorHandler.TableDoesNotExist(alterStmt.TableName);
        }

        var result = alterStmt switch
        {
            _ when alterStmt is AddColumnStatement addColumn => ExecuteAddColumnStmt(addColumn, table),
            _ when alterStmt is DropColumnStatement dropColumn => ExecuteDropColumnStmt(dropColumn, table),
            _ when alterStmt is RenameColumnStatement renameColumn => ExecuteRenameColumnStmt(renameColumn, table),
            _ when alterStmt is RenameTableStatement renameTable => ExecuteRenameTableStmt(renameTable, table),
            _ when alterStmt is AlterColumnStatement alterColumn => ExecuteAlterColumnStmt(alterColumn, table),
            _ => _errorHandler.UnknownExpression(alterStmt.ToString() ?? "")
        };

        return result;
    }

    private QueryResult<List<DatabaseRow>> ExecuteAddColumnStmt(AddColumnStatement addColumnStmt, DatabaseTable table)
    {
        if (table.HasColumn(addColumnStmt.ColumnName)) 
        {
            return _errorHandler.ColumnAlreadyExists(addColumnStmt.ColumnName);
        }

        var column = _columnFactory.Create(addColumnStmt.ColumnName, addColumnStmt.DataType);
        table.Columns.Add(column);

        return QueryResult<List<DatabaseRow>>.Ok(tableAffected: table);
    }

    private QueryResult<List<DatabaseRow>> ExecuteDropColumnStmt(DropColumnStatement dropColumnStmt, DatabaseTable table)
    {
        var column = table.GetColumn(dropColumnStmt.ColumnName);
        if (column is null)
        {
            return _errorHandler.ColumnDoesNotExist(dropColumnStmt.ColumnName);
        }

        table.Columns.Remove(column);

        return QueryResult<List<DatabaseRow>>.Ok(tableAffected: table);
    }

    private QueryResult<List<DatabaseRow>> ExecuteRenameColumnStmt(RenameColumnStatement renameColumnStmt, DatabaseTable table)
    {
        var column = table.GetColumn(renameColumnStmt.OldColumnName);
        if (column is null)
        {
            return _errorHandler.ColumnDoesNotExist(renameColumnStmt.OldColumnName);
        }

        column.Title = renameColumnStmt.NewColumnName;

        foreach (var row in table.Rows)
        {
            if (!row.Values.ContainsKey(renameColumnStmt.OldColumnName))
                continue;

            var value = row.Values[renameColumnStmt.OldColumnName];
            row.Values.Remove(renameColumnStmt.OldColumnName);
            row.Values[renameColumnStmt.NewColumnName] = value;
        }

        return QueryResult<List<DatabaseRow>>.Ok(tableAffected: table);
    }

    private QueryResult<List<DatabaseRow>> ExecuteRenameTableStmt(RenameTableStatement renameTableStmt, DatabaseTable table)
    {
        var tableExists = GetTable(renameTableStmt.TableName) is not null;
        if (!tableExists)
        {
            return _errorHandler.TableDoesNotExist(renameTableStmt.TableName);
        }

        table.Name = renameTableStmt.NewTableName;

        return QueryResult<List<DatabaseRow>>.Ok(tableAffected: table);
    }

    private QueryResult<List<DatabaseRow>> ExecuteAlterColumnStmt(AlterColumnStatement alterColumnStmt, DatabaseTable table)
    {
        var column = table.GetColumn(alterColumnStmt.ColumnName);
        if (column is null)
        {
            return _errorHandler.ColumnDoesNotExist(alterColumnStmt.ColumnName);
        }

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