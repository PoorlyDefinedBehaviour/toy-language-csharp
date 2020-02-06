using System;

namespace ToyLanguage.Interpreter.Exceptions
{
    internal class ReturnException<T> : Exception
    {
        public ReturnException() : base()
        {
        }
    }
}