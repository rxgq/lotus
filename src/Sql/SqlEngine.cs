using lotus.src.Models;
using lotus.src.Sql.Mappers;
using lotus.src.Sql.Models;
using System.Collections.Generic;
using System.Data;

namespace lotus.src.Sql;

public sealed class SqlEngine(List<SqlStatement> statements, List<DatabaseTable> tables)
{
    private readonly List<SqlStatement> Statements = statements;
    private readonly List<DatabaseTable> Tables = tables;

    private int Current = 0;

    public List<QueryResult<List<DatabaseRow>>> ExecuteStatements() 
    {
        var results = new List<QueryResult<List<DatabaseRow>>>();
        while (Current < Statements.Count) 
        {
            var stmt = Statements[Current];

            results.Add(Execute(stmt));
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
            _ => null
        };

        return result;
    }

    private QueryResult<List<DatabaseRow>> ExecuteSelectStmt(SelectStatement selectStmt) 
    {
        var columns = selectStmt.Values.ToList();

        var fromStmt = Statements[Current + 1] as FromStatement;
        var table = GetTable(fromStmt.Table);
        Current++;

        if (table is null) {
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

        return QueryResult<List<DatabaseRow>>.Ok(rows, tableAffected: table);
    }

    private QueryResult<List<DatabaseRow>> ExecuteCreateTableStmt(CreateTableStatement createStmt)
    {
        var tableName = createStmt.TableName;

        var tableExists = GetTable(tableName) is not null;
        if (tableExists) { 
            return QueryResult<List<DatabaseRow>>.Err($"table '{createStmt.TableName}' already exists.");
        }

        List<DatabaseColumn> columns = [];
        foreach (var column in createStmt.Columns) 
        {
            var dbColumn = new DatabaseColumn() {  
                DataType = DataTypeMapper.Map(column.DataType),
                Title = column.ColumnName
            };

            columns.Add(dbColumn);
        }

        var table = new DatabaseTable() { 
            Name = tableName,
            Columns = columns,
            Rows = []
        };

        Tables.Add(table);

        return QueryResult<List<DatabaseRow>>.Ok(
            message: $"created table '{tableName}'.",
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

        var dbRow = new DatabaseRow()
        {
            Values = new Dictionary<string, object>()
        };

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
        if (table is null) { 
            return QueryResult<List<DatabaseRow>>.Err($"table '{dropStmt.TableName}' does not exist.");
        }

        var tableAffected = new DatabaseTable() {
            Columns = [],
            Name = table.Name,
            Rows= []
        };

        Tables.Remove(table);

        return QueryResult<List<DatabaseRow>>.Ok(tableAffected: tableAffected);
    }

    private DatabaseTable? GetTable(string name) 
    {
        return Tables.FirstOrDefault(x => x.Name == name);
    }
}
