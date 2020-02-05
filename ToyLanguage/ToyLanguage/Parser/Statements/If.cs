namespace Parser.Statements
{
    internal class If : IStatement
    {
        public Expression Condition { get; }
        public IStatement ThenBranch { get; }
        public IStatement? ElseBranch { get; }

        public If(Expression condition, IStatement thenBranch, IStatement elseBranch)
        {
            Condition = condition;
            ThenBranch = thenBranch;
            ElseBranch = elseBranch;
        }

        public dynamic Accept(IStatementVisitor visitor) => visitor.VisitIfStatement(this);
    }
}