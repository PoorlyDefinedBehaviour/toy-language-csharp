using System;
using System.Collections.Generic;

using ToyLanguage.Interfaces;
using ToyLanguage.Lexer;
using ToyLanguage.Parser.Exceptions;
using ToyLanguage.Parser.Expressions;
using ToyLanguage.Parser.Statements;

namespace ToyLanguage.Parser
{
  internal struct ParserResult
  {
    public bool Ok { get; set; }
    public List<IStatement> Statements { get; set; }
  }

  internal class Parser
  {
    private int Current = 0;
    private bool ok = true;
    private readonly List<Token> Tokens;

    public Parser(List<Token> Tokens) => this.Tokens = Tokens;

    private Token Previous() => Tokens[Current - 1];

    private Token Peek(int offset = 0) => Tokens[Current + offset];

    private bool EndOfTokens() => Peek().Type == TokenType.END_OF_FILE;

    private Token Advance()
    {
      if (!EndOfTokens())
        ++Current;

      return Previous();
    }

    private bool Check(TokenType type) => EndOfTokens() ? false : Peek().Type == type;

    private bool Match(params TokenType[] types)
    {
      foreach (TokenType type in types)
      {
        if (Check(type))
        {
          Advance();
          return true;
        }
      }

      return false;
    }

    private void Error(Token token, string message)
    {
      if (token.Type == TokenType.END_OF_FILE)
        Console.WriteLine($"<{token.Line}> ${message}");
      else
        Console.WriteLine($"<{token.Lexeme.ToString()}><{token.Line}> {message}");

      ok = false;
    }

    private void Synchronize()
    {
      Advance();

      while (!EndOfTokens())
      {
        switch (Peek().Type)
        {
          case TokenType.CLASS:
          case TokenType.FUNCTION:
          case TokenType.LET:
          case TokenType.FOR:
          case TokenType.IF:
          case TokenType.WHILE:
          case TokenType.PRINT:
          case TokenType.RETURN:
            return;
        }

        Advance();
      }
    }

    private Token Consume(TokenType type, string message)
    {
      if (Check(type))
        return Advance();

      Error(Peek(), $"{message}, got {Peek().Lexeme}");
      throw new ParserException($"{message}, got {Peek().Lexeme}");
    }

    private IExpression Primary()
    {
      if (Match(TokenType.FALSE))
        return new Literal(false);

      if (Match(TokenType.TRUE))
        return new Literal(true);

      if (Match(TokenType.NULL))
        return new Literal(null);

      if (Match(TokenType.NUMBER, TokenType.STRING))
        return new Literal(Previous().Literal);

      if (Match(TokenType.THIS))
        return new Variable(Previous(), TokenType.THIS);

      if (Match(TokenType.SUPER))
        return new Variable(Previous(), TokenType.SUPER);

      if (Match(TokenType.IDENTIFIER))
        return new Variable(Previous(), TokenType.IDENTIFIER);

      if (Match(TokenType.LEFT_PAREN))
      {
        Consume(TokenType.RIGHT_PAREN, "Expected ')' after expression");
        return new Grouping(Expression());
      }

      Error(Peek(), "Expression expected");
      throw new ParserException($"Expression expected at line {Previous().Line}");
    }

    private IExpression FinishCall(IExpression callee)
    {
      var args = new List<IExpression>();

      if (!Check(TokenType.RIGHT_PAREN))
        while (true)
        {
          args.Add(Expression());

          if (args.Count > 254)
            Error(Peek(), "A function can't take more than 254 arguments");

          if (!Match(TokenType.COMMA))
            break;
        }

      var parentheses = Consume(TokenType.RIGHT_PAREN, "Expected ')' after function arguments");
      Console.WriteLine(parentheses.Lexeme);

      return new Call(callee, parentheses, args);
    }

    private IExpression Call()
    {
      IExpression expr = Primary();

      while (true)
      {
        if (Match(TokenType.LEFT_PAREN))
          expr = FinishCall(expr);
        else if (Match(TokenType.DOT))
          expr = new AccessObjectProperty(expr,
              Consume(TokenType.IDENTIFIER, "Expect property name after object access op"),
                  Tokens[Current - 3].Type == TokenType.SUPER);
        else
          break;
      }

      return expr;
    }

