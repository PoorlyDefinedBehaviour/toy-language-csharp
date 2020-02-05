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

    public void accept(StatementVisitor visitor)
    {
      visitor.visitBlockStatement(this);
    }
  }