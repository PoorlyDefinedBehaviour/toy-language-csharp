using ToyLanguage.Interfaces;

namespace ToyLanguage.Parser.Statements
{
    internal class If : IStatement
    {
        public IExpression Condition { get; }
        public IStatement ThenBranch { get; }
        public IStatement ElseBranch { get; }

        public If(IExpression condition, IStatement thenBranch, IStatement elseBranch)
        {
            Condition = condition;
            ThenBranch = thenBranch;
            ElseBranch = elseBranch;
        }

        public T Accept<T>(IStatementVisitor<T> visitor) => visitor.VisitIfStatement(this);
    }
}