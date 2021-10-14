using System;
using System.Collections.Generic;
using System.Linq;

namespace N.Analysis
{
    internal sealed class Lexer
        {
            private readonly string m_Text;
            private int m_Position;
            private List<string> m_Diagnostics = new List<string>();

            public Lexer(string _text)
            {
                m_Text = _text;
            }

            public IEnumerable<string> Diagnostics => m_Diagnostics;
            private char Current
            {
                get {
                    if (m_Position >= m_Text.Length)
                        return '\0';
                    return m_Text[m_Position];
                }
            }

            private void Next()
            {
                m_Position++;
            }

            public Token NextToken()
            {
                if (m_Position >= m_Text.Length)
                    return new Token(TokenType.EndOfFile, m_Position++, "\0", null); 

                if (char.IsDigit(Current))
                {
                    var start = m_Position;

                    while (char.IsDigit(Current))
                        Next();
                    var length = m_Position - start;
                    var text = m_Text.Substring(start, length);
                    if (!int.TryParse(text, out var val))
                        m_Diagnostics.Add($"The number {m_Text} is not a valid Int32");
                    return new Token(TokenType.Number, start, text, val);
                }

                if (char.IsWhiteSpace(Current))
                {
                    var start = m_Position;

                    while (char.IsWhiteSpace(Current))
                        Next();
                    var length = m_Position - start;
                    var text = m_Text.Substring(start, length);
                    return new Token(TokenType.WhiteSpace, start, text, null);
                }
                switch(Current)
                {
                    case '+':
                        return new Token(TokenType.MathAdd, m_Position++, "+", null);
                    case '-':
                        return new Token(TokenType.MathSubtract, m_Position++, "-", null);
                    case '*':
                        return new Token(TokenType.MathMultiply, m_Position++, "*", null);
                    case '/':
                        return new Token(TokenType.MathDivide, m_Position++, "/", null);
                    case '(':
                        return new Token(TokenType.OpenParenthesis, m_Position++, "(", null);
                    case ')':
                        return new Token(TokenType.CloseParenthesis, m_Position++, ")", null);
                }

                m_Diagnostics.Add($"Syntax Error: Unexpected Token: '{Current}'");
                return new Token(TokenType.BadToken, m_Position++, m_Text.Substring(m_Position-1, 1), null);
            }
        }
}