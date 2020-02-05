namespace Parser.Statements
{
    internal class Print : IStatement
    {
        public Expression Expression { get; }

        public Print(Expression expression) => Expression = expression;

        public dynamic Accept(IStatementVisitor visitor) => visitor.VisitPrintStatement(this);
    }
}