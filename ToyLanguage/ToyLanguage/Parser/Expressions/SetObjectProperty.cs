using ToyLanguage.Interfaces;
using ToyLanguage.Lexer;

namespace ToyLanguage.Parser.Expressions
{
    internal class SetObjectProperty : IExpression
    {
        public IExpression Object { get; }
        public Token Token { get; }
        public IExpression Value { get; }

        public SetObjectProperty(IExpression @object, Token token, IExpression value)
        {
            Object = @object;
            Token = token;
            Value = value;
        }

        public T Accept<T>(IExpressionVisitor<T> visitor) => visitor.VisitSetObjectPropertyExpression(this);
    }
}