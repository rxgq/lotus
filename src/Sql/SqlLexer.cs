using lotus.src.Sql.Enums;

namespace lotus.src.Sql;

public sealed class SqlLexer(string source)
{
    private readonly string Source = source;
    private int Current = 0;

    private List<SqlToken> Tokens = [];

    private readonly Dictionary<string, SqlTokenType> Keywords = new() {
        { "select", SqlTokenType.Select },
        { "from", SqlTokenType.From },
        { "create", SqlTokenType.Create },
        { "table", SqlTokenType.Table },
    };

    public List<SqlToken> Tokenize()
    {
        while (Current < Source.Length)
        {
            ParseToken();
            Current++;
        }

        return Tokens;
    }

    private void ParseToken() 
    {
        var token = Source[Current] switch
        {
            char c when char.IsAsciiLetter(c) => ParseIdentifier(),
            '*' => NewToken(SqlTokenType.Star, "*"),
            ',' => NewToken(SqlTokenType.Comma, ","),
            '(' => NewToken(SqlTokenType.LeftParen, "("),
            ')' => NewToken(SqlTokenType.RightParen, ")"),
            _ => null
        };

        if (token is not null) 
        { 
            Tokens.Add(token);
        }
    }

    private static SqlToken NewToken(SqlTokenType type, string literal) 
    {
        return new() {
            Type = type,
            Literal = literal,
        };
    }

    private SqlToken ParseIdentifier() 
    {
        var start = Current;

        while (Current < Source.Length && char.IsAsciiLetterOrDigit(Source[Current])) 
            Current++;

        var literal = Source[start..Current];

        var type = SqlTokenType.Identifier;
        if (Keywords.TryGetValue(literal, out SqlTokenType value)) type = value;

        var token = new SqlToken() { 
            Literal = Source[start..Current],
            Type = type
        };

        Current--;

        return token;
    }
}