using System;
using System.IO;
using System.Collections.Generic;

using ToyLanguage.Lexer;

namespace ToyLanguage.Repl
{
  internal class Repl
  {
    private void DebugTokens(List<Token> tokens)
    {
      Console.WriteLine($"[Debug] Debugging {tokens.Count} tokens");
      tokens.ForEach(t => Console.WriteLine($"{t.Literal} => {t.Type}"));
    }

    public void FromFile(string filePath)
    {
      var sourceCode = File.ReadAllText(filePath);

      var tokens = new Tokenizer(sourceCode).Tokenize();

      var parser = new Parser.Parser(tokens);
      var interpreter = new Interpreter.Interpreter();

      var parserResult = parser.Run();
      if (parserResult.Ok)
      {
        new Resolver.Resolver(interpreter).Resolve(parserResult.Statements);
        interpreter.Run(parserResult.Statements);
      }
    }

    public void Loop(bool debug = false)
    {
      var interpreter = new Interpreter.Interpreter();
      var resolver = new Resolver.Resolver(interpreter);

      int openBrackets = 0;
      string buffer = "";

      while (true)
      {
        Console.Write("$> ");

        buffer += Console.ReadLine();

        char lastCharacter = buffer[buffer.Length - 1];
        if (lastCharacter == '{')
          ++openBrackets;
        else if (lastCharacter == '}' && openBrackets > 0)
          --openBrackets;

        if (openBrackets == 0)
        {
          var tokens = new Tokenizer(buffer).Tokenize();

          if (debug)
            DebugTokens(tokens);

          var parser = new Parser.Parser(tokens);

          var parserResult = parser.Run();
          if (parserResult.Ok)
          {
            resolver.Resolve(parserResult.Statements);
            interpreter.Run(parserResult.Statements);
          }

          buffer = "";
        }
      }
    }
  }
}