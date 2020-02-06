using System;

namespace ToyLanguage.Interpreter.Exceptions
{
    internal class InvalidNumberOfArgumentsException : Exception
    {
        public InvalidNumberOfArgumentsException(string functionName, double expected, int got) : base($"<{functionName}> expected {expected} arguments , got {got}`")
        {
        }
    }
}