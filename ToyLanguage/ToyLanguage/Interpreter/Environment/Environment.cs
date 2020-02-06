using System.Collections.Generic;

using ToyLanguage.Interpreter.Exceptions;
using ToyLanguage.Lexer;

namespace ToyLanguage.Interpreter.Environment
{
    internal class Environment
    {
        private Environment parentEnvironment;
        private readonly Dictionary<string, object> values = new Dictionary<string, object>();
        private readonly Dictionary<string, object> constValues = new Dictionary<string, object>();

        public Environment SetParentEnvironment(Environment environment)
        {
            parentEnvironment = environment;
            return this;
        }

        public Environment Define(string name, object value, bool @const = false)
        {
            if (values.ContainsKey(name) || constValues.ContainsKey(name))
                throw new IdentifierAlreadyDeclaredException(name);

            if (@const)
            {
                constValues[name] = value;
            }
            else
                values[name] = value;

            return this;
        }

        public Environment Assign(Token token, object value)
        {
            if (values.ContainsKey(token.Lexeme))
            {
                values[token.Lexeme] = value;
                return this;
            }

            if (constValues.ContainsKey(token.Lexeme))
                throw new AssignmentToConstantVariableException(token.Lexeme);

            if (parentEnvironment != null)
            {
                parentEnvironment.Assign(token, value);
                return this;
            }

            throw new UndefinedVariableException($"Assignment on undefined variable {token.Lexeme}");
        }

        public object Get(Token variableName)
        {
            if (values.ContainsKey(variableName.Lexeme))
                return values[variableName.Lexeme];

            if (constValues.ContainsKey(variableName.Lexeme))
                return constValues[variableName.Lexeme];

            if (parentEnvironment != null)
                return parentEnvironment.Get(variableName);

            throw new UndefinedVariableException(variableName.Lexeme);
        }

        private Environment GetEnvironmentAt(int scopeDistance)
        {
            Environment currentEnvironment = this;

            for (int i = 0; i < scopeDistance; ++i)
                currentEnvironment = currentEnvironment.parentEnvironment;

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