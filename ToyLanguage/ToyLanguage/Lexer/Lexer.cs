
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

            makeKeywordsDictionary();
            makeSingleCharacterTokensDictionary();
        }

        private void makeKeywordsDictionary()
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

        private void makeSingleCharacterTokensDictionary()
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

        private char nextToken() => source[current++];

        private bool endOfSource() => current >= source.Length;

        private bool isDigit(char c) => c >= '0' && c <= '9';

        private bool isAlpha(char c)
        {
            return c >= 'a' && c <= 'z' ||
                   c >= 'A' && c <= 'Z' ||
                   c == '_';
        }

        private char peek(int offset = 0) => endOfSource() ? '\0' : source[current + offset];

        private bool match(char expected)
        {
            if (endOfSource() || source[current] != expected)
                return false;

            ++current;
            return true;
        }

        private void addToken(TokenType type, object literal = null)
        {
            string expression = source.Substring(start, current);
            tokens.Add(new Token(type, expression, literal, line));
        }

        private void stringLiteral()
        {
            while (peek() != '"' && !endOfSource())
            {
                if (peek() == '\n')
                    ++line;
                nextToken();
            }

            if (endOfSource())
            {
                Console.WriteLine("Unterminated string on line {0}", line);
                return;
            }

            nextToken();

            string expression = source.Substring(start, current);
            string value = source.Substring(start + 1, current - 1);
            tokens.Add(new Token(TokenType.STRING, expression, value, line));
        }

        private void numberLiteral()
        {
            while (isDigit(peek()))
                nextToken();

            if (peek() == '.' && isDigit(peek(1)))
            {
                nextToken();

                while (isDigit(peek()))
                    nextToken();
            }

            string value = source.Substring(start, current);
            addToken(TokenType.NUMBER, value);
        }

        private void identifier()
        {
            while (isAlpha(peek()))
                nextToken();

            string keyword = source.Substring(start, current);
            TokenType type = keywords.GetValueOrDefault(keyword, TokenType.IDENTIFIER);
            addToken(type);
        }

        private void scanToken(char c)
        {
            if (singleCharacterTokens.ContainsKey(c))
            {
                addToken(singleCharacterTokens.GetValueOrDefault(c));
                return;
            }

            switch (c)
            {
                case '!':
                    if (match('!'))
                        addToken(TokenType.DOUBLE_BANG);
                    else if (match('='))
                        addToken(TokenType.BANG_EQUAL);
                    else
                        addToken(TokenType.BANG);
                    break;

                case '=':
                    addToken(match('=') ? TokenType.EQUAL_EQUAL : TokenType.EQUAL);
                    break;

                case '<':
                    addToken(match('=') ? TokenType.LESS_EQUAL : TokenType.LESS);
                    break;

                case '>':
                    addToken(match('=') ? TokenType.GREATER_EQUAL : TokenType.GREATER);
                    break;

                case '?':
                    if (match('?'))
                        addToken(TokenType.NULL_COALESCING);
                    else
                        Console.WriteLine("Unexpected character <{0}> on line <{1}>", c, line);

                    break;

                case '/':
                    if (match('/'))
                        while (peek() != '\n' && !endOfSource())
                            nextToken();
                    else if (match('*'))
                    {
                        int blockCommentStart = line;

                        while (
                          peek() != '*' &&
                          peek(1) != '/' &&
                          !endOfSource()
                        )
                            if (nextToken() == '\n')
                                ++line;

                        if (endOfSource())
                            Console.WriteLine("Unterminated block comment starting at line {0}", blockCommentStart);

                        nextToken(); // consume *
                        nextToken(); // consume /
                    }
                    else
                        addToken(TokenType.SLASH);
                    break;

                case ' ':
                case '\r':
                case '\t':
                    break;

                case '\n':
                    ++line;
                    break;

                case '"':
                    stringLiteral();
                    break;

                default:
                    if (isDigit(c))
                        numberLiteral();
                    else if (isAlpha(c))
                        identifier();
                    else
                        Console.WriteLine("Unexpected character <{0}> on line <{1}>", c, line);
                    break;
            }
        }

        public List<Token> Tokenize()
        {
            while (!endOfSource())
            {
                start = current;
                scanToken(nextToken());
            }

            tokens.Add(new Token(TokenType.END_OF_FILE, "", null, line));
            return tokens;
        }
    }
}