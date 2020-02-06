using System;
using System.Collections.Generic;

using ToyLanguage.Interfaces;
using ToyLanguage.Interpreter.builtins.Class;
using ToyLanguage.Interpreter.Exceptions;
using ToyLanguage.Lexer;
using ToyLanguage.Parser.Expressions;
using ToyLanguage.Parser.Statements;

namespace ToyLanguage.Interpreter
{
    internal class Interpreter : IExpressionVisitor<object>, IStatementVisitor<object>
    {
        public Environment.Environment globals = new Environment.Environment();
        private Dictionary<IExpression, int> locals = new Dictionary<IExpression, int>();
        public Environment.Environment Environment { get; }

        public Interpreter() => Environment = new Environment.Environment().SetParentEnvironment(globals);

        public object VisitBinaryExpression(Binary expression)
        {
            object left = expression.Left.Accept(this);
            object right = expression.Right.Accept(this);

            return expression.Operation.Type switch
            {
                TokenType.PLUS => (double)left + (double)right,
                TokenType.MINUS => (double)left - (double)right,
                TokenType.SLASH => (double)left / (double)right,
                TokenType.STAR => (double)left * (double)right,
                TokenType.GREATER => (double)left > (double)right,
                TokenType.GREATER_EQUAL => (double)left >= (double)right,
                TokenType.LESS => (double)left < (double)right,
                TokenType.LESS_EQUAL => (double)left <= (double)right,
                TokenType.BANG_EQUAL => (double)left != (double)right,
                TokenType.EQUAL_EQUAL => (double)left == (double)right,
                TokenType.MOD => (double)left % (double)right,
                TokenType.NULL_COALESCING => VisitNullCoalescingExpression((Expression)left, (Expression)right),
                _ => null,
            };
        }

        private object VisitNullCoalescingExpression(Expression left, Expression right) =>
            left == null ? right.Accept(this) : left.Accept(this) ?? right.Accept(this);

        private object LookUpVariable(Variable expression)
        {
            if (locals.ContainsKey(expression))
                return Environment.GetAtScope(locals[expression], expression.Name);

            return Environment.Get(expression.Name);
        }

        public object VisitUnaryExpression(Unary expression)
        {
            object right = expression.Right.Accept(this);

            return expression.Operation.Type switch
            {
                TokenType.BANG => !(bool)right,
                TokenType.DOUBLE_BANG => IsTruthy(right),
                TokenType.MINUS => -(double)right,
                _ => null,
            };

            static bool IsTruthy(object obj) =>
                !(obj is null) && !(obj as int?).Equals(0) && !(obj as string).Equals("") && !(obj as bool?).Equals(false);
        }

        public object VisitGroupingExpression(Grouping expression) =>
            expression.Expression.Accept(this);

        public object VisitLiteralExpression(Literal expression) => expression.Value;

        public object VisitVariableExpression(Variable expression) => LookUpVariable(expression);

        public object VisitAssignExpression(Assign expression)
        {
            object value = expression.Value.Accept(this);

            if (locals.ContainsKey(expression))
            {
                Environment.AssignAtScope(locals[expression], expression.Name, value);
            }
            else
            {
                globals.Assign(expression.Name, value);
            }
            return value;
        }

        public object VisitLogicalExpression(Logical expression)
        {
            object left = expression.Left.Accept(this);

            if (expression.Operator.Type == TokenType.OR && left != null)
            {
                return left;
            }
            else if (left != null)
                return left;

            return expression.Right.Accept(this);
        }

        public object VisitSetObjectPropertyExpression(SetObjectProperty expression)
        {
            object @object = expression.Object.Accept(this);

            if (@object is BuiltinClass builtin)
            {
                builtin.SetProperty(expression.Token.Lexeme, expression.Value.Accept(this));
            }
            else
                throw new RuntimeException($"Object <{@object}> is not a class instance");

            return null;
        }

        public object VisitThisExpression(This expression) => LookUpVariable(expression);

        public object VisitSuperExpression(Super expression) => LookUpVariable(expression);

        public object VisitCallExpression(Call expression)
        {
            
        }

        //finish

        public object VisitAccessObjectPropertyExpression(AccessObjectProperty expression)
        {
        }

        public object VisitBlockStatement(Block statement) => throw new NotImplementedException();

        public object VisitClassStatement(Class statement) => throw new NotImplementedException();

        public object VisitConstStatement(Const statement) => throw new NotImplementedException();

        public object VisitExpressionStatement(Expression statement) => throw new NotImplementedException();

        public object VisitFunctionStatement(Function statement) => throw new NotImplementedException();

        public object VisitIfStatement(If statement) => throw new NotImplementedException();

        public object VisitLetStatement(Let statement) => throw new NotImplementedException();

        public object VisitPrintStatement(Print statement) => throw new NotImplementedException();

        public object VisitReturnStatement(Return statement) => throw new NotImplementedException();

        public object VisitWhileStatement(While statement) => throw new NotImplementedException();
    }
}