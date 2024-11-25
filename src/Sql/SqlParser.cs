using lotus.src.Sql.Enums;

namespace lotus.src.Sql;

public sealed class SqlParser(List<SqlToken> tokens)
{
    private readonly List<SqlToken> Tokens = tokens;
    private int Current = 0;

    private List<SqlStatement> Statements = [];

    public List<SqlStatement> ParseSql() {
        while (Current < Tokens.Count) {
            ParseStmt();
            Current++;
        }

        return Statements;
    }

    private void ParseStmt() 
    {
        SqlStatement stmt = Tokens[Current].Type switch 
        { 
            SqlTokenType.Select => ParseSelectStmt(),
            SqlTokenType.From => ParseFromStmt(),
            _ => new BadStatement(Tokens[Current].Literal)
        };

        Statements.Add(stmt);
    }

    private SelectStatement ParseSelectStmt()
    {
        Expect(SqlTokenType.Select);

        List<string> values = [];
        do
        {
            if (Tokens[Current].Type == SqlTokenType.Comma)
                Current++;

            var identifier = Tokens[Current];
            values.Add(identifier.Literal);

            if (identifier.Type == SqlTokenType.Star) 
                break;

            Current++;
        } 
        while (Tokens[Current].Type == SqlTokenType.Comma);

        return new SelectStatement(values);
    }

    private FromStatement ParseFromStmt()
    {
        Expect(SqlTokenType.From);

        var identifier = Tokens[Current];
        return new FromStatement(identifier.Literal);
    }

    private bool Expect(SqlTokenType type) {
        if (Tokens[Current].Type == type) 
        {
            Current++;
            return true;
        }

        return false;
    }
}