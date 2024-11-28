using lotus.src.Database.Models;

namespace lotus.src.Database.Factories;

public sealed class DatabaseFactory
{
    public DatabaseModel Create(string name) 
    {
        return new()
        {
            Name = name,
            Tables = []
        };
    }
}
