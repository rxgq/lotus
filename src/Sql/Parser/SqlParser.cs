using lotus.src.Database.Enums;
using lotus.src.Sql.Enums;
using lotus.src.Sql.Mappers;
using lotus.src.Sql.Models;
using System;

namespace lotus.src.Sql.Parser;

public sealed class SqlParser(List<SqlToken> tokens)
{
    private readonly List<SqlToken> Tokens = tokens;
    private int Current = 0;

    private readonly List<SqlStatement> Statements = [];

    public List<SqlStatement> ParseStatements()
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
            SqlTokenType.Drop   => ParseDropStmt(),
            SqlTokenType.Alter  => ParseAlterStmt(),
            SqlTokenType.Delete => ParseDeleteStmt(),
            SqlTokenType.Use    => ParseUseStmt(),
            _ => new BadStatement(Tokens[Current].Literal)
        };

        Statements.Add(stmt);
    }

    private SelectStatement ParseSelectStmt()
    {
        Expect(SqlTokenType.Select);

        bool isDistinct = Match(SqlTokenType.Distinct);
        if (isDistinct) Advance();

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

        WhereStatement? whereStmt = null;
        if (Match(SqlTokenType.Where)) {
            whereStmt = ParseWhereStmt();
        }

        Current--;
        return new SelectStatement(values, fromStmt, whereStmt, isDistinct);
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

    private SqlStatement ParseCreateStmt()
    {
        Expect(SqlTokenType.Create);

        var action = Tokens[Current].Type;

        SqlStatement result = action switch
        {
            SqlTokenType.Table => ParseCreateTableStmt(),
            SqlTokenType.Database => ParseCreateDatabaseStmt(),
            _ => throw new Exception("")
        };

        return result;
    }

    private CreateTableStatement ParseCreateTableStmt() 
    {
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

    private CreateDatabaseStatement ParseCreateDatabaseStmt()
    {
        Expect(SqlTokenType.Database);

        var identifier = Tokens[Current].Literal;
        
        return new(identifier);
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

    private InsertIntoStatement ParseInsertStmt()
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

    private SqlStatement ParseDropStmt() 
    { 
        Expect(SqlTokenType.Drop);

        var action = Tokens[Current].Type;

        SqlStatement result = action switch
        {
            SqlTokenType.Table => ParseDropTableStmt(),
            SqlTokenType.Database => ParseDropDatabaseStmt(),
            _ => throw new Exception("")
        };

        return result;
    }

    private DropTableStatement ParseDropTableStmt()
    {
        Expect(SqlTokenType.Table);

        var identifier = Tokens[Current].Literal;

        return new(identifier);
    }

    private DropDatabaseStatement ParseDropDatabaseStmt()
    {
        Expect(SqlTokenType.Database);

        var identifier = Tokens[Current].Literal;

        return new(identifier);
    }

    private AlterTableStatement ParseAlterStmt()
    {
        Expect(SqlTokenType.Alter);
        Expect(SqlTokenType.Table);

        var tableName = Tokens[Current].Literal;
        Advance();

        var action = Tokens[Current].Type;

        AlterTableStatement result = action switch
        {
            SqlTokenType.Add => ParseAddColumnStmt(tableName),
            SqlTokenType.Drop => ParseDropColumnStmt(tableName),
            SqlTokenType.Rename => ParseRenameStmt(tableName),
            SqlTokenType.Alter => ParseAlterColumnStmt(tableName),
            _ => throw new Exception("")
        };

        return result;
    }

    private AlterTableStatement ParseRenameStmt(string tableName) 
    {
        Expect(SqlTokenType.Rename);

        var action = Tokens[Current].Type;
        AlterTableStatement result = action switch
        {
            SqlTokenType.To => ParseRenameTableStmt(tableName),
            SqlTokenType.Column => ParseRenameColumnStmt(tableName),
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
        Expect(SqlTokenType.Column);

        var oldColumnName = Tokens[Current].Literal;
        Advance();

        Expect(SqlTokenType.To);
        var newColumnName = Tokens[Current].Literal;

        return new(tableName, oldColumnName, newColumnName);
    }

    private RenameTableStatement ParseRenameTableStmt(string tableName)
    {
        Expect(SqlTokenType.To);

        var newTableName = Tokens[Current].Literal;
        Advance();

        return new(tableName, newTableName);
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

    private UseStmt ParseUseStmt() 
    {
        Expect(SqlTokenType.Use);

        var tableName = Tokens[Current].Literal;

        return new(tableName);
    }

    private WhereStatement ParseWhereStmt()
    {
        Expect(SqlTokenType.Where);

        var condition = ParseExpression();
        return new WhereStatement(condition);
    }

    private Expression ParseExpression()
    {
        var left = ParsePrimary();

        while (Current < Tokens.Count && IsLogicalOperator(Tokens[Current].Type))
        {
            var op = Tokens[Current].Literal;
            Advance();

            var right = ParseExpression();
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

        if (Tokens[Current].Type == SqlTokenType.Identifier)
        {
            var column = Tokens[Current].Literal;
            Advance();

            if (IsComparisonOperator(Tokens[Current].Type))
            {
                var op = Tokens[Current].Literal;
                Advance();

                if (Tokens[Current].Type == SqlTokenType.String ||
                    Tokens[Current].Type == SqlTokenType.Integer ||
                    Tokens[Current].Type == SqlTokenType.Float ||
                    Tokens[Current].Type == SqlTokenType.True ||
                    Tokens[Current].Type == SqlTokenType.False)
                {
                    var value = Tokens[Current].Literal;
                    Advance();

                    return new ComparisonExpression(column, op, value);
                }

                throw new Exception("Expected a literal value after the comparison operator.");
            }

            throw new Exception("Expected a comparison operator after column name.");
        }

        if (Tokens[Current].Type == SqlTokenType.String ||
            Tokens[Current].Type == SqlTokenType.Integer ||
            Tokens[Current].Type == SqlTokenType.Float ||
            Tokens[Current].Type == SqlTokenType.True ||
            Tokens[Current].Type == SqlTokenType.False)
        {
            var value = Tokens[Current].Literal;
            Advance();
            return new LiteralExpression(value);
        }

        throw new Exception($"Unexpected token: {Tokens[Current].Literal}");
    }

    private static bool IsLogicalOperator(SqlTokenType type)
    {
        return type == SqlTokenType.And || type == SqlTokenType.Or;
    }

    private static bool IsComparisonOperator(SqlTokenType type)
    {
        return type == SqlTokenType.Equals ||
               type == SqlTokenType.GreaterThan ||
               type == SqlTokenType.GreaterThanEq ||
               type == SqlTokenType.LessThanEq ||
               type == SqlTokenType.LessThan ||
               type == SqlTokenType.Like ||
               type == SqlTokenType.NotEquals;
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