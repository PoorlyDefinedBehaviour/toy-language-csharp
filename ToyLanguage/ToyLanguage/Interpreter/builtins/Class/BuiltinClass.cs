using System.Collections.Generic;

using ToyLanguage.Interpreter.builtins.Callable;
using ToyLanguage.Interpreter.Exceptions;
using ToyLanguage.Interpreter.Interfaces;

namespace ToyLanguage.Interpreter.builtins.Class
{
  internal class BuiltinClass : ICallable
  {
    private readonly Dictionary<string, object> properties = new Dictionary<string, object>();

    private readonly string name;
    private readonly BuiltinClass superclass;
    private readonly Dictionary<string, BuiltinFunction> methods;
    private readonly Dictionary<string, BuiltinFunction> staticMethods;

    public bool IsClassInstance { get; }

    public BuiltinClass(string name, BuiltinClass builtinClass, Dictionary<string, BuiltinFunction> methods,
        Dictionary<string, BuiltinFunction> staticMethods, bool isClassInstance = false)
    {
      this.name = name;
      superclass = builtinClass;
      this.methods = methods;
      this.staticMethods = staticMethods;
      this.IsClassInstance = isClassInstance;
    }

    public override string ToString() => name;

    public double Arity()
    {
      BuiltinFunction constructor = GetMethod("constructor");
      return constructor != null ? constructor.Arity() : 0;
    }

    public object Call(Interpreter interpreter, List<object> args)
    {
      var instance = new BuiltinClass(
        name,
        superclass.Call(interpreter, args) as BuiltinClass,
        methods,
        staticMethods,
        true
        );

      BuiltinFunction constructor = GetMethod("constructor");

      if (constructor != null)
        constructor.Bind(instance, superclass).Call(interpreter, args);

      return instance;
    }

    public string Name() => name;

    public BuiltinFunction GetMethod(string name) =>
        methods.ContainsKey(name) ? methods[name] : superclass?.GetMethod(name);

    public BuiltinFunction GetStaticMethod(string name) =>
        staticMethods.ContainsKey(name) ? staticMethods[name] : superclass?.GetStaticMethod(name);

    public object GetProperty(string name)
    {
      if (properties.ContainsKey(name))
        return properties[name];

      BuiltinFunction method = GetMethod(name);
      if (method != null)
      {
        return method.Bind(this, superclass);
      }

      if (superclass != null)
      {
        BuiltinFunction superMethod = superclass.GetMethod(name);
        if (superMethod != null)
        {
          return method.Bind(this, superclass);
        }
      }

      throw new RuntimeException($"Object <{ ToString() } > has no property <{name}>");
    }

    public BuiltinClass SetProperty(string name, object value)
    {
      properties[name] = value;
      return this;
    }

    public BuiltinFunction GetSuperClassMethod(string name) => superclass?.GetMethod(name);

    public BuiltinFunction GetSuperClassStaticMethod(string name) => superclass?.GetStaticMethod(name);

    public BuiltinFunction GetSuperClassProperty(string name)
    {
      if (superclass != null)
        if (superclass.GetProperty(name) is BuiltinFunction method)
          return method.Bind(this, superclass);

      return null;
    }
  }
}