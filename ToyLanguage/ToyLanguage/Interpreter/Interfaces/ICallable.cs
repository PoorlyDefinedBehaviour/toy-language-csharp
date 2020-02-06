using System;
using System.Collections.Generic;
using System.Text;

namespace ToyLanguage.Interpreter.Interfaces
{
    interface ICallable<T>
    {
        int Arity();
        string Name();
        T Call(Interpreter interpreter, object[] args);
    }
}
