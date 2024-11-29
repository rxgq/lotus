using lotus.src.Sql.Enums;
using lotus.src.Sql.Models;
using System.Text;

namespace lotus.src.Sql.Parser;

public sealed class SqlLexer(string source)
{
    private readonly string Source = source;
    private int Current = 0;
    private int Line = 1;

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
        { "true",     SqlTokenType.True },
        { "false",    SqlTokenType.False },
        { "like",     SqlTokenType.Like },
    };

    public List<SqlToken> Tokenize()
    {
        while (Current < Source.Length)
        {
            ParseToken();
            Current++;

            if (LexerErrors.Count > 0) return [];
        }

        return Tokens;
    }

    private void ParseToken()
    {
        if (Source[Current] == '\n') Line++;

        var token = Source[Current] switch
        {
            char c when char.IsAsciiLetter(c) => ParseIdentifier(),
            char c when char.IsAsciiDigit(c) => ParseNumeric(),
            '\'' => ParseString(),
            '*'  => NewToken(SqlTokenType.Star, "*"),
            ','  => NewToken(SqlTokenType.Comma, ","),
            '('  => NewToken(SqlTokenType.LeftParen, "("),
            ')'  => NewToken(SqlTokenType.RightParen, ")"),
            '<'  => Peek() == '=' ? NewToken(SqlTokenType.LessThanEq, "<=") : NewToken(SqlTokenType.LessThan, "<"),
            '>'  => Peek() == '=' ? NewToken(SqlTokenType.GreaterThanEq, ">=") : NewToken(SqlTokenType.GreaterThan, ">"),
            '='  => NewToken(SqlTokenType.Equals, "="),
            '!'  => Peek() == '=' ? NewToken(SqlTokenType.NotEquals, "!=") : NewToken(SqlTokenType.Exclamation, "!"),
            '\n' or '\r' or ' ' => null,
            _    => NewToken(SqlTokenType.Bad, "")
        };

        if (token is null) return;

        if (token?.Type == SqlTokenType.Bad) 
        {
            Error($"Unexpected token '{Source[Current]}'");
        }

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

        var literal = new StringBuilder();

        while (Current < Source.Length)
        {
            if (Source[Current] == '\'' && Current + 1 < Source.Length && Source[Current + 1] == '\'')
            {
                literal.Append('\'');
                Current += 2;
            }
            else if (Source[Current] == '\'')
            {
                Current++;
                return new SqlToken
                {
                    Literal = literal.ToString(),
                    Type = SqlTokenType.String
                };
            }
            else
            {
                literal.Append(Source[Current]);
                Current++;
            }
        }

        Error("Unterminated string literal");
        return new SqlToken
        {
            Literal = literal.ToString(),
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
                Error($"Syntax Error");
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

    private char Peek() {
        return Current < Source.Length ? Source[++Current] : '\0'; 
    }

    private void Error(string message) 
    {
        LexerErrors.Add($"{message} on line {Line}");
    }
}