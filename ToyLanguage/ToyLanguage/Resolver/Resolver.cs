using System;
using System.Collections.Generic;
using System.Linq;

using ToyLanguage.Interfaces;
using ToyLanguage.Lexer;
using ToyLanguage.Parser.Expressions;
using ToyLanguage.Parser.Statements;
using ToyLanguage.Resolver.Exceptions;

namespace ToyLanguage.Resolver
{
    internal enum ScopeTypes
    {
        NONE,
        FUNCTION,
        METHOD,
        CLASS_CONSTRUCTOR,
        SUB_CLASS_METHOD
    }

    internal class Resolver : IExpressionVisitor<object>, IStatementVisitor<object>
    {
        private ScopeTypes currentScopeType = ScopeTypes.NONE;

        // TODO
        // constructor(private interpreter: Interpreter) {}

        //private scopes: Map<string, boolean>[] = [];
        private readonly Stack<Dictionary<string, bool>> scopes = new Stack<Dictionary<string, bool>>();

        //private beginScope() : void { this.scopes.push(new Map<string, boolean>());}
        private void BeginScope() => scopes.Push(new Dictionary<string, bool>());

        private void EndScope() => scopes.Pop();

        private Dictionary<string, bool> GetCurrentScope() => scopes.Peek();

        private void Declare(Token token)
        {
            if (scopes.Count == 0)
                return;

            Dictionary<string, bool> currentScope = GetCurrentScope();

            if (currentScope.ContainsKey(token.Lexeme))
            {
                Console.WriteLine($"Redeclaring variable { token.Lexeme}");
            }
            else
                currentScope[token.Lexeme] = false;
        }

        private void Define(Token token)
        {
            if (scopes.Count == 0)
                return;

            GetCurrentScope()[token.Lexeme] = true;
        }

        [Obsolete]
        private void ResolveLocal(IExpression expression, Token token)
        {
            int scopesCount = scopes.Count - 1;

            for (int i = scopesCount; i > -1; --i)
            {
                if (scopes.ElementAt(i).ContainsKey(token.Lexeme))
                {
                    //this.interpreter.resolve(expression, scopesCount - i);
                    return;
                }
            }
        }

        private void ResolveFunction(Function statement, ScopeTypes scopeType)
        {
            ScopeTypes parentScopeType = currentScopeType;
            currentScopeType = scopeType;

            BeginScope();

            statement.Parameters.ForEach(param =>
            {
                Declare(param);
                Define(param);
            });
            statement.Body.ForEach(stmt => stmt.Accept(this));

            EndScope();

            currentScopeType = parentScopeType;
        }

        public object VisitBlockStatement(Block statement)
        {
            BeginScope();

            statement.Statements.ForEach(stmt => stmt.Accept(this));

            EndScope();

            return null;
        }

        public object VisitLetStatement(Let statement)
        {
            Declare(statement.Name);

            if (statement.Initializer != null)
                statement.Initializer.Accept(this);

            Define(statement.Name);

            return null;
        }

        public object VisitConstStatement(Const statement)
        {
            Declare(statement.Name);
            statement.Initializer.Accept(this);
            Define(statement.Name);

            return null;
        }

        [Obsolete]
        public object VisitVariableExpression(Variable expression)
        {
            if (scopes.Count > 0 && GetCurrentScope()[expression.Name.Lexeme] == false)
                throw new InvalidInitializerException(expression.Name.Lexeme);

            ResolveLocal(expression, expression.Name);

            return null;
        }

        [Obsolete]
        public object VisitAssignExpression(Assign expression)
        {
            expression.Value.Accept(this);
            ResolveLocal(expression, expression.Name);

            return null;
        }

        public object VisitFunctionStatement(Function statement)
        {
            Declare(statement.Name);
            Define(statement.Name);
            ResolveFunction(statement, ScopeTypes.FUNCTION);

            return null;
        }

        public object VisitExpressionStatement(IExpression statement)
        {
            statement.expression.accept(this);

            return null;
        }

        #region trash

        public object VisitAccessObjectPropertyExpression(AccessObjectProperty expression) => throw new NotImplementedException();

        public object VisitBinaryExpression(Binary expression) => throw new NotImplementedException();

        public object VisitCallExpression(Call expression) => throw new NotImplementedException();

        public object VisitClassStatement(Class statement) => throw new NotImplementedException();

        public object VisitGroupingExpression(Grouping expression) => throw new NotImplementedException();

        public object VisitIfStatement(If statement) => throw new NotImplementedException();

        public object VisitLiteralExpression(Literal expression) => throw new NotImplementedException();

        public object VisitLogicalExpression(Logical expression) => throw new NotImplementedException();

        public object VisitPrintStatement(Print statement) => throw new NotImplementedException();

        public object VisitReturnStatement(Return statement) => throw new NotImplementedException();

        public object VisitSetObjectPropertyExpression(SetObjectProperty expression) => throw new NotImplementedException();

        public object VisitSuperExpression(Super expression) => throw new NotImplementedException();

        public object VisitThisExpression(This expression) => throw new NotImplementedException();

        public object VisitUnaryExpression(Unary expression) => throw new NotImplementedException();

        public object VisitWhileStatement(While statement) => throw new NotImplementedException();

        #endregion trash
    }
}