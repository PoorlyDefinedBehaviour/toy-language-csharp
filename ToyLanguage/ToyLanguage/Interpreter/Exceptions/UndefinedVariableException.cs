using System;

namespace ToyLanguage.Interpreter.Exceptions
{
    internal class UndefinedVariableException : Exception
    {
        public UndefinedVariableException(string variableName) : base($"Undefined variable <{variableName}>")
        {
        }
    }
}