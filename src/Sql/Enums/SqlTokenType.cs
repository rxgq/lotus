namespace lotus.src.Sql.Enums;

public enum SqlTokenType
{
    // Symbols
    Select,
    From,
    Create,
    Table,
    Insert,
    Into,
    Values,
    Drop,
    Alter,
    Add,
    Column,
    Rename,
    To,
    Delete,
    Where,
    Limit,
    Distinct,
    Use,
    Database,

    // Symbols
    Star,
    Comma,
    SingleQuote,
    LeftParen,
    RightParen,

    // Logical
    LessThan,
    GreaterThan,
    Equals,
    And,
    Or,
    Not,

    // Data types
    String,
    Integer,
    Float,
    Bool,

    // Misc
    Identifier,
    WhiteSpace
}