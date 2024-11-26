using lotus.src.Sql.Enums;
using lotus.src.Sql.Models;

namespace lotus.src.Sql;

public sealed class SqlLexer(string source)
{
    private readonly string Source = source;
    private int Current = 0;

    private readonly List<SqlToken> Tokens = [];

    public readonly List<string> LexerWarnings = [];
    public readonly List<string> LexerErrors = [];

    private readonly Dictionary<string, SqlTokenType> Keywords = new() {
        { "select", SqlTokenType.Select },
        { "from", SqlTokenType.From },
        { "create", SqlTokenType.Create },
        { "table", SqlTokenType.Table },
        { "insert", SqlTokenType.Insert },
        { "into", SqlTokenType.Into },
        { "values", SqlTokenType.Values },
        { "drop", SqlTokenType.Drop },
    };

    public List<SqlToken> Tokenize()
    {
        while (Current < Source.Length)
        {
            while (Current < Source.Length && Source[Current] == ' ' || Source[Current] == '\n')
                Current++;

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
            char c when char.IsAsciiDigit(c) => ParseNumeric(),
            '\'' => ParseString(),
            '*'  => NewToken(SqlTokenType.Star, "*"),
            ','  => NewToken(SqlTokenType.Comma, ","),
            '('  => NewToken(SqlTokenType.LeftParen, "("),
            ')'  => NewToken(SqlTokenType.RightParen, ")"),
            _    => throw new Exception($"Unknown token: >{Source[Current]}<")
        };

        Tokens.Add(token);
    }

    private static SqlToken NewToken(SqlTokenType type, string literal) 
    {
        return new() {
            Type = type,
            Literal = literal,
        };
    }

    private SqlToken ParseString() 
    {
        var start = Current;

        Current++;
        while (Current < Source.Length && Source[Current] != '\'')
            Current++;

        var literal = Source[(start + 1)..Current];

        return new() 
        {  
            Literal = literal,
            Type = SqlTokenType.String
        };
    }

    private SqlToken ParseNumeric() 
    { 
        var start = Current;

        bool hasDecimal = false;
        while (char.IsAsciiDigit(Source[Current]) || Source[Current] == '.') {
            if (hasDecimal && Source[Current] == '.') { 
                // bad
            }

            if (Source[Current] == '.') hasDecimal = true;
            Current++;
        }

        var literal = Source[start..Current];
        Current--;

        return NewToken(hasDecimal ? SqlTokenType.Float : SqlTokenType.Integer, literal);
    }

    private SqlToken ParseIdentifier() 
    {
        var start = Current;

        while (Current < Source.Length && (char.IsAsciiLetterOrDigit(Source[Current]) || Source[Current] == '_')) 
            Current++;

        var literal = Source[start..Current];

        var type = SqlTokenType.Identifier;
        if (Keywords.TryGetValue(literal, out SqlTokenType value)) type = value;

        var token = NewToken(type, Source[start..Current]);

        Current--;

        return token;
    }
}