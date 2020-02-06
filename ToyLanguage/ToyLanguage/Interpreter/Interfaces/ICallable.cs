using System;
using System.Collections.Generic;
using System.Text;

namespace ToyLanguage.Interpreter.Interfaces
{
    interface ICallable
    {
        int Arity();
        string Name();
        T Call<T>(Interpreter interpreter, object[] args);
    }
}
