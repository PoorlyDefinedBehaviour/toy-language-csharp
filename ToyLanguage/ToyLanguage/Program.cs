using System;

using ToyLanguage.Lexer;

internal class Program
{
    private static void Main()
    {
        var lexer = new Tokenizer("let a = 10");
        Console.WriteLine("hello world");
    }
}