using Lexer;

using System.Collections.Generic;

namespace Parser.Statements
{
    internal class Function : IStatement
    {
        public Token Name { get; }
        public List<Token> Parameters { get; }
        public List<IStatement> Body { get; }

        public Function(Token name, List<Token> parameters, List<IStatement> body)
        {
            Name = name;
            Parameters = parameters;
            Body = body;
        }

        public dynamic Accept(IStatementVisitor visitor) => visitor.VisitFunctionStatement(this);
    }
}