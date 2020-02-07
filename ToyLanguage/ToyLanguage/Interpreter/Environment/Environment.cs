using System.Collections.Generic;
using System;

using ToyLanguage.Interpreter.Exceptions;
using ToyLanguage.Lexer;

namespace ToyLanguage.Interpreter.Environment
{
  internal class Environment
  {
    private Environment ParentEnvironment;
    private readonly Dictionary<string, object> Values = new Dictionary<string, object>();
    private readonly Dictionary<string, object> ConstValues = new Dictionary<string, object>();

    public Environment SetParentEnvironment(Environment environment)
    {
      ParentEnvironment = environment;
      return this;
    }

    public Environment Define(string name, object value, bool @const = false)
    {
      if (Values.ContainsKey(name) || ConstValues.ContainsKey(name))
        throw new IdentifierAlreadyDeclaredException(name);

      if (@const)
        ConstValues.Add(name, value);
      else
        Values.Add(name, value);

      return this;
    }

    public Environment Assign(Token token, object value)
    {
      if (Values.ContainsKey(token.Lexeme))
      {
        Values[token.Lexeme] = value;
        return this;
      }

      if (ConstValues.ContainsKey(token.Lexeme))
        throw new AssignmentToConstantVariableException(token.Lexeme);

      if (ParentEnvironment != null)
      {
        ParentEnvironment.Assign(token, value);
        return this;
      }

      throw new UndefinedVariableException($"Assignment on undefined variable <{token.Lexeme}>");
    }

    public object Get(Token variableName)
    {
      if (Values.ContainsKey(variableName.Lexeme))
        return Values[variableName.Lexeme];

      if (ConstValues.ContainsKey(variableName.Lexeme))
        return ConstValues[variableName.Lexeme];

      if (ParentEnvironment != null)
        return ParentEnvironment.Get(variableName);

      throw new UndefinedVariableException(variableName.Lexeme);
    }

    private Environment GetEnvironmentAt(int scopeDistance)
    {
      Environment currentEnvironment = this;

      for (int i = 0; i < scopeDistance; ++i)
        currentEnvironment = currentEnvironment.ParentEnvironment;

      return currentEnvironment;
    }

    public object GetAtScope(int scopeDistance, Token token) =>
        GetEnvironmentAt(scopeDistance).Get(token);

    public Environment AssignAtScope(int scopeDistance, Token token, object value)
    {
      GetEnvironmentAt(scopeDistance).Assign(token, value);
      return this;
    }
  }
}