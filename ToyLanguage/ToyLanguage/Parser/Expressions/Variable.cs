using ToyLanguage.Interfaces;
using ToyLanguage.Lexer;

namespace ToyLanguage.Parser.Expressions
{
  internal class Variable : IExpression
  {
    public Token Name { get; }

    public Variable(Token name)
    {
      Name = name;
    }

    public T Accept<T>(IExpressionVisitor<T> visitor) => visitor.VisitVariableExpression(this);
  }
}