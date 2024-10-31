import 'expression.dart';
import 'token.dart';

final class Parser {
  final List<Token> tokens;
  final List<Expression> expressions = [];

  int current = 0;

  Token get currTok => tokens[current];

  Parser({
    required this.tokens
  });

  List<Expression> parse() {
    while (currTok.type != TokenType.eof) {
      advance();
    }

    return expressions;
  }

  void advance() {
    current++;
  }
}