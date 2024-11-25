namespace lotus.src.Models;

public sealed class DatabaseTable
{
    public required string Name { get; set; }
    public required List<DatabaseColumn> Columns { get; set; }
    public required List<DatabaseRow> Rows { get; set; }
}
