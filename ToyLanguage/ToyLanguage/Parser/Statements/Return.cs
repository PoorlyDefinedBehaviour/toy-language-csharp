using Lexer;

namespace Parser.Statements
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

        public dynamic Accept(IStatementVisitor visitor) => visitor.VisitReturnStatement(this);
    }
}