using ToyLanguage.Interfaces;
using ToyLanguage.Lexer;

namespace ToyLanguage.Parser.Statements
{
    internal class Const : IStatement
    {
        public Token Name { get; }
        public Expression Initializer { get; }

        public Const(Token name, Expression initializer)
        {
            Name = name;
            Initializer = initializer;
        }

        public T Accept<T>(IStatementVisitor visitor) => visitor.VisitConstStatement(this);
    }
}