    private IExpression Unary() =>
        Match(TokenType.BANG, TokenType.DOUBLE_BANG) ? new Unary(Previous(), Unary()) : Call();

    private IExpression NullCoalescing()
    {
      IExpression expr = Unary();

      while (Match(TokenType.NULL_COALESCING))
        expr = new Binary(expr, Previous(), new Literal(Expression()));

      return expr;
    }

    private IExpression Multiplication()
    {
      IExpression expr = NullCoalescing();

      while (Match(TokenType.SLASH, TokenType.STAR))
        expr = new Binary(expr, Previous(), Unary());

      return expr;
    }

    private IExpression Mod()
    {
      IExpression expr = Multiplication();

      while (Match(TokenType.MOD))
        expr = new Binary(expr, Previous(), Multiplication());

      return expr;
    }

    private IExpression Addition()
    {
      IExpression expr = Mod();

      while (Match(TokenType.MINUS, TokenType.PLUS))
        expr = new Binary(expr, Previous(), Mod());

      return expr;
    }

    private IExpression Comparison()
    {
      IExpression expr = Addition();

      while (Match(
        TokenType.GREATER,
        TokenType.GREATER_EQUAL,
        TokenType.LESS,
        TokenType.LESS_EQUAL
      ))
        expr = new Binary(expr, Previous(), Addition());

      return expr;
    }

    private IExpression Equality()
    {
      IExpression expr = Comparison();

      while (Match(TokenType.BANG_EQUAL, TokenType.EQUAL_EQUAL))
        expr = new Binary(expr, Previous(), Comparison());

      return expr;
    }

    private IExpression And()
    {
      IExpression expr = Equality();

      while (Match(TokenType.AND))
        expr = new Logical(expr, Previous(), Equality());

      return expr;
    }

    private IExpression Or()
    {
      IExpression expr = And();

      while (Match(TokenType.OR))
        expr = new Logical(expr, Previous(), And());

      return expr;
    }

    private IExpression Assignment()
    {
      IExpression expr = Or();

      if (Match(TokenType.EQUAL))
      {
        IExpression value = Assignment();

        if (expr is Variable variable)
          return new Assign(variable.Name, value);

        if (expr is AccessObjectProperty accessExpr)
          return new SetObjectProperty(accessExpr.Object,
                                       accessExpr.Token,
                                       value);

        Error(Previous(), "Invalid assignment target");
      }

      return expr;
    }

    private IExpression Expression() => Assignment();

    private IStatement ExpressionStatement() => new Expression(Expression());

    private IStatement IfStatement()
    {
      Consume(TokenType.LEFT_PAREN, "Expected '(' after if keyword");

      var condition = Expression();

      Consume(TokenType.RIGHT_PAREN, "Expected ')' after if condition");

      var thenBranch = Statement();
      var elseBranch = Match(TokenType.ELSE)
         ? Statement()
         : null;

      return new If(condition, thenBranch, elseBranch);
    }

    private IStatement ForStatement()
    {
      Consume(TokenType.LEFT_PAREN, "Expected '(' after for keyword");

      IStatement initializer = Match(TokenType.LET)
        ? LetDeclaration() as IStatement
        : null;

      //Consume(TokenType.SEMI_COLON, "Expected ';' after for loop initializer");

      IExpression condition = Match(TokenType.SEMI_COLON)
        ? Expression()
        : new Literal(true);

      Consume(TokenType.SEMI_COLON, "Expected ';' after for loop condition");

      IExpression increment = this.Match(TokenType.RIGHT_PAREN)
        ? null
        : Expression();

      Consume(TokenType.RIGHT_PAREN, "Expected ')' after for loop increment");

      IStatement body = Statement();

      body = increment != null
        ? new Block(new List<IStatement> { body, new Expression(increment) })
        : null;

      if (condition == null)
        condition = new Literal(true);

      body = new While(condition, body);

      if (initializer != null)
        body = new Block(new List<IStatement> { initializer, body });

      return body;
    }

    private List<IStatement> BlockStatement()
    {
      var statements = new List<IStatement>();

      while (!Check(TokenType.RIGHT_BRACE) && !EndOfTokens())
      {
        IStatement declaration = Declaration();
        if (declaration != null)
          statements.Add(declaration);
      }

      Consume(TokenType.RIGHT_BRACE, "Expected '}' after block.");
      return statements;
    }

