using System;

using ToyLanguage.Interpreter.builtins.Class;
using ToyLanguage.Interpreter.Exceptions;
using ToyLanguage.Interpreter.Interfaces;
using ToyLanguage.Parser.Statements;

namespace ToyLanguage.Interpreter.builtins.Callable
{
    internal class BuiltinFunction : ICallable
    {
        private readonly Environment.Environment closure;
        public Function Declaration { get; }

        public BuiltinFunction(Function declaration, Environment.Environment closure)
        {
            this.closure = closure;
            Declaration = declaration;
        }

        public BuiltinFunction Bind(BuiltinClass instance, BuiltinClass superClass)
        {
            Environment.Environment environment = new Environment.Environment()
              .SetParentEnvironment(closure)
              .Define("this", instance)
              .Define("super", superClass);

            return new BuiltinFunction(Declaration, environment);
        }

        public int Arity() => Declaration.Parameters.Count;

        public string Name() => Declaration.Name.Lexeme;

        [Obsolete]
        public T Call<T>(Interpreter interpreter, object[] args)
        {
            Environment.Environment environment = new Environment.Environment().SetParentEnvironment(closure);

            for (int i = 0; i < Declaration.Parameters.Count; ++i)
                environment.Define(Declaration.Parameters[i].Lexeme, args[i]);

            try
            {
                //interpreter.executeBlock(Declaration.Body, environment);
            }
            catch (ReturnException<T> e)
            {
                return e.Value;
            }

            return default;
        }
    }
}