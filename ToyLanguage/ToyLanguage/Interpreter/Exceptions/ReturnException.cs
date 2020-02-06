using System;

namespace ToyLanguage.Interpreter.Exceptions
{
    internal class ReturnException<T> : Exception
    {
        public T Value { get; set; }

        public ReturnException() : base()
        {
        }
    }
}