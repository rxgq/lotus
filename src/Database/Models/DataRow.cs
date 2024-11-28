namespace lotus.src.Database.Models;

public sealed class DatabaseRow
{
    public required Dictionary<string, object?> Values { get; set; }
}
