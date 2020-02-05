namespace Parser.Statements
{
    internal class Expression : IStatement
    {
        public Expression CurrentExpression { get; }

        public Expression(Expression expression) => CurrentExpression = expression;

        public dynamic Accept(IStatementVisitor visitor) => visitor.VisitExpressionStatement(this);
    }
}