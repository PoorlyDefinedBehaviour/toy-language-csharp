using System;

using ToyLanguage.Interfaces;
using ToyLanguage.Lexer;

namespace ToyLanguage.Parser.Expressions
{
  internal class Super : IExpression
  {
    public Token Name { get; }

    public Super(Token name)
    {
      Name = name;
    }

    public T Accept<T>(IExpressionVisitor<T> visitor) => throw new NotImplementedException();
  }
}