using System.Collections.Generic;

namespace ToyLanguage.Interpreter.Interfaces
{
  internal interface ICallable
  {
    double Arity();

    string Name();

    object Call(Interpreter interpreter, List<object> args);
  }
}