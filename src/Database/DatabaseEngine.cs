using lotus.src.Models;
using lotus.src.Sql;
using System.Data;

namespace lotus.src.Database;

public sealed class DatabaseEngine
{
    public readonly List<DatabaseTable> Tables = [];

    public void CreateTable(DatabaseTable table) 
    {
        Tables.Add(table);
    }

    public List<DatabaseRow> SelectFromTable(string name) 
    {
        var table = Tables.FirstOrDefault(x => x.Name == name);

        if (table is null) return [];
        return table.Rows;
    }

    public List<QueryResult<List<DatabaseRow>>> ParseSql(string source) 
    {
        var lexer = new SqlLexer(source);
        var tokens = lexer.Tokenize();

        var parser = new SqlParser(tokens);
        var statements = parser.ParseSql();

        var sqlEngine = new SqlEngine(statements, Tables);
        var results = sqlEngine.ExecuteStatements();

        return results;
    }
}
