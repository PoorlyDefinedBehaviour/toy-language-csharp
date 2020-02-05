using ToyLanguage.Interfaces;
using ToyLanguage.Lexer;

namespace ToyLanguage.Parser.Expressions
{
    internal class AccessObjectProperty : IExpression
    {
        public IExpression Object { get; }
        public Token Token { get; }
        public bool IsSuperClassProperty { get; }

        public AccessObjectProperty(IExpression @object, Token token, bool isSuperClassProperty = false)
        {
            Object = @object;
            Token = token;
            IsSuperClassProperty = isSuperClassProperty;
        }

        public T Accept<T>(IExpressionVisitor<T> visitor) => visitor.VisitAccessObjectPropertyExpression(this);
    }
}