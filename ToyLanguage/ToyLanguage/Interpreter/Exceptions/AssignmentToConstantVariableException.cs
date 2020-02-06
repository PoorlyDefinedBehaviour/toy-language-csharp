using System;

namespace ToyLanguage.Interpreter.Exceptions
{
    internal class AssignmentToConstantVariableException : Exception
    {
        public AssignmentToConstantVariableException(string identifier) : base($"Assignment to constant variable <{identifier}>")
        {
        }
    }
}