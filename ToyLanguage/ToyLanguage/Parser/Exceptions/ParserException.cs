using System;

namespace ToyLanguage.Parser.Exceptions
{
    internal class ParserException : Exception
    {
        public ParserException(string message) : base(message)
        {
        }
    }
}