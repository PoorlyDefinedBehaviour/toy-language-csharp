namespace ToyLanguage.Interfaces
{
    internal interface IExpression
    {
        T Accept<T>(IExpressionVisitor visitor);
    }
}