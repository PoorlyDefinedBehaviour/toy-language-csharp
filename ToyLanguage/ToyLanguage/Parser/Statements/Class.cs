using System.Collections.Generic;

using ToyLanguage.Interfaces;
using ToyLanguage.Lexer;
using ToyLanguage.Parser.Expressions;

namespace ToyLanguage.Parser.Statements
{
    internal class Class : IStatement
    {
        public Token Name { get; }
        public Variable Superclass { get; }
        public List<Function> Methods { get; }
        public List<Function> StaticMethods { get; }

        public Class(Token name, Variable superclass, List<Function> methods, List<Function> staticMethods)
        {
            Name = name;
            Superclass = superclass;
            Methods = methods;
            StaticMethods = staticMethods;
        }

        public T Accept<T>(IStatementVisitor visitor) => visitor.VisitClassStatement(this);
    }
}