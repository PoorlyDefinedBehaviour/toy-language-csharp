using Lexer;

namespace Parser.Statements
{
  class Const : Statement
  {
    public readonly Token name;
    public readonly Expression initializer;

    public Const(Token name, Expression initializer)
    {
      this.name = name;
      this.initializer = initializer;
    }

    public dynamic accept(StatementVisitor visitor)
    {
      return visitor.visitConstStatement(this);
    }
  }