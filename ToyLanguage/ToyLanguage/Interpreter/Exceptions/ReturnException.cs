using System;

namespace ToyLanguage.Interpreter.Exceptions
{
  internal class ReturnException : Exception
  {
    public Object Value { get; set; }

    public ReturnException(Object value) : base()
    {
      Value = value;
    }
  }
}