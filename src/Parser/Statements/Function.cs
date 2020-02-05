using System.Collections.Generic;
using Lexer;

namespace Parser.Statements
{
  class Function : Statement
  {
    public readonly Token name;
    public readonly List<Token> parameters;
    public readonly List<Statement> body;

    public Function(Token name,
                    List<Token> parameters,
                    List<Statement> body)
    {
      this.name = name;
      this.parameters = parameters;
      this.body = body;
    }

    public dynamic accept(StatementVisitor visitor)
    {
      return visitor.visitFunctionStatement(this);
    }
  }
}