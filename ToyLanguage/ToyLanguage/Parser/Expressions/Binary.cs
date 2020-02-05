using ToyLanguage.Interfaces;
using ToyLanguage.Lexer;

namespace ToyLanguage.Parser.Expressions
{
    internal class Binary : IExpression
    {
        public IExpression Left { get; }
        public Token Operation { get; }
        public IExpression Right { get; }

        public Binary(IExpression left, Token operation, IExpression right)
        {
            Left = left;
            Operation = operation;
            Right = right;
        }

        public T Accept<T>(IExpressionVisitor<T> visitor) => visitor.VisitBinaryExpression(this);
    }
}