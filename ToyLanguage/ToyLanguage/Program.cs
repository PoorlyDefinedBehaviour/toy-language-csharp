using System;

using ToyLanguage.Repl;

internal class Program
{
  private static void Main(string[] args)
  {
    var repl = new Repl();

    if (args.Length > 1)
      if (args[1] == "--debug")
        repl.Loop(debug: true);
      else
        repl.FromFile(args[1]);
    else
      repl.Loop(debug: false);
  }
}
