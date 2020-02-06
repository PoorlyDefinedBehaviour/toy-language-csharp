using ToyLanguage.Interfaces;

namespace ToyLanguage.Parser.Expressions
{
    internal class Literal : IExpression
    {
        public object Value { get; }

        public Literal(object value) => Value = value;

        public T Accept<T>(IExpressionVisitor<T> visitor) => visitor.VisitLiteralExpression(this);
    }
}