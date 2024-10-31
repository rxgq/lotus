import 'Token.dart';

final class Lexer {
  final String source;
  final List<Token> tokens = [];

  int current = 0;

  String get lexCurr => current < source.length ? source[current] : '';

  Lexer({required this.source});

  List<Token> tokenize() {
    while (current < source.length) {
      if (isWhitespace(lexCurr)) {
        advance();
        continue;
      }

      tokens.add(parseToken());
    }

    return tokens;
  }

  Token parseToken() {
    switch (lexCurr) {
      case "=":
        advance();
        return Token(lexeme: "=", type: TokenType.singleEquals);
      case ".":
        advance();
        return Token(lexeme: ".", type: TokenType.dot);
      case '\'':
        return parseString();
      case '*':
        advance();
        return Token(lexeme: "*", type: TokenType.star);
      default:
        if (isLetter(lexCurr)) return parseKeyword();
        throw Exception('Unexpected character: $lexCurr');
    }
  }

  Token parseKeyword() {
    final int start = current;
    while (isLetter(lexCurr)) {
      advance();
    }

    print(lexCurr);

    final lexeme = source.substring(start, current);
    return Token(lexeme: lexeme, type: tokenMap[lexeme] ?? TokenType.identifier);
  }

  Token parseString() {
    final int start = current;
    advance();

    while (current < source.length && lexCurr != "\'") {
      if (lexCurr == '\\') {
        advance();
        if (current < source.length) {
          advance();
        }
      } else {
        advance();
      }
    }

    if (current >= source.length) {
      throw Exception('Unterminated string literal');
    }
    advance();

    final lexeme = source.substring(start, current);
    return Token(lexeme: lexeme, type: TokenType.string);
  }

  bool isLetter(String char) {
    if (char.length != 1) return false;
    int codeUnit = char.codeUnitAt(0);
    return (codeUnit >= 65 && codeUnit <= 90) || (codeUnit >= 97 && codeUnit <= 122);
  }

  bool isWhitespace(String char) {
    return char.trim().isEmpty;
  }

  void advance() {
    current++;
  }

  String peek() {
    return source[current + 1];
  }
}