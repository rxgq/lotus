using lotus.src.Enums;

namespace lotus.src.Models;

public sealed class DatabaseColumn
{
    public required string Title { get; set; }
    public required DataColumnType DataType { get; set; }
}