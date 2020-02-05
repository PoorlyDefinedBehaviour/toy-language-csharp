using ToyLanguage.Interfaces;

namespace ToyLanguage.Parser.Statements
{
  internal class Expression : IStatement
  {
    public IExpression _Expression { get; }

    public Expression(IExpression expression)
    {
      _Expression = expression;
    }

    public T Accept<T>(IStatementVisitor<T> visitor) => visitor.visitExpressionStatement(this);
  }
}