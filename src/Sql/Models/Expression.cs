namespace lotus.src.Sql.Models;

public abstract class Expression { }

public sealed class BinaryExpression(Expression left, string op, Expression right) : Expression
{
    public Expression Left { get; } = left;
    public string Operator { get; } = op;
    public Expression Right { get; } = right;
}

public sealed class LiteralExpression(string value) : Expression
{
    public string Value { get; } = value;
}

public sealed class ComparisonExpression(string column, string op, string value) : Expression
{
    public string Column { get; } = column;
    public string Operator { get; } = op;
    public string Value { get; } = value;
}
