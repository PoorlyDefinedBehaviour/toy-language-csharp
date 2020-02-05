using System;

using ToyLanguage.Interfaces;
using ToyLanguage.Lexer;

namespace ToyLanguage.Parser.Expressions
{
  internal class Logical : IExpression
  {
    public IExpression Left { get; }
    public Token Operator { get; }
    public IExpression Right { get; }

    public Logical(IExpression left,
                   Token op,
                   IExpression right)
    {
      Left = left;
      Operator = op;
      Right = right;
    }

    public T Accept<T>(IExpressionVisitor<T> visitor) => throw new NotImplementedException();
  }
}