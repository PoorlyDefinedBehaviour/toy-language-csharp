using ToyLanguage.Interfaces;
using ToyLanguage.Lexer;

namespace ToyLanguage.Parser.Statements
{
    internal class Return : IStatement
    {
        public Token Keyword { get; }
        public IExpression Value { get; }

        public Return(Token keyword, IExpression value)
        {
            Keyword = keyword;
            Value = value;
        }

        public T Accept<T>(IStatementVisitor<T> visitor) => visitor.VisitReturnStatement(this);
    }
}