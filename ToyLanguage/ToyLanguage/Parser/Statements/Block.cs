using System.Collections.Generic;

namespace Parser.Statements
{
    internal class Block : IStatement
    {
        public readonly List<IStatement> statements;

        public Block(List<IStatement> statements) => this.statements = statements;

        public dynamic Accept(IStatementVisitor visitor) => visitor.VisitBlockStatement(this);
    }
}