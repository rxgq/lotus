abstract final class Expression {

}

final class SelectExpr extends Expression {
  String select;

  SelectExpr({
    required this.select
  });

  @override
  String toString() {
    return "select $select";
  }
}

final class FromExpr extends Expression {
  String from;

  FromExpr({
    required this.from
  });

  @override
  String toString() {
    return "from $from";
  }
}

final class WhereExpr extends Expression {

}

final class LogicalExpr extends Expression {

}

// ((select *) from cards) (where ((card.project = "here") and (card.user == "user 1") or (card.user == "user 2")))