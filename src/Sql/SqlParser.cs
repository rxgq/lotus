using lotus.src.Sql.Enums;
using lotus.src.Sql.Models;

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
            SqlTokenType.Create => ParseFromCreateStmt(),
            SqlTokenType.Insert => ParseInsertStmt(),
            SqlTokenType.Drop => ParseDropTable(),
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

    private CreateTableStatement ParseFromCreateStmt() 
    {
        Expect(SqlTokenType.Create);
        Expect(SqlTokenType.Table);

        var identifier = Tokens[Current].Literal;
        Current++;

        Expect(SqlTokenType.LeftParen);

        var columns = new List<ColumnDeclarationStatement>();
        while (Tokens[Current].Type != SqlTokenType.RightParen) {
            var column = ParseColumnDeclaration();
            columns.Add(column);
        }

        Expect(SqlTokenType.RightParen);

        return new CreateTableStatement(identifier, columns);
    }

    private ColumnDeclarationStatement ParseColumnDeclaration() 
    { 
        var identifier = Tokens[Current].Literal;
        Current++;

        var dataType = Tokens[Current].Literal;
        Current++;

        Expect(SqlTokenType.Comma);

        return new(identifier, dataType);
    }

    private InsertStatement ParseInsertStmt() 
    {
        Expect(SqlTokenType.Insert);
        Expect(SqlTokenType.Into);

        var tableName = Tokens[Current].Literal;
        Current++;

        Expect(SqlTokenType.LeftParen);

        List<string> columns = [];
        while (Tokens[Current].Type != SqlTokenType.RightParen) 
        {
            columns.Add(Tokens[Current].Literal);
            Current++;

            Expect(SqlTokenType.Comma);
        }

        Expect(SqlTokenType.RightParen);

        Expect(SqlTokenType.Values);
        Expect(SqlTokenType.LeftParen);

        List<SqlToken> values = [];
        while (Tokens[Current].Type != SqlTokenType.RightParen) 
        {
            values.Add(Tokens[Current]);
            Current++;

            Expect(SqlTokenType.Comma);
        }

        Expect(SqlTokenType.RightParen);

        return new(tableName, columns, values);
    }

    private DropTableStatement ParseDropTable() 
    {
        Expect(SqlTokenType.Drop);
        Expect(SqlTokenType.Table);

        var identifier = Tokens[Current].Literal;

        return new(identifier);
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