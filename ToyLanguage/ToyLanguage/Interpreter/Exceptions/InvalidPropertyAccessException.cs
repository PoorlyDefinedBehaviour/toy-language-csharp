using System;

namespace ToyLanguage.Interpreter.Exceptions
{
    internal class InvalidPropertyAccessException : Exception
    {
        public InvalidPropertyAccessException(string propertyName) : base($"Tried to access property <{propertyName}> not on object")
        {
        }
    }
}