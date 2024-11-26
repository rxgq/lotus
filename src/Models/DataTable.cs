﻿namespace lotus.src.Models;

public sealed class DatabaseTable
{
    public required string Name { get; set; }
    public required List<DatabaseColumn> Columns { get; set; }
    public required List<DatabaseRow> Rows { get; set; }

    public bool HasColumn(string title) 
    {
        return Columns.Any(x => x.Title == title);
    }
}