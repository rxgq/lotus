namespace lotus.src.Sql.Models;

public abstract class SqlStatement
{
}

public sealed class SelectStatement(List<string> values, FromStatement fromStmt, bool isDistinct) : SqlStatement
{
    public List<string> Values { get; set; } = values;
    public FromStatement FromStmt { get; set; } = fromStmt;
    public bool IsDistinct { get; set; } = isDistinct;
}

public sealed class FromStatement(string table, LimitStmt? limitStmt) : SqlStatement
{
    public string Table { get; set; } = table;
    public LimitStmt? LimitStmt { get; set; } = limitStmt;
}

public sealed class CreateTableStatement(string tableName, List<ColumnDeclarationStatement> columns) : SqlStatement
{
    public string TableName { get; set; } = tableName;
    public List<ColumnDeclarationStatement> Columns { get; set; } = columns;
}

public sealed class CreateDatabaseStatement(string databaseName) : SqlStatement 
{
    public string DatabaseName { get; set; } = databaseName;
}

public sealed class ColumnDeclarationStatement(string columnName, string dataType) : SqlStatement
{
    public string ColumnName { get; set; } = columnName;
    public string DataType { get; set; }  = dataType;
}

public sealed class InsertIntoStatement(string tableName, List<string> columns, List<SqlToken> values) : SqlStatement 
{
    public string TableName { get; set; } = tableName;
    public List<string> Columns { get; set; } = columns;
    public List<SqlToken> Values { get; set; } = values;
}

public sealed class DropTableStatement(string tableName) : SqlStatement
{
    public string TableName { get; set; } = tableName;
}

public sealed class DropDatabaseStatement(string databaseName) : SqlStatement
{
    public string DatabaseName { get; set; } = databaseName;
}

public abstract class AlterTableStatement(string tableName) : SqlStatement
{
    public string TableName { get; set; } = tableName;
}

public sealed class AddColumnStatement(string tableName, string columnName, string dataType) : AlterTableStatement(tableName)
{
    public string ColumnName { get; set; } = columnName;
    public string DataType { get; set; } = dataType;
}

public sealed class DropColumnStatement(string tableName, string columnName) : AlterTableStatement(tableName)
{
    public string ColumnName { get; set; } = columnName;
}

public sealed class RenameColumnStatement(string tableName, string oldColumnName, string newColumnName) : AlterTableStatement(tableName)
{
    public string OldColumnName { get; set; } = oldColumnName;
    public string NewColumnName { get; set; } = newColumnName;
}

public sealed class AlterColumnStatement(string tableName, string columnName, string dataType) : AlterTableStatement(tableName)
{
    public string ColumnName { get; set; } = columnName;
    public string DataType { get; set; } = dataType;
}

public sealed class RenameTableStatement(string tableName, string newTableName) : AlterTableStatement(tableName)
{
    public string NewTableName { get; set; } = newTableName;
}


public sealed class DeleteFromStmt(string tableName) : SqlStatement
{
    public string TableName { get; set; } = tableName;
}

public sealed class LimitStmt(string count) : SqlStatement
{
    public string Count { get; set; } = count;
}

public sealed class UseStmt(string databaseName) : SqlStatement 
{
    public string DatabaseName { get; set; } = databaseName;
}

public sealed class WhereStmt(Expression condition) : SqlStatement
{
    public Expression Condition { get; set; } = condition;
}

public sealed class BadStatement(string literal) : SqlStatement
{
    public string Literal { get; set; } = literal;
}