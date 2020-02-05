namespace Parser.Statements
{
    internal interface IStatement
    {
        dynamic Accept(IStatementVisitor visitor);
    }
}