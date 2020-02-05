namespace Parser.Statements
{
  class If : Statement
  {
    public readonly Expression condition;
    public readonly Statement thenBranch;
    public readonly Statement? elseBranch;

    public If(Expression condition,
              Statement thenBranch,
              Statement? elseBranch)
    {
      this.condition = condition;
      this.thenBranch = thenBranch;
      this.elseBranch = elseBranch;
    }

    public dynamic accept(StatementVisitor visitor)
    {
      return visitor.visitIfStatement(this);
    }
  }
}