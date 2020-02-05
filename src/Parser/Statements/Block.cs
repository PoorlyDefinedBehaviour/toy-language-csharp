using System.Collections.Generic;

namespace Parser.Statements
{
  class Block : Statement
  {
    public readonly List<Statement> statements;

    public Block(List<Statement> statements)
    {
      this.statements = statements;
    }

    public dynamic accept(StatementVisitor visitor)
    {
      return visitor.visitBlockStatement(this);
    }
  }
}