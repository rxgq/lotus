namespace lotus.src.Models;

public sealed class DatabaseRow
{
    public required Dictionary<string, object?> Values { get; set; }
}
