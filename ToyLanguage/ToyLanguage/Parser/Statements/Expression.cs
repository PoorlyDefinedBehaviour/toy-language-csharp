using ToyLanguage.Interfaces;

namespace ToyLanguage.Parser.Statements
{
    internal class Expression : IStatement
    {
        public T Accept<T>(IStatementVisitor visitor) => visitor.VisitExpressionStatement(this);
    }
}