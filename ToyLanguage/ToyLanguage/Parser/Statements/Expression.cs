using ToyLanguage.Interfaces;

namespace ToyLanguage.Parser.Statements
{
    internal class Expression : IStatement
    {
        public IExpression CurrentExpression { get; }

        public Expression(IExpression expression) => CurrentExpression = expression;

        public T Accept<T>(IStatementVisitor<T> visitor) => visitor.VisitExpressionStatement(this);
    }
}