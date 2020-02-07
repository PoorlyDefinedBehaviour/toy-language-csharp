using System;

namespace ToyLanguage.Interpreter.Exceptions
{
  internal class ObjectIsNotCallableException : Exception
  {
    public ObjectIsNotCallableException(object @object) : base($"Object <${@object}> is not callable")
    {
    }
  }
}