using ToyLanguage.Parser.Expressions;

namespace ToyLanguage.Interfaces
{
    internal interface IExpressionVisitor
    {
        dynamic VisitBinaryExpression(Binary expression);

        dynamic VisitUnaryExpression(Unary expression);

        dynamic VisitGroupingExpression(Grouping expression);

        dynamic VisitLiteralExpression(Literal expression);

        dynamic VisitVariableExpression(Variable expression);

        dynamic VisitAssignExpression(Assign expression);

        dynamic VisitLogicalExpression(Logical expression);

        dynamic VisitCallExpression(Call expression);

        dynamic VisitAccessObjectPropertyExpression(AccessObjectProperty expression);

        dynamic VisitSetObjectPropertyExpression(SetObjectProperty expression);

        dynamic VisitThisExpression(This expression);

        dynamic VisitSuperExpression(Super expression);
    }
}