namespace ToyLanguage.Interfaces
{
  internal interface IStatement
  {
    T Accept<T>(IStatementVisitor<T> visitor);
  }
}