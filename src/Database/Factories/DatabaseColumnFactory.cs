using lotus.src.Database.Models;
using lotus.src.Sql.Mappers;

namespace lotus.src.Database.Factories;

public sealed class DatabaseColumnFactory
{
    public DatabaseColumn Create(string title, string dataType)
    {
        return new()
        {
            DataType = DataTypeMapper.Map(dataType),
            Title = title
        };
    }
}
