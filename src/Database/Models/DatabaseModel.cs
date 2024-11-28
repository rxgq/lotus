namespace lotus.src.Database.Models;

public sealed class DatabaseModel
{
    public required List<DatabaseTable> Tables { get; set; }
    public required string Name { get; set; }
}
