namespace Parser.Statements
{
  interface StatementVisitor
  {
    dynamic visitBlockStatement(Block statement);
    dynamic visitExpressionStatement(Expression statement);
    dynamic visitPrintStatement(Print statement);
    dynamic visitLetStatement(Let statement);
    dynamic visitConstStatement(Const statement);
    dynamic visitIfStatement(If statement);
    dynamic visitWhileStatement(While statement);
    dynamic visitFunctionStatement(Function statement);
    dynamic visitReturnStatement(Return statement);
    dynamic visitClassStatement(Class statement);
  }
}