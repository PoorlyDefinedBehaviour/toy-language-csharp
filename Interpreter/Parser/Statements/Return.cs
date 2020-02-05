using Lexer;

namespace Parser.Statements
{
  class Return : Statement
  {
    public readonly Token keyword;
    public readonly Expression? value;

    public Return(Token keyword, Expression? value)
    {
      this.keyword = keyword;
      this.value = value;
    }

    public dynamic accept(StatementVisitor visitor)
    {
      return visitor.visitReturnStatement(this);
    }
  }
}