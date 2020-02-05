using ToyLanguage.Parser.Statements;

namespace ToyLanguage.Interfaces
{
  internal interface IStatementVisitor<T>
  {
    T VisitBlockStatement(Block statement);

    T VisitExpressionStatement(IExpression statement);

    T VisitPrintStatement(Print statement);

    T VisitLetStatement(Let statement);

    T VisitConstStatement(Const statement);

    T VisitIfStatement(If statement);

    T VisitWhileStatement(While statement);

    T VisitFunctionStatement(Function statement);

    T VisitReturnStatement(Return statement);

    T VisitClassStatement(Class statement);

    T visitExpressionStatement(Expression statement);
  }
}