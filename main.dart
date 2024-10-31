import 'src/lexer.dart';
import 'dart:io';

void main() async {
  final source = await File("source.txt").readAsString();

  final lexer = Lexer(source: source);
  var tokens = lexer.tokenize();

  for (var token in tokens) print(token.toString());
}