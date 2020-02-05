namespace Lexer
{
  enum TokenType
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

  class Token
  {
    public readonly TokenType type;
    public readonly string lexeme;
    public readonly object literal;
    public readonly int line;

    public Token(TokenType type,
                 string lexeme,
                 object literal,
                 int line)
    {
      this.type = type;
      this.lexeme = lexeme;
      this.literal = literal;
      this.line = line;
    }
  }

}