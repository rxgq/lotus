using lotus.src.Factories;
using lotus.src.Models;
using lotus.src.Sql.Models;
using lotus.src.Sql.Utils;

namespace lotus.src.Sql.Engine;

public sealed class SqlEngine(List<SqlStatement> statements, List<DatabaseTable> tables)
{
    private readonly DatabaseColumnFactory _columnFactory = new();
    private readonly DatabaseRowFactory _rowFactory = new();
    private readonly DatabaseTableFactory _tableFactory = new();

    private readonly List<SqlStatement> Statements = statements;
    private readonly List<DatabaseTable> Tables = tables;

    private int Current = 0;

    public List<QueryResult<List<DatabaseRow>>> ExecuteStatements()
    {
        var results = new List<QueryResult<List<DatabaseRow>>>();
        while (Current < Statements.Count)
        {
            results.Add(Execute(Statements[Current]));
            Current++;
        }

        return results;
    }

    private QueryResult<List<DatabaseRow>> Execute(SqlStatement stmt)
    {
        var result = stmt switch
        {
            _ when stmt is SelectStatement select => ExecuteSelectStmt(select),
            _ when stmt is CreateTableStatement create => ExecuteCreateTableStmt(create),
            _ when stmt is InsertStatement insert => ExecuteInsertStmt(insert),
            _ when stmt is DropTableStatement drop => ExecuteDropTableStmt(drop),
            _ when stmt is AlterTableStatement alter => ExecuteAlterTableStmt(alter),
            _ when stmt is DeleteFromStmt delete => ExecuteDeleteFromStmt(delete),
            _ => throw new Exception($"Unknown SQL expression {stmt}")
        };

        return result;
    }

    private QueryResult<List<DatabaseRow>> ExecuteSelectStmt(SelectStatement selectStmt)
    {
        var columns = selectStmt.Values.ToList();

        var fromStmt = Statements[Current + 1] as FromStatement;
        var table = GetTable(fromStmt.Table);
        Current++;

        if (table is null)
        {
            return QueryResult<List<DatabaseRow>>.Err($"table '{fromStmt.Table}' does not exist.");
        }

        List<DatabaseRow> rows = [];
        if (columns[0] == "*")
        {
            rows = table.Rows;
        }
        else
        {
            // handle specific columns being selected
        }

        if (fromStmt.LimitStmt is not null) 
        {
            var isDigit = int.TryParse(fromStmt.LimitStmt.Count, out int count);

            if (!isDigit || count < 0) 
            { 
                return QueryResult<List<DatabaseRow>>.Err($"'{fromStmt.LimitStmt.Count}' is not valid in this expression.");
            }

            rows = rows.Take(count).ToList();
        }

        return QueryResult<List<DatabaseRow>>.Ok(rows, tableAffected: table);
    }

    private QueryResult<List<DatabaseRow>> ExecuteCreateTableStmt(CreateTableStatement createStmt)
    {
        var tableExists = GetTable(createStmt.TableName) is not null;
        if (tableExists)
        {
            return QueryResult<List<DatabaseRow>>.Err($"table '{createStmt.TableName}' already exists.");
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

    private QueryResult<List<DatabaseRow>> ExecuteInsertStmt(InsertStatement insertStmt)
    {
        var table = GetTable(insertStmt.TableName);
        if (table is null)
        {
            return QueryResult<List<DatabaseRow>>.Err($"table '{insertStmt.TableName}' does not exist.");
        }

        var dbRow = _rowFactory.Create([]);

        var providedColumns = new HashSet<string>(insertStmt.Columns);

        foreach (var column in table.Columns)
        {
            var columnIndex = insertStmt.Columns.IndexOf(column.Title);

            if (columnIndex >= 0)
            {
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
        var table = GetTable(dropStmt.TableName);
        if (table is null)
        {
            return QueryResult<List<DatabaseRow>>.Err($"table '{dropStmt.TableName}' does not exist.");
        }

        // creates a new reference to the table before it is deleted
        // this allows the 'tableAffected' to be set accordingly
        var tableAffected = _tableFactory.Create(table.Name, [], []);

        Tables.Remove(table);

        return QueryResult<List<DatabaseRow>>.Ok(tableAffected: tableAffected);
    }

    private QueryResult<List<DatabaseRow>> ExecuteAlterTableStmt(AlterTableStatement alterStmt)
    {
        var table = GetTable(alterStmt.TableName);
        if (table is null)
        {
            return QueryResult<List<DatabaseRow>>.Err($"table '{alterStmt.TableName}' does not exist.");
        }

        var result = alterStmt switch
        {
            _ when alterStmt is AddColumnStatement addColumn => ExecuteAddColumnStmt(addColumn, table),
            _ when alterStmt is DropColumnStatement dropColumn => ExecuteDropColumnStmt(dropColumn, table),
            _ when alterStmt is RenameColumnStatement renameColumn => ExecuteRenameColumnStmt(renameColumn, table),
            _ when alterStmt is AlterColumnStatement alterColumn => ExecuteAlterColumnStmt(alterColumn, table),
        };

        return result;
    }

    private QueryResult<List<DatabaseRow>> ExecuteAddColumnStmt(AddColumnStatement addColumnStmt, DatabaseTable table)
    {
        if (table.HasColumn(addColumnStmt.ColumnName)) 
        {
            return QueryResult<List<DatabaseRow>>.Err($"column '{addColumnStmt.ColumnName}' already exists.");
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
            return QueryResult<List<DatabaseRow>>.Err($"column '{dropColumnStmt.ColumnName}' does not exist.");
        }

        table.Columns.Remove(column);

        return QueryResult<List<DatabaseRow>>.Ok(tableAffected: table);
    }

    private QueryResult<List<DatabaseRow>> ExecuteRenameColumnStmt(RenameColumnStatement renameColumnStmt, DatabaseTable table)
    {
        var column = table.GetColumn(renameColumnStmt.OldColumnName);
        if (column is null)
        {
            return QueryResult<List<DatabaseRow>>.Err($"column '{renameColumnStmt.OldColumnName}' does not exist.");
        }

        column.Title = renameColumnStmt.NewColumnName;

        return QueryResult<List<DatabaseRow>>.Ok(tableAffected: table);
    }

    private QueryResult<List<DatabaseRow>> ExecuteAlterColumnStmt(AlterColumnStatement alterColumnStmt, DatabaseTable table)
    {
        var column = table.GetColumn(alterColumnStmt.ColumnName);
        if (column is null)
        {
            return QueryResult<List<DatabaseRow>>.Err($"column '{alterColumnStmt.ColumnName}' does not exist.");
        }

        return QueryResult<List<DatabaseRow>>.Ok(tableAffected: table);
    }

    private QueryResult<List<DatabaseRow>> ExecuteDeleteFromStmt(DeleteFromStmt deleteFromStmt)
    {
        var table = GetTable(deleteFromStmt.TableName);
        if (table is null)
        {
            return QueryResult<List<DatabaseRow>>.Err($"table '{deleteFromStmt.TableName}' does not exist.");
        }

        table.Rows.Clear();

        return QueryResult<List<DatabaseRow>>.Ok(tableAffected: table);
    }

    private DatabaseTable? GetTable(string name)
    {
        return Tables.FirstOrDefault(x => x.Name == name);
    }
}