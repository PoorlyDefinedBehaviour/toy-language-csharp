using ToyLanguage.Interfaces;

namespace ToyLanguage.Parser.Statements
{
    internal class Print : IStatement
    {
        public IExpression Expression { get; }

        public Print(IExpression expression) => Expression = expression;

        public T Accept<T>(IStatementVisitor<T> visitor) => visitor.VisitPrintStatement(this);
    }
}