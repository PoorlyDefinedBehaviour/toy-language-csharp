using ToyLanguage.Interfaces;

namespace ToyLanguage.Parser.Expressions
{
  internal class Grouping : IExpression
  {
    public IExpression Expression { get; }

    public Grouping(IExpression expression)
    {
      Expression = expression;
    }

    public T Accept<T>(IExpressionVisitor<T> visitor) => visitor.VisitGroupingExpression(this);
  }
}