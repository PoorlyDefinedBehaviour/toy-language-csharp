namespace Parser.Statements
{
  interface StatementVisitor
  {
    dynamic T visitBlockStatement(statement: Block);
    dynamic visitExpressionStatement(statement: Expression);
    dynamic visitPrintStatement(statement: Print);
    dynamic visitLetStatement(statement: Let);
    dynamic visitConstStatement(statement: Const);
    dynamic visitIfStatement(statement: If);
    dynamic visitWhileStatement(statement: While);
    dynamic visitFunctionStatement(statement: Function);
    dynamic visitReturnStatement(statement: Return);
    dynamic visitClassStatement(statement: Class);
  }
}