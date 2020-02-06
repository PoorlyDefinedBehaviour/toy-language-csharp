namespace ToyLanguage.Interpreter.Interfaces
{
    internal interface ICallable<T>
    {
        double Arity();

        string Name();

        T Call(Interpreter interpreter, object[] args);
    }
}