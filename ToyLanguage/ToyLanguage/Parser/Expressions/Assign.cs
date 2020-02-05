using ToyLanguage.Interfaces;
using ToyLanguage.Lexer;

namespace ToyLanguage.Parser.Expressions
{
  internal class Assign : IExpression
  {
    public Token Name { get; }
    public IExpression Value { get; }

    public Assign(Token name, IExpression value)
    {
      Name = name;
      Value = value;
    }

    public T Accept<T>(IExpressionVisitor<T> visitor) => visitor.VisitAssignExpression(this);
  }
}