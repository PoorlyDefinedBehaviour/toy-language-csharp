using System;
using ToyLanguage.Interpreter.builtins.Class;
using ToyLanguage.Interpreter.Interfaces;
using ToyLanguage.Parser.Statements;

namespace ToyLanguage.Interpreter.builtins.Callable
{
    internal class BuiltinFunction : ICallable
    {
        private readonly Environment.Environment closure;
        public Function Declaration { get; }

        public BuiltinFunction(Environment.Environment closure, Function declaration)
        {
            this.closure = closure;
            Declaration = declaration;
        }

        public BuiltinFunction Bind(BuiltinClass instance, BuiltinClass superClass)
        {
            var environment = new Environment.Environment()
              .setParentEnvironment(closure)
              .define("this", instance)
              .define("super", superClass);

            return new BuiltinFunction(Declaration, environment);
        }

        public int Arity() => throw new NotImplementedException();

        public T Call<T>(Interpreter interpreter, object[] args) => throw new NotImplementedException();

        public string Name() => throw new NotImplementedException();
    }
}