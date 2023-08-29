using System.IO.Pipes;
using System.Text.RegularExpressions;

namespace cslox
{
    public class Parser
    {
        public class ParseError : Exception { }

        readonly List<Token> tokens;
        int current = 0;

        public Parser(List<Token> tokens)
        {
            this.tokens = tokens;
        }

        public Expr Parse()
        {
            try
            {
                return Expression();
            }
            catch (ParseError error)
            {
                return null;
            }
        }

        Expr Expression()
        {
            return Equality();
        }

        Expr Equality()
        {
            Expr expr = Comparison();
            while (Match(TokenType.BANG_EQUAL, TokenType.EQUAL_EQUAL))
            {
                Token _operator = Previous();
                Expr right = Comparison();
                expr = new Binary(expr, _operator, right);
            }

            return expr;
        }

        Expr Comparison()
        {
            Expr expr = Term();

            while (Match(TokenType.GREATER, TokenType.GREATER_EQUAL, TokenType.LESS, TokenType.LESS_EQUAL))
            {
                Token _operator = Previous();
                Expr right = Term();
                expr = new Binary(expr, _operator, right);
            }

            return expr;
        }

        Expr Term()
        {
            Expr expr = Factor();
            while (Match(TokenType.MINUS, TokenType.PLUS))
            {
                Token _operator = Previous();
                Expr right = Factor();
                expr = new Binary(expr, _operator, right);
            }

            return expr;
        }

        Expr Factor()
        {
            Expr expr = Unary_();
            while (Match(TokenType.SLASH, TokenType.STAR))
            {
                Token _operator = Previous();
                Expr right = Unary_();
                expr = new Binary(expr, _operator, right);
            }

            return expr;
        }


        Expr Unary_()
        {
            if (Match(TokenType.BANG, TokenType.MINUS))
            {
                Token _opeartor = Previous();
                Expr right = Unary_();
                return new Unary(_opeartor, right);
            }

            return Primary();
        }

        Expr Primary()
        {
            if (Match(TokenType.FALSE)) return new Literal(false);
            if (Match(TokenType.TRUE)) return new Literal(true);
            if (Match(TokenType.NIL)) return new Literal(null);

            if (Match(TokenType.NUMBER, TokenType.STRING))
            {
                return new Literal(Previous().literal);
            }

            if (Match(TokenType.LEFT_PAREN))
            {
                Expr expr = Expression();
                Consume(TokenType.RIGHT_PAREN, "Expect ')' after expression.");
                return new Grouping(expr);
            }

            throw Error(Peek(), "Expect expression.");
        }

        bool Match(params TokenType[] types)
        {
            foreach (TokenType type in types)
            {
                if (Check(type))
                {
                    Advance();
                    return true;
                }
            }

            return false;
        }

        private Token Consume(TokenType type, string message)
        {
            if (Check(type)) return Advance();
            throw Error(Peek(), message);
        }

        static ParseError Error(Token token, string message)
        {
            Lox.Error(token, message);
            return new ParseError();
        }

        void Synchronize()
        {
            Advance();

            while (!IsAtEnd())
            {
                if (Previous().type == TokenType.SEMICOLON) return;

                switch (Peek().type)
                {
                    case TokenType.CLASS:
                    case TokenType.FUN:
                    case TokenType.VAR:
                    case TokenType.FOR:
                    case TokenType.IF:
                    case TokenType.WHILE:
                    case TokenType.PRINT:
                    case TokenType.RETURN:
                        return;
                }
                Advance();
            }
        }

        bool Check(TokenType type)
        {
            if (IsAtEnd()) return false;
            return Peek().type == type;
        }

        Token Advance()
        {
            if (!IsAtEnd()) current++;
            return Previous();
        }

        bool IsAtEnd()
        {
            return Peek().type == TokenType.EOF;
        }

        Token Peek()
        {
            return tokens.ElementAt(current);
        }

        Token Previous()
        {
            return tokens.ElementAt(current - 1);
        }

    }



}