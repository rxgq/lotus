using lotus.src.Database.Enums;

namespace lotus.src.Database.Models;

public sealed class DatabaseColumn
{
    public required string Title { get; set; }
    public required DataColumnType DataType { get; set; }
}