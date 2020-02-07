using System;
using System.Collections.Generic;

using ToyLanguage.Interfaces;
using ToyLanguage.Interpreter.builtins.Class;
using ToyLanguage.Interpreter.builtins.Callable;
using ToyLanguage.Interpreter.Exceptions;
using ToyLanguage.Interpreter.Interfaces;
using ToyLanguage.Lexer;
using ToyLanguage.Parser.Expressions;
using ToyLanguage.Parser.Statements;

namespace ToyLanguage.Interpreter
{
  internal class Interpreter : IExpressionVisitor<object>, IStatementVisitor<object>
  {
    public Environment.Environment Globals = new Environment.Environment();
    private Dictionary<IExpression, int> Locals = new Dictionary<IExpression, int>();
    public Environment.Environment Environment { get; set; }

    public Interpreter()
    {
      Environment = new Environment.Environment().SetParentEnvironment(Globals);
    }

    private bool IsTruthy(object obj)
    {
      if (obj == null)
        return false;

      if (obj as int? == 0)
        return false;

      if (obj as string == "")
        return false;

      if (obj as bool? == false)
        return false;

      return true;
    }

    private bool IsDigit(object c) => (c as char?) >= '0' && (c as char?) <= '9';

    public object VisitBinaryExpression(Binary expression)
    {
      object left = expression.Left.Accept(this);
      object right = expression.Right.Accept(this);

      if (IsDigit(left) && IsDigit(right))
        return Convert.ToDouble(left) + Convert.ToDouble(right);

      return expression.Operation.Type switch
      {
        TokenType.PLUS => Convert.ToString(left) + Convert.ToString(right),
        TokenType.MINUS => Convert.ToDouble(left) - Convert.ToDouble(right),
        TokenType.SLASH => Convert.ToDouble(left) / Convert.ToDouble(right),
        TokenType.STAR => Convert.ToDouble(left) * Convert.ToDouble(right),
        TokenType.GREATER => Convert.ToDouble(left) > Convert.ToDouble(right),
        TokenType.GREATER_EQUAL => Convert.ToDouble(left) >= Convert.ToDouble(right),
        TokenType.LESS => Convert.ToDouble(left) < Convert.ToDouble(right),
        TokenType.LESS_EQUAL => Convert.ToDouble(left) <= Convert.ToDouble(right),
        TokenType.BANG_EQUAL => Convert.ToDouble(left) != Convert.ToDouble(right),
        TokenType.EQUAL_EQUAL => Convert.ToDouble(left) == Convert.ToDouble(right),
        TokenType.MOD => Convert.ToDouble(left) % Convert.ToDouble(right),
        TokenType.NULL_COALESCING => VisitNullCoalescingExpression(left, right),
        _ => null,
      };
    }

    private object VisitNullCoalescingExpression(object left, object right)
    {
      if (left != null)
        return left;

      return (right as IExpression).Accept(this);

    }

    private object LookUpVariable(Variable expression)
    {
      if (Locals.ContainsKey(expression))
        return Environment.GetAtScope(Locals[expression], expression.Name);

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
    }

    public object VisitGroupingExpression(Grouping expression) =>
        expression.Expression.Accept(this);

    public object VisitLiteralExpression(Literal expression) => expression.Value;


    public object VisitAssignExpression(Assign expression)
    {
      object value = expression.Value.Accept(this);

      if (Locals.ContainsKey(expression))
        Environment.AssignAtScope(Locals[expression], expression.Name, value);
      else
        Environment.Assign(expression.Name, value);

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
        return null;
      }

      throw new RuntimeException($"Object <{@object}> is not a class instance");
    }

    public object VisitVariableExpression(Variable expression) => LookUpVariable(expression);

    public object VisitCallExpression(Call expression)
    {
      var callee = expression.Callee.Accept(this);

      if (!(callee is ICallable))
        throw new ObjectIsNotCallableException(callee);

      var args = new List<object>();
      expression.Args.ForEach(arg => args.Add(arg.Accept(this)));

      var callable = callee as ICallable;

      if (args.Count != callable.Arity())
      {
        throw new InvalidNumberOfArgumentsException(
          callable.Name(),
          callable.Arity(),
          args.Count
        );
      }

