using ToyLanguage.Interfaces;
using ToyLanguage.Lexer;

namespace ToyLanguage.Parser.Expressions
{
  internal class This : IExpression
  {
    public Token Name { get; }

    public This(Token name)
    {
      Name = name;
    }

    public T Accept<T>(IExpressionVisitor<T> visitor) => visitor.VisitThisExpression(this);
  }
}