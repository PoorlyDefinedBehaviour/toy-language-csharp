namespace Parser.Statements
{
    internal interface IStatementVisitor
    {
        dynamic VisitBlockStatement(Block statement);

        dynamic VisitExpressionStatement(Expression statement);

        dynamic VisitPrintStatement(Print statement);

        dynamic VisitLetStatement(Let statement);

        dynamic VisitConstStatement(Const statement);

        dynamic VisitIfStatement(If statement);

        dynamic VisitWhileStatement(While statement);

        dynamic VisitFunctionStatement(Function statement);

        dynamic VisitReturnStatement(Return statement);

        dynamic VisitClassStatement(Class statement);
    }
}