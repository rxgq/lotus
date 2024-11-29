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

        if (lexer.LexerErrors.Count > 0) {
            return new(null!, lexer.LexerErrors);
        }

        var parser = new SqlParser(tokens);
        var statements = parser.ParseStatements();

        var sql = new SqlEngine(statements, this);
        var results = sql.ExecuteStatements();

        return results;
    }
}
