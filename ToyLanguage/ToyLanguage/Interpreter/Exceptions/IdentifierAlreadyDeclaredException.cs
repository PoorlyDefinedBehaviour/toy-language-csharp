using System;

namespace ToyLanguage.Interpreter.Exceptions
{
    internal class IdentifierAlreadyDeclaredException : Exception
    {
        public IdentifierAlreadyDeclaredException(string identifier) : base($"Identifier <{identifier}> has already been declared`")
        {
        }
    }
}