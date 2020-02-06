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
        private int current = 0;
        private bool ok = true;
        private readonly List<Token> tokens;

        public Parser(List<Token> tokens) => this.tokens = tokens;

        private Token Previous() => tokens[current - 1];

        private Token Peek(int offset = 0) => tokens[current + offset];

        private bool EndOfTokens() => Peek().Type == TokenType.END_OF_FILE;

        private Token Advance() => tokens[++current];

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
            {
                Console.WriteLine($"<{token.Line}> ${message}");
            }
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

            Error(Peek(), message);
            throw new ParserException($"Expected <{Peek().Lexeme.ToString()} after {Previous().Lexeme.ToString()}");
        }

        private IExpression Primary()
        {
            if (Match(TokenType.FALSE))
                return new Literal(false);

            if (Match(TokenType.TRUE))
                return new Literal(true);

            if (Match(TokenType.NULL))
                return new Literal(null);

            if (Match(TokenType.NULL, TokenType.STRING))
                return new Literal(Previous().Literal);

            if (Match(TokenType.THIS))
                return new This(Previous());

            if (Match(TokenType.SUPER))
                return new Super(Previous());

            if (Match(TokenType.IDENTIFIER))
                return new Variable(Previous());

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

            return new Call(callee, Consume(TokenType.RIGHT_PAREN, "Expected ')' after function arguments"), args);
        }

        private IExpression Call()
        {
            IExpression expr = Primary();

            while (true)
            {
                if (Match(TokenType.LEFT_PAREN))
                {
                    expr = FinishCall(expr);
                }
                else if (Match(TokenType.DOT))
                {
                    expr = new AccessObjectProperty(expr,
                        Consume(TokenType.IDENTIFIER, "Expect property name after object access op"),
                        tokens[current - 3].Type == TokenType.SUPER);
                }
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
              // ? TokenType.LEFT_BRACE,
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

                Error(Previous(), "Invalid assignment target");
            }

            return expr;
        }

        private IExpression Expression() => Assignment();

        private IStatement ExpressionStatement() => new Expression(Expression());

        private IStatement IfStatement()
        {
            Consume(TokenType.LEFT_PAREN, "Expected '(' after if keyword");

            Consume(TokenType.RIGHT_PAREN, "Expected ')' after if condition");

            return new If(
                Expression(),
                Statement(),
                Match(TokenType.ELSE) ? Statement() : null);
        }

        private IStatement ForStatement()
        {
            Consume(TokenType.LEFT_PAREN, "Expected '(' after for keyword");

            IExpression condition = null;
            if (Match(TokenType.SEMI_COLON))
                condition = Expression();

            Consume(TokenType.SEMI_COLON, "Expected ';' after for loop condition");
            Consume(TokenType.RIGHT_PAREN, "Expected ')' after for loop statement");

            IStatement body = Statement();
            IExpression increment = Match(TokenType.RIGHT_PAREN) ? null : Expression();

            if (increment != null)
                body = new Block(new List<IStatement> { body, new Expression(increment) });

            /* body = increment
              ? new Block({ body, Expression(increment) })
              : null; */

            //if (condition == null)
            //    condition = new Literal(true);
            condition ??= new Literal(true);

            body = new While(condition, body);

            /*       var initializer = Match(TokenType.LET)
                  ? LetDeclaration()
                  : ExpressionStatement(); */
            if (Match(TokenType.LET))
            {
                IStatement initializer = LetDeclaration();
                if (initializer != null)
                    body = new Block(new List<IStatement> { initializer, body });
            }
            else
            {
                IStatement initializer = ExpressionStatement();
                if (initializer != null)
                    body = new Block(new List<IStatement> { initializer, body });
            }

            return body;
        }

        private List<IStatement> BlockStatement()
        {
            var statements = new List<IStatement>();

            while (!Match(TokenType.RIGHT_BRACE) && !EndOfTokens())
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
            Consume(TokenType.LEFT_BRACE, $"Expected '[' before {kind} body");

            return new Function(Consume(TokenType.IDENTIFIER, $"Expected {kind} name"), parameters, BlockStatement());
        }

        private IStatement WhileStatement()
        {
            Consume(TokenType.LEFT_PAREN, "Expected '(' after while keyword");
            Consume(TokenType.RIGHT_PAREN, "Expected ')' after while condition");

            return new While(Expression(), Statement());
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

            return Match(TokenType.IF) ? IfStatement() : ExpressionStatement();
        }

        private IStatement ClassDeclaration()
        {
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

                return Match(TokenType.CONST) ? ConstDeclaration() : Statement();
            }
            catch (Exception)
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