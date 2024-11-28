using lotus.src.Database.Enums;
using lotus.src.Database.Models;
using lotus.src.Sql.Mappers;
using lotus.src.Sql.Models;
using lotus.src.Sql.Utils;

namespace lotus.src.Sql.Engine;

public sealed partial class SqlEngine
{
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
            var type = DataTypeMapper.Map(column.DataType);
            if (type == DataColumnType.Bad) 
            {
                return _errorHandler.NotAValidDatatype(column.DataType);
            }

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
}
