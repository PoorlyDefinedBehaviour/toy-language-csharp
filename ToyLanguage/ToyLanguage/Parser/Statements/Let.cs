using ToyLanguage.Interfaces;
using ToyLanguage.Lexer;

namespace ToyLanguage.Parser.Statements
{
    internal class Let : IStatement
    {
        public Token Name { get; }
        public IExpression Initializer { get; }

        public Let(Token name, IExpression initializer)
        {
            Name = name;
            Initializer = initializer;
        }

        public T Accept<T>(IStatementVisitor<T> visitor) => visitor.VisitLetStatement(this);
    }
}