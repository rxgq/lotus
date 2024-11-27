namespace lotus.src.Sql.Models;

public abstract class Expression { }

public class BinaryExpression(Expression left, string op, Expression right) : Expression
{
    public Expression Left { get; } = left;
    public string Operator { get; } = op;
    public Expression Right { get; } = right;
}

public class LiteralExpression(string value) : Expression
{
    public string Value { get; } = value;
}

public class ComparisonExpression(string column, string operator_, string value) : Expression
{
    public string Column { get; } = column;
    public string Operator { get; } = operator_;
    public string Value { get; } = value;
}
