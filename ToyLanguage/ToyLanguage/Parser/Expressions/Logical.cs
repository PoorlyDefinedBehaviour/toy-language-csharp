using System;

using ToyLanguage.Interfaces;
using ToyLanguage.Lexer;

namespace ToyLanguage.Parser.Expressions
{
    internal class Logical : IExpression
    {
        public IExpression Left { get; set; }
        public Token Operator { get; set; }
        public IExpression Right { get; set; }

        public T Accept<T>(IExpressionVisitor<T> visitor) => throw new NotImplementedException();
    }
}