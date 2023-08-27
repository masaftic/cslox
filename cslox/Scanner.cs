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


        private bool isAtEnd()
        {
            return current >= source.Length;
        }

        public List<Token> scanTokens()
        {
            while (!isAtEnd())
            {
                start = current;
                scanToken();
            }

            tokens.Add(new Token(TokenType.EOF, "", null, line));
            return tokens;
        }

        private void scanToken()
        {
            char c = advance();
            switch (c)
            {
                case '(': addToken(TokenType.LEFT_PAREN); break;
                case ')': addToken(TokenType.RIGHT_PAREN); break;
                case '{': addToken(TokenType.LEFT_BRACE); break;
                case '}': addToken(TokenType.RIGHT_BRACE); break;
                case ',': addToken(TokenType.COMMA); break;
                case '.': addToken(TokenType.DOT); break;
                case '-': addToken(TokenType.MINUS); break;
                case '+': addToken(TokenType.PLUS); break;
                case ';': addToken(TokenType.SEMICOLON); break;
                case '*': addToken(TokenType.STAR); break;
                case '/':
                    if (match('/'))
                    {
                        while (peek() != '\n' && !isAtEnd()) advance();
                    }
                    else
                    {
                        addToken(TokenType.SLASH); 
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
                    addToken(match('=') ? TokenType.BANG_EQUAL : TokenType.BANG);
                    break;
                case '=':
                    addToken(match('=') ? TokenType.EQUAL_EQUAL : TokenType.EQUAL);
                    break;
                case '<':
                    addToken(match('=') ? TokenType.LESS_EQUAL : TokenType.LESS_EQUAL);
                    break;
                case '>':
                    addToken(match('=') ? TokenType.GREATER_EQUAL : TokenType.GREATER);
                    break;
                case '"':
                    stringToken();
                    break;

                default:
                    if (char.IsNumber(c))
                    {
                        numberToken();
                    }
                    else if (isAlpha(c))
                    {
                        identifierToken();
                    }
                    else
                    {
                        Lox.error(line, "Unexpected character.");
                    }
                    break;
            }
        }

        private char peek(int ahead = 0)
        {
            if (isAtEnd()) return '\0';
            return source.ElementAt(current + ahead);
        }

        private char advance()
        {
            return source.ElementAt(current++);
        }

        private void addToken(TokenType type)
        {
            addToken(type, null);
        }

        private void addToken(TokenType type, object literal)
        {
            string text = source.Substring(start, current - start);
            tokens.Add(new Token(type, text, literal, line));
        }

        private bool match(char expected)
        {
            if (isAtEnd()) return false;

            if (source.ElementAt(current) != expected) return false;

            current++;
            return true;
        }

        private void stringToken()
        {
            while (peek() != '"' && !isAtEnd())
            {
                if (peek() == '\n') 
                {
                    line++;
                }
                advance();
            }

            if (isAtEnd() )
            {
                Lox.error(line, "Unterminated string.");
                return;
            }
            // The closing "
            advance();
            
            string value =  source.Substring(start + 1, current - start - 2);
            addToken(TokenType.STRING, value);
        }

        private void numberToken()
        {
            while (char.IsNumber(peek())) advance();

            if (peek() == '.' && char.IsNumber(peek(1)))
            {
                advance();
                while (char.IsNumber(peek())) advance();
            }

            addToken(TokenType.NUMBER, Double.Parse(source.Substring(start, current - start)));
        }
        private bool isAlpha(char c)
        {
            return char.IsLetter(c) || c == '_';
        }

        private bool isAlphaNumeric(char c)
        {
            return isAlpha(c) || char.IsDigit(c);
        }

        private void identifierToken()
        {
            while (isAlphaNumeric(peek())) advance();

            string text = source.Substring(start, current - start);

            TokenType type;
            bool isKeyword = keywords.TryGetValue(text, out type);
            if (!isKeyword)
            {
                type = TokenType.IDENTIFIER;
            }
            addToken(type);
        }

    }

}
