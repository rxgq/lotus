using lotus.src.Models;
using System.Collections.Generic;

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
            _ => null
        };

        return result;
    }

    private QueryResult<List<DatabaseRow>> ExecuteSelectStmt(SelectStatement selectStmt) 
    {
        var columns = selectStmt.Values.ToList();

        var fromStmt = Statements[Current + 1] as FromStatement;
        var table = Tables.FirstOrDefault(x => x.Name == fromStmt.Table);
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
            
        }

        return QueryResult<List<DatabaseRow>>.Ok(rows);
    }
}
