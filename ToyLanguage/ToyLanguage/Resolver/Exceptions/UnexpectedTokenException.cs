using System;

namespace ToyLanguage.Resolver.Exceptions
{
    internal class UnexpectedTokenException : Exception
    {
        public UnexpectedTokenException(string tokenName) : base($"Unexpected <{tokenName}>")
        {
        }
    }
}