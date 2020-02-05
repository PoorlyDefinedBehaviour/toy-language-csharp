using System.Collections.Generic;

using ToyLanguage.Interfaces;

namespace ToyLanguage.Parser.Statements
{
    internal class While : IStatement
    {
        public IExpression Condition { get; }
        public List<IStatement> Body { get; }

        public While(IExpression condition, List<IStatement> body)
        {
            Condition = condition;
            Body = body;
        }

        public T Accept<T>(IStatementVisitor<T> visitor) => visitor.VisitWhileStatement(this);
    }
}