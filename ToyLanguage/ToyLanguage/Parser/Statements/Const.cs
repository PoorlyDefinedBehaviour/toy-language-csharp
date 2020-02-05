using Lexer;

namespace Parser.Statements
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

        public dynamic Accept(IStatementVisitor visitor) => visitor.VisitConstStatement(this);
    }
}