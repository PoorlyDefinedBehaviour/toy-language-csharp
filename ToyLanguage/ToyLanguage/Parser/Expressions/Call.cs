using System.Collections.Generic;

using ToyLanguage.Interfaces;
using ToyLanguage.Lexer;

namespace ToyLanguage.Parser.Expressions
{
  internal class Call : IExpression
  {
    public IExpression Callee { get; }
    public Token Parentheses { get; }
    public List<IExpression> Args { get; }

    public Call(IExpression callee,
                Token parentheses,
                List<IExpression> args)
    {
      Callee = callee;
      Parentheses = parentheses;
      Args = args;
    }

    public T Accept<T>(IExpressionVisitor<T> visitor) => visitor.VisitCallExpression(this);
  }
}