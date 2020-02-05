using System.Collections.Generic;

namespace Parser.Statements
{
    internal class Block : Statement
    {
        public readonly List<Statement> _statements;

        public Block(List<Statement> statements) => _statements = statements;

        public dynamic accept(StatementVisitor visitor) => visitor.visitBlockStatement(this);
    }
}