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
    Like,

    // Symbols
    Star,
    Comma,
    SingleQuote,
    LeftParen,
    RightParen,
    Exclamation,

    // Logical
    LessThan,
    LessThanEq,
    GreaterThan,
    GreaterThanEq,
    Equals,
    NotEquals,
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
    True,
    False,
    WhiteSpace
}