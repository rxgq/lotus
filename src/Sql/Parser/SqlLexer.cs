using lotus.src.Sql.Enums;
using lotus.src.Sql.Models;

namespace lotus.src.Sql.Parser;

public sealed class SqlLexer(string source)
{
    private readonly string Source = source;
    private int Current = 0;

    private readonly List<SqlToken> Tokens = [];

    public readonly List<string> LexerWarnings = [];
    public readonly List<string> LexerErrors = [];

    private readonly Dictionary<string, SqlTokenType> Keywords = new() {
        { "select",   SqlTokenType.Select },
        { "from",     SqlTokenType.From },
        { "create",   SqlTokenType.Create },
        { "table",    SqlTokenType.Table },
        { "insert",   SqlTokenType.Insert },
        { "into",     SqlTokenType.Into },
        { "values",   SqlTokenType.Values },
        { "drop",     SqlTokenType.Drop },
        { "alter",    SqlTokenType.Alter },
        { "column",   SqlTokenType.Column },
        { "add",      SqlTokenType.Add },
        { "rename",   SqlTokenType.Rename },
        { "to",       SqlTokenType.To },
        { "delete",   SqlTokenType.Delete },
        { "where",    SqlTokenType.Where },
        { "and",      SqlTokenType.And },
        { "or",       SqlTokenType.Or },
        { "not",      SqlTokenType.Not },
        { "limit",    SqlTokenType.Limit },
        { "distinct", SqlTokenType.Distinct },
        { "use",      SqlTokenType.Use },
        { "database", SqlTokenType.Database },
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
            char c when char.IsAsciiDigit(c) => ParseNumeric(),
            '\'' => ParseString(),
            '*'  => NewToken(SqlTokenType.Star, "*"),
            ','  => NewToken(SqlTokenType.Comma, ","),
            '('  => NewToken(SqlTokenType.LeftParen, "("),
            ')'  => NewToken(SqlTokenType.RightParen, ")"),
            '<'  => NewToken(SqlTokenType.LessThan, "<"),
            '>'  => NewToken(SqlTokenType.GreaterThan, ">"),
            '='  => NewToken(SqlTokenType.Equals, "="),
            '\n' or '\r' or ' ' => null,
            _    => throw new Exception($"Unknown token: >{Source[Current]}<")
        };

        if (token is null) return;
        Tokens.Add(token);
    }

    private static SqlToken NewToken(SqlTokenType type, string literal)
    {
        return new()
        {
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
        while (Current < Source.Length && (char.IsAsciiDigit(Source[Current]) || Source[Current] == '.'))
        {
            if (hasDecimal && Source[Current] == '.')
            {
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
        if (Keywords.TryGetValue(literal.ToLower(), out SqlTokenType value)) 
            type = value;

        var token = NewToken(type, Source[start..Current]);

        Current--;

        return token;
    }
}