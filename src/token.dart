enum TokenType {
  select,
  where,
  identifier,
  whitespace,
  singleEquals,
  dot,
  string,
  from,
  and,
  or,
  not,
  star,
  order_by,
  eof
}

const Map<String, TokenType> tokenMap = {
  "select": TokenType.select,
  "where": TokenType.where,
  "and": TokenType.and,
  "or": TokenType.or,
  "not": TokenType.not,
  "from": TokenType.from,
};

final class Token {
  final String lexeme;
  final TokenType type;

  Token({
    required this.lexeme,
    required this.type,
  });

  @override
  String toString() {
    return "$type: >$lexeme<";
  }
}