using System.Collections.Generic;

using ToyLanguage.Interfaces;

namespace ToyLanguage.Parser.Statements
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

        public T Accept<T>(IStatementVisitor visitor) => visitor.VisitWhileStatement(this);
    }
}