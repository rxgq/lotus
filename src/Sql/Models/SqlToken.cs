﻿using lotus.src.Sql.Enums;

namespace lotus.src.Sql.Models;

public sealed class SqlToken()
{
    public required SqlTokenType Type { get; set; }
    public required string Literal { get; set; }
}