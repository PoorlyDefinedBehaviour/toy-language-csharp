using ToyLanguage.Interfaces;
using ToyLanguage.Parser.Expressions;

namespace ToyLanguage.Utils
{
    internal class AstPrinter : IExpressionVisitor<string>
    {
        /*
          private parenthesize = (name: string, ...expressions: any[]) =>
          {
              (${name} ${expressions.reduce((result, expr) =>
              {
                  ${result} ${expr.accept(this)}
              },"")})
          }.replace(/  /g, " ");
        */

        public string Print(IExpression expression) => expression.Accept(this);

        public string VisitBinaryExpression(Binary expression)
        {
            return "";
        }

        public string VisitUnaryExpression(Unary expression) => throw new System.NotImplementedException();

        public string VisitGroupingExpression(Grouping expression) => throw new System.NotImplementedException();

        public string VisitLiteralExpression(Literal expression) =>
            expression.Value == null ? "nil" : expression.Value.ToString();

        #region trash

        public string VisitLogicalExpression(Logical expression) => throw new System.NotImplementedException();

        public string VisitAccessObjectPropertyExpression(AccessObjectProperty expression) => throw new System.NotImplementedException();

        public string VisitAssignExpression(Assign expression) => throw new System.NotImplementedException();

        public string VisitCallExpression(Call expression) => throw new System.NotImplementedException();

        public string VisitSetObjectPropertyExpression(SetObjectProperty expression) => throw new System.NotImplementedException();

        public string VisitSuperExpression(Super expression) => throw new System.NotImplementedException();

        public string VisitThisExpression(This expression) => throw new System.NotImplementedException();

        public string VisitVariableExpression(Variable expression) => throw new System.NotImplementedException();

        #endregion trash
    }
}