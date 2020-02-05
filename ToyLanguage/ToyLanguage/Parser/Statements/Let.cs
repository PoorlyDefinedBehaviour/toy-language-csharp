using ToyLanguage.Interfaces;
using ToyLanguage.Lexer;

namespace ToyLanguage.Parser.Statements
{
    internal class Let : IStatement
    {
        public Token Name { get; }
        public Expression Initializer { get; }

        public Let(Token name, Expression initializer)
        {
            Name = name;
            Initializer = initializer;
        }

        public T Accept<T>(IStatementVisitor visitor) => visitor.VisitLetStatement(this);
    }
}