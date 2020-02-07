namespace ToyLanguage.Interfaces
{
  internal interface IExpression
  {
    T Accept<T>(IExpressionVisitor<T> visitor);
  }
}