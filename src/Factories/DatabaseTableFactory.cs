using lotus.src.Models;

namespace lotus.src.Factories;

public sealed class DatabaseTableFactory
{
    public DatabaseTable Create(string name, List<DatabaseColumn> columns, List<DatabaseRow> rows) 
    {
        return new()
        {
            Name = name,
            Columns = columns,
            Rows = rows,
        };
    }
}
