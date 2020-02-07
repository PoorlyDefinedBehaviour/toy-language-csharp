using ToyLanguage.Interfaces;
using ToyLanguage.Lexer;

namespace ToyLanguage.Parser.Expressions
{
  internal class Variable : IExpression
  {
    public Token Name { get; }
    public TokenType Type { get; }

    public Variable(Token name, TokenType type)
    {
      Name = name;
      Type = type;
    }

    public T Accept<T>(IExpressionVisitor<T> visitor) => visitor.VisitVariableExpression(this);
  }
}