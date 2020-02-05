using ToyLanguage.Interfaces;
using ToyLanguage.Lexer;

namespace ToyLanguage.Parser.Statements
{
    internal class Return : IStatement
    {
        public Token Keyword { get; }
        public Expression Value { get; }

        public Return(Token keyword, Expression value)
        {
            Keyword = keyword;
            Value = value;
        }

        public T Accept<T>(IStatementVisitor visitor) => visitor.VisitReturnStatement(this);
    }
}