using System.Collections.Generic;

namespace Parser.Statements
{
  class While : Statement
  {
    public readonly Expression condition;
    public readonly List<Statement> body;

    public While(Expression condition, List<Statement> body)
    {
      this.condition = condition;
      this.body = body;
    }

    public dynamic accept(StatementVisitor visitor)
    {
      return visitor.visitWhileStatement(this);
    }
  }
}