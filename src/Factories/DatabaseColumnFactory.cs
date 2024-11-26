using lotus.src.Models;
using lotus.src.Sql.Mappers;

namespace lotus.src.Factories;

public sealed class DatabaseColumnFactory
{
    public DatabaseColumn Create(string title, string dataType) 
    {
        return new() {
            DataType = DataTypeMapper.Map(dataType),
            Title = title
        };
    }
}
