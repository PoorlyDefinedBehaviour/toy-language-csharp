using ToyLanguage.Interfaces;
using ToyLanguage.Lexer;

namespace ToyLanguage.Parser.Expressions
{
    internal class Call : IExpression
    {
        public IExpression Callee { get; set; }
        public Token Parentheses { get; set; }
        public IExpression[] Args { get; set; }

        public T Accept<T>(IExpressionVisitor<T> visitor) => visitor.VisitCallExpression(this);
    }
}