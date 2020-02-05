using System;
using System.Collections.Generic;

using ToyLanguage.Lexer;
using ToyLanguage.Interfaces;
using ToyLanguage.Parser.Exceptions;
using ToyLanguage.Parser.Statements;
using ToyLanguage.Parser.Expressions;

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
    private bool Ok = true;
    private List<Token> Tokens;

    public Parser(List<Token> tokens)
    {
      Tokens = tokens;
    }

    private Token Previous() => Tokens[Current - 1];

    private Token Peek(int offset = 0) => Tokens[Current + offset];

    private bool EndOfTokens() => Peek().Type == TokenType.END_OF_FILE;

    private Token Advance() => Tokens[++Current];

    private bool Check(TokenType type) => EndOfTokens() ? false : Peek().Type == type;

    private bool Match(params TokenType[] types)
    {
      foreach (var type in types)
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
      {
        Console.WriteLine($"<{token.Line}> ${message}");
      }
      else
      {
        Console.WriteLine($"<{token.Lexeme.ToString()}><{token.Line}> {message}");
      }

      Ok = false;
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
      {
        return Advance();
      }

      Error(Peek(), message);
      throw new ParserException($"Expected <{Peek().Lexeme.ToString()} after {Previous().Lexeme.ToString()}");
    }

    private IExpression Primary()
    {
      if (Match(TokenType.FALSE))
      {
        return new Literal(false);
      }
      if (Match(TokenType.TRUE))
      {
        return new Literal(true);
      }
      if (Match(TokenType.NULL))
      {
        return new Literal(null);
      }
      if (Match(TokenType.NULL, TokenType.STRING))
      {
        return new Literal(Previous().Literal);
      }
      if (Match(TokenType.THIS))
      {
        return new This(Previous());
      }
      if (Match(TokenType.SUPER))
      {
        return new Super(Previous());
      }
      if (Match(TokenType.IDENTIFIER))
      {
        return new Variable(Previous());
      }
      if (Match(TokenType.LEFT_PAREN))
      {
        var expression = Expression();
        Consume(TokenType.RIGHT_PAREN, "Expected ')' after expression");
        return new Grouping(expression);
      }

      Error(Peek(), "Expression expected");
      throw new ParserException($"Expression expected at line {Previous().Line}");
    }

    private IExpression FinishCall(IExpression callee)
    {
      var args = new List<IExpression>();

      if (!Check(TokenType.RIGHT_PAREN))
      {
        while (true)
        {
          args.Add(Expression());

          if (args.Count > 254)
          {
            Error(Peek(),
               "A function can't take more than 254 arguments");
          }

          if (!Match(TokenType.COMMA))
          {
            break;
          }
        }
      }

      var parentheses = Consume(TokenType.RIGHT_PAREN, "Expected ')' after function arguments");

      return new Call(callee, parentheses, args);
    }

    private IExpression Call()
    {
      var expr = Primary();

      while (true)
      {
        if (Match(TokenType.LEFT_PAREN))
        {
          expr = FinishCall(expr);
        }
        else if (Match(TokenType.DOT))
        {
          var propertyName = Consume(TokenType.IDENTIFIER, "Expect property name after object access op");

          var isSuperClassProperty = Tokens[Current - 3].Type == TokenType.SUPER;

          expr = new AccessObjectProperty(expr, propertyName, isSuperClassProperty);
        }
        else
        {
          break;
        }
      }

      return expr;
    }

    private IExpression Unary()
    {
      if (Match(TokenType.BANG, TokenType.DOUBLE_BANG))
      {
        var op = Previous();
        var right = Unary();
        return new Unary(op, right);
      }

      return Call();
    }

    private IExpression NullCoalescing()
    {
      var expr = Unary();

      while (Match(TokenType.NULL_COALESCING))
      {
        var op = Previous();
        var right = Expression();
        expr = new Binary(expr, op, new Literal(right));
      }

      return expr;
    }

    private IExpression Multiplication()
    {
      var expr = NullCoalescing();

      while (Match(TokenType.SLASH, TokenType.STAR))
      {
        var op = Previous();
        var right = Unary();
        expr = new Binary(expr, op, right);
      }

      return expr;
    }

    private IExpression Mod()
    {
      var expr = Multiplication();

      while (Match(TokenType.MOD))
      {
        var op = Previous();
        var right = Multiplication();
        expr = new Binary(expr, op, right);
      }

      return expr;
    }

    private IExpression Addition()
    {
      var expr = Mod();

      while (Match(TokenType.MINUS, TokenType.PLUS))
      {
        var op = Previous();
        var right = Mod();
        expr = new Binary(expr, op, right);
      }

      return expr;
    }

    private IExpression Comparison()
    {
      var expr = Addition();

      while (Match(
        TokenType.GREATER,
        TokenType.GREATER_EQUAL,
        // ? TokenType.LEFT_BRACE,
        TokenType.LESS,
        TokenType.LESS_EQUAL
      ))
      {
        var op = Previous();
        var right = Addition();
        expr = new Binary(expr, op, right);
      }

      return expr;
    }

    private IExpression Equality()
    {
      var expr = Comparison();

      while (Match(TokenType.BANG_EQUAL, TokenType.EQUAL_EQUAL))
      {
        var op = Previous();
        var right = Comparison();
        expr = new Binary(expr, op, right);
      }

      return expr;
    }

    private IExpression And()
    {
      var expr = Equality();

      while (Match(TokenType.AND))
      {
        var op = Previous();
        var right = Equality();
        expr = new Logical(expr, op, right);
      }

      return expr;
    }

    private IExpression Or()
    {
      var expr = And();

      while (Match(TokenType.OR))
      {
        var op = Previous();
        var right = And();
        expr = new Logical(expr, op, right);
      }

      return expr;
    }

    private IExpression Assignment()
    {
      var expr = Or();

      if (Match(TokenType.EQUAL))
      {
        var equals = Previous();
        var value = Assignment();

        if (expr is Variable)
        {
          return new Assign((expr as Variable).Name, value);
        }
        if (expr is AccessObjectProperty)
        {
          return new SetObjectProperty((expr as AccessObjectProperty).Object,
                                       (expr as AccessObjectProperty).Token,
                                       value);

        }

        Error(equals, "Invalid assignment target");
      }

      return expr;
    }

    private IExpression Expression() => Assignment();

    private IStatement ExpressionStatement()
    {
      var expr = Expression();
      return new Expression(expr);
    }

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

      IExpression condition = null;
      if (Match(TokenType.SEMI_COLON))
      {
        condition = Expression();
      }

      Consume(TokenType.SEMI_COLON, "Expected ';' after for loop condition");

      var increment = Match(TokenType.RIGHT_PAREN)
        ? null
        : Expression();

      Consume(TokenType.RIGHT_PAREN, "Expected ')' after for loop statement");

      var body = Statement();
      if (increment != null)
      {
        body = new Block(new List<IStatement> { body, new Expression(increment) });
      }
      /* body = increment
        ? new Block({ body, Expression(increment) })
        : null; */

      if (condition == null)
      {
        condition = new Literal(true);
      }

      body = new While(condition, body);


      /*       var initializer = Match(TokenType.LET)
            ? LetDeclaration()
            : ExpressionStatement(); */
      if (Match(TokenType.LET))
      {
        var initializer = LetDeclaration();
        if (initializer != null)
        {
          body = new Block(new List<IStatement> { initializer, body });
        }
      }
      else
      {
        var initializer = ExpressionStatement();
        if (initializer != null)
        {
          body = new Block(new List<IStatement> { initializer, body });
        }
      }

      return body;
    }

    private List<IStatement> BlockStatement()
    {
      var statements = new List<IStatement>();

      while (!Match(TokenType.RIGHT_BRACE) && !EndOfTokens())
      {
        var declaration = Declaration();
        if (declaration != null)
        {
          statements.Add(declaration);
        }
      }

      Consume(TokenType.RIGHT_BRACE, "Expected '}' after block.");
      return statements;
    }

    private IStatement ConstDeclaration()
    {
      var name = Consume(TokenType.IDENTIFIER, "Expected variable name");

      Consume(TokenType.EQUAL, $"Expect initializer for const variable '${name.Lexeme}");

      var initializer = Expression();

      return new Const(name, initializer);
    }

    private IStatement LetDeclaration()
    {
      var name = Consume(TokenType.IDENTIFIER, "Expected variable name");
      var initializer = Match(TokenType.EQUAL)
        ? Expression()
        : null;

      return new Let(name, initializer);
    }


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
          {
            break;
          }
        }
      }

      Consume(TokenType.RIGHT_PAREN, $"Expected ')' after {kind} parameters");
      Consume(TokenType.LEFT_BRACE, $"Expected '[' before {kind} body");


      var body = BlockStatement();
      return new Function(name, parameters, body);
    }

    private IStatement WhileStatement()
    {
      Consume(TokenType.LEFT_PAREN, "Expected '(' after while keyword");

      var condition = Expression();

      Consume(TokenType.RIGHT_PAREN, "Expected ')' after while condition");

      var body = Statement();

      return new While(condition, body);
    }

    private IStatement ReturnStatement()
    {
      var keyword = Previous();

      var value = Match(TokenType.RIGHT_BRACE) ? null : Expression();

      return new Return(keyword, value);
    }

    private IStatement PrintStatement()
    {
      var value = Expression();
      return new Print(value);
    }

    private IStatement Statement()
    {
      if (Match(TokenType.PRINT))
      {
        return PrintStatement();
      }
      if (Match(TokenType.RETURN))
      {
        return ReturnStatement();
      }
      if (Match(TokenType.WHILE))
      {
        return WhileStatement();
      }
      if (Match(TokenType.LEFT_BRACE))
      {
        return new Block(BlockStatement());
      }
      if (Match(TokenType.FOR))
      {
        return ForStatement();
      }
      if (Match(TokenType.IF))
      {
        return IfStatement();
      }

      return ExpressionStatement();
    }

    private IStatement ClassDeclaration()
    {
      var className = Consume(TokenType.IDENTIFIER, "Expected identifier after class keyword");

      Variable superClass = null;
      if (Match(TokenType.CLASS_EXTENDS))
      {
        Consume(TokenType.IDENTIFIER, "Expected superclass name as extends keyword");
        superClass = new Variable(Previous());
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
        {
          classMethods.Add(FunctionDeclaration("method"));
        }
      }

      Consume(TokenType.RIGHT_BRACE, "Expected } after class body");
      return new Class(className, superClass, classMethods, staticMethods);
    }

    private IStatement Declaration()
    {
      try
      {
        if (Match(TokenType.CLASS))
        {
          return ClassDeclaration();
        }
        if (Match(TokenType.FUNCTION))
        {
          return FunctionDeclaration("function");
        }
        if (Match(TokenType.LET))
        {
          return LetDeclaration();
        }
        if (Match(TokenType.CONST))
        {
          return ConstDeclaration();
        }

        return Statement();
      }
      catch (System.Exception)
      {
        Synchronize();
        return null;
      }
    }

    public ParserResult Parse()
    {
      var statements = new List<IStatement>();

      while (!EndOfTokens())
      {
        var declaration = Declaration();

        if (declaration != null)
        {
          statements.Add(declaration);
        }
      }

      return new ParserResult { Ok = Ok, Statements = statements };
    }
  }
}