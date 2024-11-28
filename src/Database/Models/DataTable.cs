namespace lotus.src.Database.Models;

public sealed class DatabaseTable
{
    public required string Name { get; set; }
    public required List<DatabaseColumn> Columns { get; set; }
    public required List<DatabaseRow> Rows { get; set; }

    public bool HasColumn(string title)
    {
        return Columns.Any(x => x.Title == title);
    }

    public DatabaseColumn? GetColumn(string title)
    {
        return Columns.FirstOrDefault(x => x.Title == title);
    }
}