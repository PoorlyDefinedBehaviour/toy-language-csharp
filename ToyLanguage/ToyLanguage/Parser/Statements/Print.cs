using ToyLanguage.Interfaces;

namespace ToyLanguage.Parser.Statements
{
    internal class Print : IStatement
    {
        public Expression Expression { get; }

        public Print(Expression expression) => Expression = expression;

        public T Accept<T>(IStatementVisitor visitor) => visitor.VisitPrintStatement(this);
    }
}