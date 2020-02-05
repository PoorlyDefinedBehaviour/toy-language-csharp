using System.Collections.Generic;

namespace Parser.Statements
{
    internal class While : IStatement
    {
        public Expression Condition { get; }
        public List<IStatement> Body { get; }

        public While(Expression condition, List<IStatement> body)
        {
            Condition = condition;
            Body = body;
        }

        public dynamic Accept(IStatementVisitor visitor) => visitor.VisitWhileStatement(this);
    }
}