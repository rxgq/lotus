using lotus.src.Models;

namespace lotus.src.Factories;

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
