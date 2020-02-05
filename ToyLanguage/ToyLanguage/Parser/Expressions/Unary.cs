using ToyLanguage.Interfaces;
using ToyLanguage.Lexer;

namespace ToyLanguage.Parser.Expressions
{
    internal class Unary : IExpression
    {
        public Token Operation { get; }
        public IExpression Right { get; }

        public Unary(Token operation, IExpression right)
        {
            Operation = operation;
            Right = right;
        }

        public T Accept<T>(IExpressionVisitor<T> visitor) => visitor.VisitUnaryExpression(this);
    }
}