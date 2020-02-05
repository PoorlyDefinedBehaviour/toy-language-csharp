using System.Collections.Generic;

using ToyLanguage.Interfaces;

namespace ToyLanguage.Parser.Statements
{
  internal class Block : IStatement
  {
    public List<IStatement> Statements { get; }

    public Block(List<IStatement> statements)
    {
      Statements = statements;
    }

    public T Accept<T>(IStatementVisitor<T> visitor) => visitor.VisitBlockStatement(this);
  }
}