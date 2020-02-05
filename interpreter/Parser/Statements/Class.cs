using System.Collections.Generic;
using Lexer;

namespace Parser.Statements
{
  class Class : Statement
  {
    public readonly Token name;
    public readonly Variable? superclass;
    public readonly List<Function> methods;
    public readonly List<Function> staticMethods;

    public Class(Token name,
                 Variable? superclass,
                 List<Function> methods,
                 List<Function> staticMethods)
    {
      this.name = name;
      this.superclass = superclass;
      this.methods = methods;
      this.staticMethods = staticMethods;
    }

    public dynamic accept(StatementVisitor visitor)
    {
      return visitor.visitClassStatement(this);
    }
  }