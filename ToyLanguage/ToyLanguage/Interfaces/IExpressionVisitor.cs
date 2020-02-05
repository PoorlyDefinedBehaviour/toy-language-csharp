using ToyLanguage.Parser.Expressions;

namespace ToyLanguage.Interfaces
{
    internal interface IExpressionVisitor<T>
    {
        T VisitBinaryExpression(Binary expression);

        T VisitUnaryExpression(Unary expression);

        T VisitGroupingExpression(Grouping expression);

        T VisitLiteralExpression(Literal expression);

        T VisitVariableExpression(Variable expression);

        T VisitAssignExpression(Assign expression);

        T VisitLogicalExpression(Logical expression);

        T VisitCallExpression(Call expression);

        T VisitAccessObjectPropertyExpression(AccessObjectProperty expression);

        T VisitSetObjectPropertyExpression(SetObjectProperty expression);

        T VisitThisExpression(This expression);

        T VisitSuperExpression(Super expression);
    }
}