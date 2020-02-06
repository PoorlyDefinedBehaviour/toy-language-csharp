using System;

namespace ToyLanguage.Interpreter.Exceptions
{
    internal class RuntimeException : Exception
    {
        public RuntimeException(string message) : base(message)
        {
        }
    }
}