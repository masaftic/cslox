using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace cslox
{
    class Scanner
    {
        private readonly string source;
        private readonly List<Token> tokens = new List<Token>();
        private static readonly Dictionary<string, TokenType> keywords;
        private int start = 0;
        private int current = 0;
        private int line = 1;

        public Scanner(string source)
        {
            this.source = source;
        }

        static Scanner()
        {
            keywords = new Dictionary<string, TokenType>
            {
                { "and", TokenType.AND },
                { "class", TokenType.CLASS },
                { "else", TokenType.ELSE },
                { "false", TokenType.FALSE },
                { "for", TokenType.FOR },
                { "fun", TokenType.FUN },
                { "if", TokenType.IF },
                { "nil", TokenType.NIL },
                { "or", TokenType.OR },
                { "print", TokenType.PRINT },
                { "return", TokenType.RETURN },
                { "super", TokenType.SUPER },
                { "this", TokenType.THIS },
                { "true", TokenType.TRUE },
                { "var", TokenType.VAR },
                { "while", TokenType.WHILE }
            };
        }


        private bool IsAtEnd()
        {
            return current >= source.Length;
        }

        public List<Token> ScanTokens()
        {
            while (!IsAtEnd())
            {
                start = current;
                ScanToken();
            }

            tokens.Add(new Token(TokenType.EOF, "", null, line));
            return tokens;
        }

        private void ScanToken()
        {
            char c = Advance();
            switch (c)
            {
                case '(': AddToken(TokenType.LEFT_PAREN); break;
                case ')': AddToken(TokenType.RIGHT_PAREN); break;
                case '{': AddToken(TokenType.LEFT_BRACE); break;
                case '}': AddToken(TokenType.RIGHT_BRACE); break;
                case ',': AddToken(TokenType.COMMA); break;
                case '.': AddToken(TokenType.DOT); break;
                case '-': AddToken(TokenType.MINUS); break;
                case '+': AddToken(TokenType.PLUS); break;
                case ';': AddToken(TokenType.SEMICOLON); break;
                case '*': AddToken(TokenType.STAR); break;
                case '/':
                    if (Match('/'))
                    {
                        while (Peek() != '\n' && !IsAtEnd()) Advance();
                    }
                    else
                    {
                        AddToken(TokenType.SLASH); 
                    }
                    break;
                case ' ':
                case '\r':
                case '\t':
                    break;
                case '\n':
                    line++;
                    break;
                case '!':
                    AddToken(Match('=') ? TokenType.BANG_EQUAL : TokenType.BANG);
                    break;
                case '=':
                    AddToken(Match('=') ? TokenType.EQUAL_EQUAL : TokenType.EQUAL);
                    break;
                case '<':
                    AddToken(Match('=') ? TokenType.LESS_EQUAL : TokenType.LESS_EQUAL);
                    break;
                case '>':
                    AddToken(Match('=') ? TokenType.GREATER_EQUAL : TokenType.GREATER);
                    break;
                case '"':
                    StringToken();
                    break;

                default:
                    if (char.IsNumber(c))
                    {
                        NumberToken();
                    }
                    else if (IsAlpha(c))
                    {
                        IdentifierToken();
                    }
                    else
                    {
                        Lox.Error(line, "Unexpected character.");
                    }
                    break;
            }
        }

        private char Peek(int ahead = 0)
        {
            if (IsAtEnd()) return '\0';
            return source.ElementAt(current + ahead);
        }

        private char Advance()
        {
            return source.ElementAt(current++);
        }

        private void AddToken(TokenType type)
        {
            AddToken(type, null);
        }

        private void AddToken(TokenType type, object literal)
        {
            string text = source.Substring(start, current - start);
            tokens.Add(new Token(type, text, literal, line));
        }

        private bool Match(char expected)
        {
            if (IsAtEnd()) return false;

            if (source.ElementAt(current) != expected) return false;

            current++;
            return true;
        }

        private void StringToken()
        {
            while (Peek() != '"' && !IsAtEnd())
            {
                if (Peek() == '\n') 
                {
                    line++;
                }
                Advance();
            }

            if (IsAtEnd())
            {
                Lox.Error(line, "Unterminated string.");
                return;
            }
            // The closing "
            Advance();
            
            string value =  source.Substring(start + 1, current - start - 2);
            AddToken(TokenType.STRING, value);
        }

        private void NumberToken()
        {
            while (char.IsNumber(Peek())) Advance();

            if (Peek() == '.' && char.IsNumber(Peek(1)))
            {
                Advance();
                while (char.IsNumber(Peek())) Advance();
            }

            AddToken(TokenType.NUMBER, Double.Parse(source.Substring(start, current - start)));
        }
        private bool IsAlpha(char c)
        {
            return char.IsLetter(c) || c == '_';
        }

        private bool IsAlphaNumeric(char c)
        {
            return IsAlpha(c) || char.IsDigit(c);
        }

        private void IdentifierToken()
        {
            while (IsAlphaNumeric(Peek())) Advance();

            string text = source.Substring(start, current - start);

            TokenType type;
            bool isKeyword = keywords.TryGetValue(text, out type);
            if (!isKeyword)
            {
                type = TokenType.IDENTIFIER;
            }
            AddToken(type);
        }

    }

}
