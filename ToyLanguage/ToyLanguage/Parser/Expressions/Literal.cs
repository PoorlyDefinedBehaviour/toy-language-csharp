using ToyLanguage.Interfaces;

namespace ToyLanguage.Parser.Expressions
{
    internal class Literal : IExpression
    {
        //add Value
        public Literal()
        {
        }

        public T Accept<T>(IExpressionVisitor<T> visitor) => visitor.VisitLiteralExpression(this);
    }
}