using System;
using System.Collections.Generic;

using ToyLanguage.Interpreter.Interfaces;

namespace ToyLanguage.Interpreter.builtins.Class
{
    internal class BuiltinClass : ICallable
    {
        private Dictionary<string, object> properties = new Dictionary<string, object>();

        private readonly string name;
        private BuiltinClass superclass;
        private Dictionary<string, BuiltinFunction> methods;
        private Dictionary<string, BuiltinFunction> staticMethods;
        private bool isClassInstance = false;

        public BuiltinClass()
        {

        }

        public override string ToString() => name;

        public int Arity() => throw new NotImplementedException();

        public T Call<T>(Interpreter interpreter, object[] args) => throw new NotImplementedException();

        public string Name() => throw new NotImplementedException();
    }
}