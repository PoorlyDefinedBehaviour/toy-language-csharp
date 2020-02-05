using System.Collections.Generic;

using ToyLanguage.Interfaces;
using ToyLanguage.Lexer;

namespace ToyLanguage.Parser.Statements
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

        public T Accept<T>(IStatementVisitor visitor) => visitor.VisitFunctionStatement(this);
    }
}