using lotus.src.Sql.Enums;
using lotus.src.Sql.Models;

namespace lotus.src.Sql.Parser;

public sealed class SqlParser(List<SqlToken> tokens)
{
    private readonly List<SqlToken> Tokens = tokens;
    private int Current = 0;

    private readonly List<SqlStatement> Statements = [];

    public List<SqlStatement> ParseSql()
    {
        while (Current < Tokens.Count)
        {
            ParseStmt();
            Advance();
        }

        return Statements;
    }

    private void ParseStmt()
    {
        SqlStatement stmt = Tokens[Current].Type switch
        {
            SqlTokenType.Select => ParseSelectStmt(),
            SqlTokenType.Create => ParseCreateStmt(),
            SqlTokenType.Insert => ParseInsertStmt(),
            SqlTokenType.Drop   => ParseDropTableStmt(),
            SqlTokenType.Alter  => ParseAlterStmt(),
            SqlTokenType.Delete => ParseDeleteStmt(),
            SqlTokenType.Where  => ParseWhereStmt(),
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
                Advance();

            var identifier = Tokens[Current];
            values.Add(identifier.Literal);

            if (identifier.Type == SqlTokenType.Star)
                break;

            Advance();
        }
        while (Tokens[Current].Type == SqlTokenType.Comma);

        Advance();
        var fromStmt = ParseFromStmt();

        return new SelectStatement(values, fromStmt);
    }

    private FromStatement ParseFromStmt()
    {
        Expect(SqlTokenType.From);

        var identifier = Tokens[Current];

        Advance();
        if (Match(SqlTokenType.Limit)) 
        {
            Advance();
            Expect(SqlTokenType.Limit);

            var count = Tokens[Current];

            return new FromStatement(identifier.Literal, new(count.Literal));
        }

        return new FromStatement(identifier.Literal, null);
    }

    private CreateTableStatement ParseCreateStmt()
    {
        Expect(SqlTokenType.Create);
        Expect(SqlTokenType.Table);

        var identifier = Tokens[Current].Literal;
        Advance();

        Expect(SqlTokenType.LeftParen);

        var columns = new List<ColumnDeclarationStatement>();
        while (Tokens[Current].Type != SqlTokenType.RightParen)
        {
            var column = ParseColumnDeclaration();
            columns.Add(column);
        }

        Expect(SqlTokenType.RightParen);
        Current--;

        return new CreateTableStatement(identifier, columns);
    }

    private ColumnDeclarationStatement ParseColumnDeclaration()
    {
        var identifier = Tokens[Current].Literal;
        Advance();

        var dataType = Tokens[Current].Literal;
        Advance();

        Expect(SqlTokenType.Comma);

        return new(identifier, dataType);
    }

    private InsertStatement ParseInsertStmt()
    {
        Expect(SqlTokenType.Insert);
        Expect(SqlTokenType.Into);

        var tableName = Tokens[Current].Literal;
        Advance();

        Expect(SqlTokenType.LeftParen);

        List<string> columns = [];
        while (Tokens[Current].Type != SqlTokenType.RightParen)
        {
            columns.Add(Tokens[Current].Literal);
            Advance();

            Expect(SqlTokenType.Comma);
        }

        Expect(SqlTokenType.RightParen);

        Expect(SqlTokenType.Values);
        Expect(SqlTokenType.LeftParen);

        List<SqlToken> values = [];
        while (Tokens[Current].Type != SqlTokenType.RightParen)
        {
            values.Add(Tokens[Current]);
            Advance();

            Expect(SqlTokenType.Comma);
        }

        Expect(SqlTokenType.RightParen);
        Current--;

        return new(tableName, columns, values);
    }

    private DropTableStatement ParseDropTableStmt()
    {
        Expect(SqlTokenType.Drop);
        Expect(SqlTokenType.Table);

        var identifier = Tokens[Current].Literal;

        return new(identifier);
    }

    private AlterTableStatement ParseAlterStmt()
    {
        Expect(SqlTokenType.Alter);
        Expect(SqlTokenType.Table);

        var identifier = Tokens[Current].Literal;
        Advance();

        var action = Tokens[Current].Type;

        AlterTableStatement result = action switch
        {
            SqlTokenType.Add => ParseAddColumnStmt(identifier),
            SqlTokenType.Drop => ParseDropColumnStmt(identifier),
            SqlTokenType.Rename => ParseRenameColumnStmt(identifier),
            SqlTokenType.Alter => ParseAlterColumnStmt(identifier),
            _ => throw new Exception("")
        };

        return result;
    }

