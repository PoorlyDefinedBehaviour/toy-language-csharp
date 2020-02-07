namespace ToyLanguage.Lexer
{
  internal enum TokenType
  {
    LEFT_PAREN,
    RIGHT_PAREN,
    LEFT_BRACE,
    RIGHT_BRACE,
    COMMA,
    DOT,
    MINUS,
    PLUS,
    MOD,
    SLASH,
    STAR,
    NULL_COALESCING,
    SEMI_COLON,

    // One or two character tokens.
    BANG,

    DOUBLE_BANG,
    BANG_EQUAL,
    EQUAL,
    EQUAL_EQUAL,
    GREATER,
    GREATER_EQUAL,
    LESS,
    LESS_EQUAL,

    // Literals.
    IDENTIFIER,

    STRING,
    NUMBER,

    // Keywords.
    STATIC,

    AND,
    CLASS,
    CLASS_EXTENDS,
    ELSE,
    FALSE,
    FUNCTION,
    FOR,
    IF,
    NULL,
    OR,
    PRINT,
    RETURN,
    SUPER,
    THIS,
    TRUE,
    LET,
    CONST,
    WHILE,
    END_OF_FILE
  }

  internal class Token
  {
    public TokenType Type { get; }
    public string Lexeme { get; }
    public object Literal { get; }
    public int Line { get; }

    public Token(TokenType type, string lexeme, object literal, int line)
    {
      Type = type;
      Lexeme = lexeme;
      Literal = literal;
      Line = line;
    }
  }
}