      return callable.Call(this, args);
    }

    public object VisitAccessObjectPropertyExpression(AccessObjectProperty expression)
    {
      object @object = expression.Object.Accept(this);
      string property = expression.Token.Lexeme;

      if (@object is BuiltinClass builtin)
      {
        if (expression.IsSuperClassProperty)
        {
          var method = @builtin.IsClassInstance
            ? @builtin.GetSuperClassProperty(property)
            : @builtin.GetSuperClassStaticMethod(property);

          if (method != null)
            return method;
        }
        else
        {
          var method = @builtin.IsClassInstance
            ? @builtin.GetProperty(property)
            : @builtin.GetStaticMethod(property);

          if (method != null)
            return method;
        }
      }

      throw new RuntimeException($"<{@object.ToString()}> has no property called <{property}>");
    }

    public object VisitBlockStatement(Block statement)
    {
      var enviroment = new Environment.Environment().SetParentEnvironment(this.Environment);
      ExecuteBlock(statement.Statements, enviroment);
      return null;
    }

    public void ExecuteBlock(List<IStatement> block, Environment.Environment environment)
    {
      var previousEnvironment = this.Environment;

      try
      {
        this.Environment = environment;
        block.ForEach(stmt => stmt.Accept(this));
      }
      catch (System.Exception)
      {
        this.Environment = previousEnvironment;
      }
    }

    public object VisitClassStatement(Class statement)
    {
      string className = statement.Name.Lexeme;
      var superclass = statement.Superclass?.Accept(this);

      if (superclass != null && !(superclass is BuiltinClass))
        throw new RuntimeException($"<{superclass}> is not a class");

      this.Environment.Define(className, null);

      var classMethods = new Dictionary<string, BuiltinFunction>();
      statement.Methods.ForEach(method =>
        classMethods.Add(
            method.Name.Lexeme,
            new BuiltinFunction(method, Environment)
        ));

      var staticMethods = new Dictionary<string, BuiltinFunction>();
      statement.StaticMethods.ForEach(method =>
        staticMethods.Add(
            method.Name.Lexeme,
            new BuiltinFunction(method, Environment)
        ));

      var builtinClass = new BuiltinClass(
        className,
        superclass as BuiltinClass,
        classMethods,
        staticMethods
      );

      Environment.Assign(statement.Name, builtinClass);

      return null;
    }

    public object VisitConstStatement(Const statement)
    {
      var value = statement.Initializer.Accept(this);
      Environment.Define(statement.Name.Lexeme, value);
      return null;
    }

    public object VisitExpressionStatement(Expression statement) => statement.CurrentExpression.Accept(this);

    public object VisitFunctionStatement(Function statement)
    {
      var fn = new BuiltinFunction(statement, Environment);
      Environment.Define(statement.Name.Lexeme, fn);
      return null;
    }

    public object VisitIfStatement(If statement)
    {
      if (IsTruthy(statement.Condition.Accept(this)))
        statement.ThenBranch.Accept(this);
      else if (statement.ElseBranch != null)
        statement.ElseBranch.Accept(this);

      return null;
    }

    public object VisitLetStatement(Let statement)
    {
      var value = statement.Initializer?.Accept(this);
      Environment.Define(statement.Name.Lexeme, value);
      return null;
    }

    public object VisitPrintStatement(Print statement)
    {
      var value = statement.Expression.Accept(this) as object;
      Console.WriteLine(value.ToString());
      return null;
    }

    public object VisitReturnStatement(Return statement)
    {
      var value = statement.Value?.Accept(this);
      throw new ReturnException(value);
    }

    public object VisitWhileStatement(While statement)
    {

      while (IsTruthy(statement.Condition.Accept(this)))
        statement.Body.Accept(this);

      return null;
    }

    public Interpreter Resolve(IExpression expression, int depth)
    {
      Locals.Add(expression, depth);
      return this;
    }

    public void Run(List<IStatement> statements)
    {
      try
      {
        statements.ForEach(stmt => stmt.Accept(this));
      }
      catch (System.Exception e)
      {
        Console.WriteLine(e.Message);
      }
    }
  }
}