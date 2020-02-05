using System;

using ToyLanguage.Interfaces;

namespace ToyLanguage.Parser.Expressions
{
    internal class Literal : IExpression
    {
        [Obsolete]
        public object Value { get; }

        [Obsolete]
        public Literal(object value) => Value = value;

        public T Accept<T>(IExpressionVisitor<T> visitor) => visitor.VisitLiteralExpression(this);
    }
}