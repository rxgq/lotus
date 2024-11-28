using lotus.src.Database.Models;
using lotus.src.Database.Utils;
using lotus.src.Sql.Engine;
using lotus.src.Sql.Parser;

namespace lotus.src.Database;

public sealed class DatabaseEngine
{
    public readonly List<DatabaseModel> Databases = [];
    public DatabaseModel? ActiveDatabase { get; set; }

    public ExecutionResult ParseSql(string source) 
    {
        var lexer = new SqlLexer(source);
        var tokens = lexer.Tokenize();

        var parser = new SqlParser(tokens);
        var statements = parser.ParseSql();

        var sqlEngine = new SqlEngine(statements, this);
        var results = sqlEngine.ExecuteStatements();

        return results;
    }
}
