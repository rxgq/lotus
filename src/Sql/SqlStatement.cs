namespace lotus.src.Sql;

public abstract class SqlStatement
{

}

public sealed class SelectStatement(List<string> values) : SqlStatement 
{
    public List<string> Values = values;
}

public sealed class FromStatement(string table) : SqlStatement
{
    public string Table = table;
}

public sealed class BadStatement(string literal) : SqlStatement
{
    public string Literal = literal;
}