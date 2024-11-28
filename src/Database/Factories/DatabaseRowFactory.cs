using lotus.src.Database.Models;

namespace lotus.src.Database.Factories;

public sealed class DatabaseRowFactory
{
    public DatabaseRow Create(Dictionary<string, object?> values)
    {
        return new()
        {
            Values = values
        };
    }
}
