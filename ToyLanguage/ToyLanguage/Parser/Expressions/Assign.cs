using ToyLanguage.Interfaces;
using ToyLanguage.Lexer;

namespace ToyLanguage.Parser.Expressions
{
    internal class Assign : IExpression
    {
        public Token Name { get; set; }
        public IExpression Value { get; set; }

        public T Accept<T>(IExpressionVisitor<T> visitor) => visitor.VisitAssignExpression(this);
    }
}