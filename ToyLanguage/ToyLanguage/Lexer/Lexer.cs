using System;
using System.Collections.Generic;

namespace ToyLanguage.Lexer
{
    internal class Tokenizer
    {
        private Dictionary<string, TokenType> keywords = new Dictionary<string, TokenType>();
        private readonly Dictionary<char, TokenType> singleCharacterTokens = new Dictionary<char, TokenType>();

        private readonly List<Token> tokens = new List<Token>();
        private int start = 0;
        private int current = 0;
        private int line = 0;

        private readonly string source;

        public Tokenizer(string source)
        {
            this.source = source;

            MakeKeywordsDictionary();
            MakeSingleCharacterTokensDictionary();
        }

        private void MakeKeywordsDictionary()
        {
            keywords.Add("and", TokenType.AND);
            keywords.Add("or", TokenType.OR);
            keywords.Add("mod", TokenType.MOD);
            keywords.Add("class", TokenType.CLASS);
            keywords.Add("else", TokenType.ELSE);
            keywords.Add("false", TokenType.FALSE);
            keywords.Add("for", TokenType.FOR);
            keywords.Add("function", TokenType.FUNCTION);
            keywords.Add("if", TokenType.IF);
            keywords.Add("null", TokenType.NULL);
            keywords.Add("print", TokenType.PRINT);
            keywords.Add("return", TokenType.RETURN);
            keywords.Add("super", TokenType.SUPER);
            keywords.Add("this", TokenType.THIS);
            keywords.Add("true", TokenType.TRUE);
            keywords.Add("let", TokenType.LET);
            keywords.Add("const", TokenType.CONST);
            keywords.Add("while", TokenType.WHILE);
            keywords.Add("extends", TokenType.CLASS_EXTENDS);
            keywords.Add("static", TokenType.STATIC);
        }

        private void MakeSingleCharacterTokensDictionary()
        {
            singleCharacterTokens.Add('(', TokenType.LEFT_PAREN);
            singleCharacterTokens.Add(')', TokenType.RIGHT_PAREN);
            singleCharacterTokens.Add('{', TokenType.LEFT_BRACE);
            singleCharacterTokens.Add('}', TokenType.RIGHT_BRACE);
            singleCharacterTokens.Add(',', TokenType.COMMA);
            singleCharacterTokens.Add(';', TokenType.SEMI_COLON);
            singleCharacterTokens.Add('.', TokenType.DOT);
            singleCharacterTokens.Add('-', TokenType.MINUS);
            singleCharacterTokens.Add('+', TokenType.PLUS);
            singleCharacterTokens.Add('*', TokenType.STATIC);
        }

        private char NextToken() => source[current++];

        private bool EndOfSource() => current >= source.Length;

        private bool IsDigit(char c) => c >= '0' && c <= '9';

        private bool IsAlpha(char c)
        {
            return c >= 'a' && c <= 'z' ||
                   c >= 'A' && c <= 'Z' ||
                   c == '_';
        }

        private char Peek(int offset = 0) => EndOfSource() ? '\0' : source[current + offset];

        private bool Match(char expected)
        {
            if (EndOfSource() || source[current] != expected)
                return false;

            ++current;
            return true;
        }

        private void AddToken(TokenType type, object literal = null) =>
            tokens.Add(new Token(type, source.Substring(start, current), literal, line));

        private void StringLiteral()
        {
            while (!Peek().Equals('"') && !EndOfSource())
            {
                if (Peek() == '\n')
                    ++line;
                NextToken();
            }

            if (EndOfSource())
            {
                Console.WriteLine("Unterminated string on line {0}", line);
                return;
            }

            NextToken();

            string expression = source.Substring(start, current);
            string value = source.Substring(start + 1, current - 1);
            tokens.Add(new Token(TokenType.STRING, expression, value, line));
        }

        private void NumberLiteral()
        {
            while (IsDigit(Peek()))
                NextToken();

            if (Peek().Equals('.') && IsDigit(Peek(1)))
            {
                NextToken();

                while (IsDigit(Peek()))
                    NextToken();
            }

            string value = source.Substring(start, current);
            AddToken(TokenType.NUMBER, value);
        }

        private void Identifier()
        {
            while (IsAlpha(Peek()))
                NextToken();

            string keyword = source.Substring(start, current);
            TokenType type = keywords.GetValueOrDefault(keyword, TokenType.IDENTIFIER);
            AddToken(type);
        }

        private void ScanToken(char c)
        {
            if (singleCharacterTokens.ContainsKey(c))
            {
                AddToken(singleCharacterTokens.GetValueOrDefault(c));
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
                        Console.WriteLine("Unexpected character <{0}> on line <{1}>", c, line);

                    break;

                case '/':
                    if (Match('/'))
                        while (Peek() != '\n' && !EndOfSource())
                            NextToken();
                    else if (Match('*'))
                    {
                        int blockCommentStart = line;

                        while (
                          Peek() != '*' &&
                          Peek(1) != '/' &&
                          !EndOfSource()
                        )
                            if (NextToken() == '\n')
                                ++line;

                        if (EndOfSource())
                            Console.WriteLine("Unterminated block comment starting at line {0}", blockCommentStart);

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
                    ++line;
                    break;

                case '"':
                    StringLiteral();
                    break;

                default:
                    if (IsDigit(c))
                        NumberLiteral();
                    else if (IsAlpha(c))
                        Identifier();
                    else
                        Console.WriteLine("Unexpected character <{0}> on line <{1}>", c, line);
                    break;
            }
        }

        public List<Token> Tokenize()
        {
            while (!EndOfSource())
            {
                start = current;
                ScanToken(NextToken());
            }

            tokens.Add(new Token(TokenType.END_OF_FILE, string.Empty, null, line));
            return tokens;
        }
    }
}