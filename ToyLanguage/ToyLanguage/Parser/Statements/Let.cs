using Lexer;

namespace Parser.Statements
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

        public dynamic Accept(IStatementVisitor visitor) => visitor.VisitLetStatement(this);
    }
}