using lotus.src.Enums;
using lotus.src.Models;

namespace lotus.src.Sql.Models;

public abstract class SqlStatement
{
}

public sealed class SelectStatement(List<string> values) : SqlStatement
{
    public List<string> Values { get; set; } = values;
}

public sealed class FromStatement(string table) : SqlStatement
{
    public string Table { get; set; } = table;
}

public sealed class CreateTableStatement(string tableName, List<ColumnDeclarationStatement> columns) : SqlStatement
{
    public string TableName { get; set; } = tableName;
    public List<ColumnDeclarationStatement> Columns { get; set; } = columns;
}

public sealed class ColumnDeclarationStatement(string columnName, string dataType) : SqlStatement
{
    public string ColumnName { get; set; } = columnName;
    public string DataType { get; set; }  = dataType;
}

public sealed class InsertStatement(string tableName, List<string> columns, List<SqlToken> values) : SqlStatement 
{
    public string TableName { get; set; } = tableName;
    public List<string> Columns { get; set; } = columns;
    public List<SqlToken> Values { get; set; } = values;
}

public sealed class BadStatement(string literal) : SqlStatement
{
    public string Literal { get; set; } = literal;
}