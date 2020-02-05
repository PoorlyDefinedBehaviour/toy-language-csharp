using System;

namespace ToyLanguage.Resolver.Exceptions
{
    internal class InvalidInitializerException : Exception
    {
        public InvalidInitializerException(object initializer) : base($"Invalid initializer {initializer}")
        {
        }
    }
}