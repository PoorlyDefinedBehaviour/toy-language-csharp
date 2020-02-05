using ToyLanguage.Interfaces;
using ToyLanguage.Lexer;

namespace ToyLanguage.Parser.Statements
{
    internal class Const : IStatement
    {
        public Token Name { get; }
        public IExpression Initializer { get; }

        public Const(Token name, IExpression initializer)
        {
            Name = name;
            Initializer = initializer;
        }

        public T Accept<T>(IStatementVisitor<T> visitor) => visitor.VisitConstStatement(this);
    }
}