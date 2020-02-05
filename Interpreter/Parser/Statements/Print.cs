namespace Parser.Statements
{
  class Print : Statement
  {
    public readonly Expression expression;

    public Print(Expression expression)
    {
      this.expression = expression;
    }

    public dynamic accept(StatementVisitor visitor)
    {
      return visitor.visitPrintStatement(this);
    }
  }