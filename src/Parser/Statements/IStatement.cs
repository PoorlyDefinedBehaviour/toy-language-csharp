namespace Parser.Statements
{
  interface Statement
  {
    dynamic accept(StatementVisitor visitor);
  }
}