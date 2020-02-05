namespace Parser.Statements
{
  class Expression : Statement
  {
    public readonly Expression expression;

    public Expression(Expression expression)
    {
      this.expression = expression;
    }

    public dynamic accept(StatementVisitor visitor)
    {
      return visitor.visitExpressionStatement(this);
    }
  }