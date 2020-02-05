using System;
using System.Collections.Generic;

namespace ToyLanguage.Lexer
{
  internal class Tokenizer
  {
    private Dictionary<string, TokenType> Keywords = new Dictionary<string, TokenType>();
    private Dictionary<char, TokenType> SingleCharacterTokens { get; } = new Dictionary<char, TokenType>();

    private List<Token> Tokens { get; } = new List<Token>();

    private int Start = 0;
    private int Current = 0;
    private int Line = 0;

    private readonly string Source;

    public Tokenizer(string source)
    {
      Source = source;

      MakeKeywordsDictionary();
      MakeSingleCharacterTokensDictionary();
    }

    private void MakeKeywordsDictionary()
    {
      Keywords.Add("and", TokenType.AND);
      Keywords.Add("or", TokenType.OR);
      Keywords.Add("mod", TokenType.MOD);
      Keywords.Add("class", TokenType.CLASS);
      Keywords.Add("else", TokenType.ELSE);
      Keywords.Add("false", TokenType.FALSE);
      Keywords.Add("for", TokenType.FOR);
      Keywords.Add("function", TokenType.FUNCTION);
      Keywords.Add("if", TokenType.IF);
      Keywords.Add("null", TokenType.NULL);
      Keywords.Add("print", TokenType.PRINT);
      Keywords.Add("return", TokenType.RETURN);
      Keywords.Add("super", TokenType.SUPER);
      Keywords.Add("this", TokenType.THIS);
      Keywords.Add("true", TokenType.TRUE);
      Keywords.Add("let", TokenType.LET);
      Keywords.Add("const", TokenType.CONST);
      Keywords.Add("while", TokenType.WHILE);
      Keywords.Add("extends", TokenType.CLASS_EXTENDS);
      Keywords.Add("static", TokenType.STATIC);
    }

    private void MakeSingleCharacterTokensDictionary()
    {
      SingleCharacterTokens.Add('(', TokenType.LEFT_PAREN);
      SingleCharacterTokens.Add(')', TokenType.RIGHT_PAREN);
      SingleCharacterTokens.Add('{', TokenType.LEFT_BRACE);
      SingleCharacterTokens.Add('}', TokenType.RIGHT_BRACE);
      SingleCharacterTokens.Add(',', TokenType.COMMA);
      SingleCharacterTokens.Add(';', TokenType.SEMI_COLON);
      SingleCharacterTokens.Add('.', TokenType.DOT);
      SingleCharacterTokens.Add('-', TokenType.MINUS);
      SingleCharacterTokens.Add('+', TokenType.PLUS);
      SingleCharacterTokens.Add('*', TokenType.STATIC);
    }

    private char NextToken() => Source[Current++];

    private bool EndOfSource() => Current >= Source.Length;

    private bool IsDigit(char c) => c >= '0' && c <= '9';

    private bool IsAlpha(char c)
    {
      return c >= 'a' && c <= 'z' ||
             c >= 'A' && c <= 'Z' ||
             c == '_';
    }

    private char Peek(int offset = 0) => EndOfSource() ? '\0' : Source[Current + offset];

    private bool Match(char expected)
    {
      if (EndOfSource() || Source[Current] != expected)
        return false;

      ++Current;
      return true;
    }

    private void AddToken(TokenType type, object literal = null)
    {
      string expression = Source.Substring(Start, Current);
      Tokens.Add(new Token(type, expression, literal, Line));
    }

    private void StringLiteral()
    {
      while (Peek() != '"' && !EndOfSource())
      {
        if (Peek() == '\n')
          ++Line;
        NextToken();
      }

      if (EndOfSource())
      {
        Console.WriteLine("Unterminated string on Line {0}", Line);
        return;
      }

      NextToken();

      string expression = Source.Substring(Start, Current);
      string value = Source.Substring(Start + 1, Current - 1);
      Tokens.Add(new Token(TokenType.STRING, expression, value, Line));
    }

    private void numberLiteral()
    {
      while (IsDigit(Peek()))
        NextToken();

      if (Peek() == '.' && IsDigit(Peek(1)))
      {
        NextToken();

        while (IsDigit(Peek()))
          NextToken();
      }

      string value = Source.Substring(Start, Current);
      AddToken(TokenType.NUMBER, value);
    }

    private void Identifier()
    {
      while (IsAlpha(Peek()))
        NextToken();

      string keyword = Source.Substring(Start, Current);
      TokenType type = Keywords.GetValueOrDefault(keyword, TokenType.IDENTIFIER);
      AddToken(type);
    }

    private void ScanToken(char c)
    {
      if (SingleCharacterTokens.ContainsKey(c))
      {
        AddToken(SingleCharacterTokens.GetValueOrDefault(c));
        return;
      }

      switch (c)
      {
        case '!':
          if (Match('!'))
            AddToken(TokenType.DOUBLE_BANG);
          else if (Match('='))
            AddToken(TokenType.BANG_EQUAL);
          else
            AddToken(TokenType.BANG);
          break;

        case '=':
          AddToken(Match('=') ? TokenType.EQUAL_EQUAL : TokenType.EQUAL);
          break;

        case '<':
          AddToken(Match('=') ? TokenType.LESS_EQUAL : TokenType.LESS);
          break;

        case '>':
          AddToken(Match('=') ? TokenType.GREATER_EQUAL : TokenType.GREATER);
          break;

        case '?':
          if (Match('?'))
            AddToken(TokenType.NULL_COALESCING);
          else
            Console.WriteLine("Unexpected character <{0}> on Line <{1}>", c, Line);

          break;

        case '/':
          if (Match('/'))
            while (Peek() != '\n' && !EndOfSource())
              NextToken();
          else if (Match('*'))
          {
            int blockCommentStart = Line;

            while (
              Peek() != '*' &&
              Peek(1) != '/' &&
              !EndOfSource()
            )
              if (NextToken() == '\n')
                ++Line;

            if (EndOfSource())
              Console.WriteLine("Unterminated block comment Starting at Line {0}", blockCommentStart);

            NextToken(); // consume *
            NextToken(); // consume /
          }
          else
            AddToken(TokenType.SLASH);
          break;

        case ' ':
        case '\r':
        case '\t':
          break;

        case '\n':
          ++Line;
          break;

        case '"':
          StringLiteral();
          break;

        default:
          if (IsDigit(c))
            numberLiteral();
          else if (IsAlpha(c))
            Identifier();
          else
            Console.WriteLine("Unexpected character <{0}> on Line <{1}>", c, Line);
          break;
      }
    }

    public List<Token> Tokenize()
    {
      while (!EndOfSource())
      {
        Start = Current;
        ScanToken(NextToken());
      }

      Tokens.Add(new Token(TokenType.END_OF_FILE, "", null, Line));
      return Tokens;
    }
  }
}