    private AddColumnStatement ParseAddColumnStmt(string tableName)
    {
        Expect(SqlTokenType.Add);

        var columnName = Tokens[Current].Literal;
        Advance();

        var dataType = Tokens[Current].Literal;

        return new(tableName, columnName, dataType);
    }

    private DropColumnStatement ParseDropColumnStmt(string tableName)
    {
        Expect(SqlTokenType.Drop);
        Expect(SqlTokenType.Column);

        var columnName = Tokens[Current].Literal;

        return new(tableName, columnName);
    }

    private RenameColumnStatement ParseRenameColumnStmt(string tableName)
    {
        Expect(SqlTokenType.Rename);
        Expect(SqlTokenType.Column);

        var oldColumnName = Tokens[Current].Literal;
        Advance();

        Expect(SqlTokenType.To);
        var newColumnName = Tokens[Current].Literal;

        return new(tableName, oldColumnName, newColumnName);
    }

    private AlterColumnStatement ParseAlterColumnStmt(string tableName)
    {
        Expect(SqlTokenType.Alter);
        Expect(SqlTokenType.Column);

        var columnName = Tokens[Current].Literal;
        Advance();

        var dataType = Tokens[Current].Literal;

        return new(tableName, columnName, dataType);
    }

    private DeleteFromStmt ParseDeleteStmt()
    {
        Expect(SqlTokenType.Delete);
        Expect(SqlTokenType.From);

        var tableName = Tokens[Current].Literal;

        return new(tableName);
    }

    private LimitStmt ParseLimitStmt() 
    {
        Expect(SqlTokenType.Limit);
        var count = Tokens[Current].Literal;

        return new(count);
    }

    private WhereStmt ParseWhereStmt()
    {
        Expect(SqlTokenType.Where);

        var condition = ParseExpression();
        return new WhereStmt(condition);
    }

    private Expression ParseExpression()
    {
        var left = ParsePrimary();

        while (IsLogicalOperator(Tokens[Current].Type) || IsComparisonOperator(Tokens[Current].Type)) 
        { 
            var op = Tokens[Current].Literal;
            Advance();

            var right = ParsePrimary();
            left = new BinaryExpression(left, op, right);
        }

        return left;
    }

    private Expression ParsePrimary()
    {
        if (Tokens[Current].Type == SqlTokenType.LeftParen)
        {
            Advance();
            var expr = ParseExpression();
            Expect(SqlTokenType.RightParen);
            return expr;
        }

        if (Tokens[Current].Type == SqlTokenType.Identifier || 
            Tokens[Current].Type == SqlTokenType.String ||
            Tokens[Current].Type == SqlTokenType.Integer ||
            Tokens[Current].Type == SqlTokenType.Float)
        {
            var value = Tokens[Current].Literal;
            Advance();
            return new LiteralExpression(value);
        }

        if (IsComparisonOperator(Tokens[Current].Type))
        {
            var column = Tokens[Current - 1].Literal;
            var op = Tokens[Current].Literal;
            Advance();
            var value = Tokens[Current].Literal;
            Advance();
            return new ComparisonExpression(column, op, value);
        }

        throw new Exception("Unexpected token in expression.");
    }

    private static bool IsLogicalOperator(SqlTokenType type)
    {
        return type == SqlTokenType.And || type == SqlTokenType.Or || type == SqlTokenType.Not;
    }

    private static bool IsComparisonOperator(SqlTokenType type)
    {
        return type == SqlTokenType.Equals ||
               type == SqlTokenType.GreaterThan ||
               type == SqlTokenType.LessThan;
    }

    private void Advance()
    {
        Current++;
    }

    private bool Match(SqlTokenType type) 
    {
        if (Current >= Tokens.Count) return false;
        return Tokens[Current].Type == type;
    }

    private bool Expect(SqlTokenType type)
    {
        if (Tokens[Current].Type == type)
        {
            Current++;
            return true;
        }

        return false;
    }
}