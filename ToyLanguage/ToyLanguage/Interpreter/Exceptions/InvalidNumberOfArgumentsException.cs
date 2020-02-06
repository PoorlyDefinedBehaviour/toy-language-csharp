using System;

namespace ToyLanguage.Interpreter.Exceptions
{
    internal class InvalidNumberOfArgumentsException : Exception
    {
        public InvalidNumberOfArgumentsException(string functionName, int expected, int got) : base($"<{functionName}> expected {expected} arguments , got {got}`")
        {
        }
    }
}