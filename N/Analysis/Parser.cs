using System;
using System.Collections.Generic;
using System.Linq;

namespace N.Analysis
{
    internal sealed class Parser
        {
            private readonly Token[] m_Tokens;
            private int m_Position;
            private List<string> m_Diagnostics = new List<string>();
            public Parser(string _text)
            {
                var tokens = new List<Token>();
                var lexer = new Lexer(_text);
                Token token;
                do
                {
                    token = lexer.NextToken();
                    if (token.type != TokenType.WhiteSpace && token.type != TokenType.BadToken)
                    {
                        tokens.Add(token);
                    }
                    
                } while (token.type != TokenType.EndOfFile);

                m_Tokens = tokens.ToArray();
                m_Diagnostics.AddRange(lexer.Diagnostics);
            }
            public IEnumerable<string> Diagnostics => m_Diagnostics;
            private Token Peek(int _offset)
            {
                var index = m_Position + _offset;
                if (index >= m_Tokens.Length)
                    return m_Tokens[m_Tokens.Length - 1];
                return m_Tokens[index];
            }

            private Token Current => Peek(0);

            private Token NextToken()
            {
                var current = Current;
                m_Position++;
                return current;
            }

            private Token Match(TokenType _type)
            {
                if (Current.type == _type)
                    return NextToken();
                
                m_Diagnostics.Add($"Syntax Error: Unexpected Token {Current.type}, expected {_type}");
                return new Token(_type, Current.position, null, null);
            }

            public Expression ParseExpr() { return ParseTerm(); }
            public SyntaxTree Parse()
            {
                var expr = ParseTerm();
                var endOfFile = Match(TokenType.EndOfFile);
                return new SyntaxTree(m_Diagnostics, expr, endOfFile);
            }
            
            private Expression ParseTerm()
            {
                var left = ParseFactor();

                while (Current.type == TokenType.MathAdd ||
                    Current.type == TokenType.MathSubtract)
                {
                    var operatorToken = NextToken();
                    var right = ParseFactor();
                    left = new BinaryExpr(left, operatorToken, right);
                }
                return left;
            }

            private Expression ParseFactor()
            {
                var left = ParsePrimaryExpr();

                while (Current.type == TokenType.MathMultiply ||
                    Current.type == TokenType.MathDivide)
                {
                    var operatorToken = NextToken();
                    var right = ParsePrimaryExpr();
                    left = new BinaryExpr(left, operatorToken, right);
                }
                return left;
            }

            private Expression ParsePrimaryExpr()
            {
                if (Current.type == TokenType.OpenParenthesis)
                {
                    var left = NextToken();
                    var expression = ParseExpr();
                    var right = Match(TokenType.CloseParenthesis);
                    return new ParenthesisExpr(left, expression, right);
                }

                var numberToken = Match(TokenType.Number);
                return new NumberExpr(numberToken);
            }
        }
}