    private IStatement ConstDeclaration()
    {
      Token name = Consume(TokenType.IDENTIFIER, "Expected variable name");

      Consume(TokenType.EQUAL, $"Expect initializer for const variable '${name.Lexeme}");

      return new Const(name, Expression());
    }

    private IStatement LetDeclaration() =>
      new Let(Consume(TokenType.IDENTIFIER, "Expected variable name"), Match(TokenType.EQUAL) ? Expression() : null);

    private Function FunctionDeclaration(string kind)
    {
      var name = Consume(TokenType.IDENTIFIER, $"Expected {kind} name");
      Consume(TokenType.LEFT_PAREN, $"Expected '(' after {kind} name");

      var parameters = new List<Token>();
      if (!Match(TokenType.RIGHT_PAREN))
      {
        while (true)
        {
          if (parameters.Count > 254)
          {
            Error(
              Peek(),
              $"{kind} can't have more than 254 parameters"
            );
          }

          parameters.Add(Consume(TokenType.IDENTIFIER, $"Expect {kind} parameter name"));

          if (!Match(TokenType.COMMA))
            break;
        }
      }

      Consume(TokenType.RIGHT_PAREN, $"Expected ')' after {kind} parameters");
      Consume(TokenType.LEFT_BRACE, $"Expected left brace before {kind} body");

      return new Function(name, parameters, BlockStatement());
    }

    private IStatement WhileStatement()
    {
      Consume(TokenType.LEFT_PAREN, "Expected '(' after while keyword");

      var condition = Expression();

      Consume(TokenType.RIGHT_PAREN, "Expected ')' after while condition");

      var body = Statement();

      return new While(condition, body);
    }

    private IStatement ReturnStatement() =>
        new Return(Previous(), Match(TokenType.RIGHT_BRACE) ? null : Expression());

    private IStatement PrintStatement() => new Print(Expression());

    private IStatement Statement()
    {
      if (Match(TokenType.PRINT))
        return PrintStatement();

      if (Match(TokenType.RETURN))
        return ReturnStatement();

      if (Match(TokenType.WHILE))
        return WhileStatement();

      if (Match(TokenType.LEFT_BRACE))
        return new Block(BlockStatement());

      if (Match(TokenType.FOR))
        return ForStatement();

      if (Match(TokenType.IF))
        return IfStatement();

      return ExpressionStatement();
    }

    private IStatement ClassDeclaration()
    {
      Variable superClass = null;
      if (Match(TokenType.CLASS_EXTENDS))
      {
        Consume(TokenType.IDENTIFIER, "Expected superclass name as extends keyword");
        superClass = new Variable(Previous(), TokenType.IDENTIFIER);
      }

      Consume(TokenType.LEFT_BRACE, "Expected '{' after class declaration");

      var classMethods = new List<Function>();
      var staticMethods = new List<Function>();

      while (!Check(TokenType.RIGHT_BRACE) && !EndOfTokens())
      {
        if (Check(TokenType.STATIC))
        {
          Advance();
          staticMethods.Add(FunctionDeclaration("static"));
        }
        else
          classMethods.Add(FunctionDeclaration("method"));
      }

      Consume(TokenType.RIGHT_BRACE, "Expected } after class body");
      return new Class(Consume(TokenType.IDENTIFIER, "Expected identifier after class keyword"), superClass, classMethods, staticMethods);
    }

    private IStatement Declaration()
    {
      try
      {
        if (Match(TokenType.CLASS))
          return ClassDeclaration();

        if (Match(TokenType.FUNCTION))
          return FunctionDeclaration("function");

        if (Match(TokenType.LET))
          return LetDeclaration();

        if (Match(TokenType.CONST))
          return ConstDeclaration();

        return Statement();
      }
      catch (Exception)
      {
        Synchronize();
        return null;
      }
    }

    public ParserResult Run()
    {
      var statements = new List<IStatement>();

      while (!EndOfTokens())
      {
        IStatement declaration = Declaration();

        if (declaration != null)
        {
          statements.Add(declaration);
        }
      }

      return new ParserResult { Ok = ok, Statements = statements };
    }
  }
}