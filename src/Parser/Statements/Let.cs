using Lexer;

namespace Parser.Statements
{
  class Let : Statement
  {
    public readonly Token name;
    public readonly Expression? initializer;

    public Let(Token name, Expression? initializer)
    {
      this.name = name;
      this.initializer = initializer;
    }

    public dynamic accept(StatementVisitor visitor)
    {
      return visitor.visitLetStatement(this);
    }
  }