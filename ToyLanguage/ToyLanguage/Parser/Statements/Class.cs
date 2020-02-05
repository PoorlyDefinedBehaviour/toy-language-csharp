using Lexer;

using System.Collections.Generic;

namespace Parser.Statements
{
    internal class Class : IStatement
    {
        public Token name { get; }
        public Variable? superclass { get; }
        public List<Function> methods { get; }
        public List<Function> staticMethods { get; }

        public Class(
           Token name,
           Variable? superclass,
           List<Function> methods,
           List<Function> staticMethods
        )
        {
            this.name = name;
            this.superclass = superclass;
            this.methods = methods;
            this.staticMethods = staticMethods;
        }

        public dynamic Accept(IStatementVisitor visitor) => visitor.VisitClassStatement(this);
    }