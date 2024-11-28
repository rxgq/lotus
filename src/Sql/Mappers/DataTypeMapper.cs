using lotus.src.Database.Enums;

namespace lotus.src.Sql.Mappers;

public static class DataTypeMapper
{
    public static DataColumnType Map(string type) 
    {
        return type.ToLower() switch
        {
            "varchar"     => DataColumnType.VarChar,
            "int"         => DataColumnType.Int,
            "float"       => DataColumnType.Float,
            "datestamp"   => DataColumnType.DateStamp,
            _             => DataColumnType.Bad
        };
    }
}
