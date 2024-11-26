using lotus.src.Enums;

namespace lotus.src.Sql.Mappers;

public static class DataTypeMapper
{
    public static DataColumnType Map(string type) 
    {
        return type switch
        {
            "varchar" => DataColumnType.VarChar,
            "int"     => DataColumnType.Int,
            "float"   => DataColumnType.Float,
            _         => DataColumnType.Bad
        };
    }
}
