using System.IO.Pipes;
using System.Linq.Expressions;
using System.Text.RegularExpressions;

namespace cslox
{
    public class Parser
    {
        public class ParseError : Exception { }

        readonly List<Token> tokens;
        int current = 0;
        int loopDepth = 0;
        public Parser(List<Token> tokens)
        {
            this.tokens = tokens;
        }

        public List<Stmt> Parse()
        {
            List<Stmt> statements = new List<Stmt>();
            while (!IsAtEnd())
            {
                statements.Add(Declaration());
            }
            return statements;
        }

        Stmt Declaration()
        {
            try
            {
                if (Match(TokenType.VAR)) return VarDeclaration();

                return Statement();
            }
            catch (ParseError error)
            {
                Synchronize();
                return null;
            }
        }

        Stmt VarDeclaration()
        {
            Token name = Consume(TokenType.IDENTIFIER, "Expect variable name.");

            Expr? initiallizer = null;

            if (Match(TokenType.EQUAL)) initiallizer = Expression();

            Consume(TokenType.SEMICOLON, "Expect ';' after variable declaration.");
            return new Var(name, initiallizer);
        }

        Stmt Statement()
        {
            if (Match(TokenType.IF))
            {
                return IfStatment();
            }
            if (Match(TokenType.PRINT))
            {
                return PrintStatement();
            }
            if (Match(TokenType.LEFT_BRACE))
            {
                return new Block(BlockStmts());
            }
            if (Match(TokenType.WHILE))
            {
                return WhileStatement();
            }
            if (Match(TokenType.FOR))
            {
                return ForStatement();
            }
            if (Match(TokenType.BREAK))
            {
                return BreakStatement();
            }

            return ExpressionStatement();
        }

        Stmt BreakStatement()
        {
            if (loopDepth == 0) 
            {
                Error(Previous(), "Must be inside a loop to use 'break'.");
            }
            Consume(TokenType.SEMICOLON, "Expect ';' after 'break'.");
            return new Break();
        }

        Stmt ForStatement()
        {
            Consume(TokenType.LEFT_PAREN, "Expect '(' after 'for'.");

            Stmt? initializer;
            if (Match(TokenType.SEMICOLON))
            {
                initializer = null;
            }
            else if (Match(TokenType.VAR))
            {
                initializer = VarDeclaration();
            }
            else
            {
                initializer = ExpressionStatement();
            }

            Expr? condition = null;
            if (!Check(TokenType.SEMICOLON))
            {
                condition = Expression();
            }
            Consume(TokenType.SEMICOLON, "Expect ';' after loop condition.");

            Expr? increment = null;
            if (!Check(TokenType.RIGHT_PAREN))
            {
                increment = Expression();
            }
            Consume(TokenType.RIGHT_PAREN, "Expect ')' after for clauses.");

            try
            {
                loopDepth++;

                Stmt body = Statement();

                if (increment is not null)
                {
                    body = new Block(new List<Stmt> {
                        body,
                        new Expression(increment)
                    });
                }

                if (condition is null) condition = new Literal(true);

                body = new While(condition, body);

                if (initializer is not null)
                {
                    body = new Block(new List<Stmt>{
                        initializer,
                        body
                    });
                }

                return body;
            }
            finally
            {
                loopDepth--;
            }

        }

        Stmt WhileStatement()
        {
            Consume(TokenType.LEFT_PAREN, "Expect '(' after 'while'.");
            Expr condition = Expression();
            Consume(TokenType.RIGHT_PAREN, "Expect ')' after condition.");
            try
            {
                loopDepth++;
                Stmt body = Statement();
                return new While(condition, body);
            }
            finally
            {
                loopDepth--;
            }

        }

        Stmt IfStatment()
        {
            Consume(TokenType.LEFT_PAREN, "Expect '(' after 'if'.");
            Expr condition = Expression();
            Consume(TokenType.RIGHT_PAREN, "Expect ')' after if condition.");

            Stmt thenBranch = Statement();
            Stmt? elseBranch = null;
            if (Match(TokenType.ELSE))
            {
                elseBranch = Statement();
            }

            return new If(condition, thenBranch, elseBranch);
        }

        Stmt ExpressionStatement()
        {
            Expr expr = Expression();
            Consume(TokenType.SEMICOLON, "Expect ';' after expression.");
            return new Expression(expr);
        }

        Stmt PrintStatement()
        {
            Expr value = Expression();
            Consume(TokenType.SEMICOLON, "Expect ';' after value.");
            return new Print(value);
        }

        Expr Expression()
        {
            return Assignment();
        }

        Expr Assignment()
        {
            Expr expr = Or();

            if (Match(TokenType.EQUAL))
            {
                Token equals = Previous();
                Expr value = Assignment();

                if (expr is Variable)
                {
                    Token name = ((Variable)expr).name;

                    return new Assign(name, value);
                }

                Error(equals, "Invalid assignment target.");
            }

            return expr;
        }

        Expr Or()
        {
            Expr expr = And();

            while (Match(TokenType.OR))
            {
                Token @operator = Previous();
                Expr right = And();
                expr = new Logical(expr, @operator, right);
            }

            return expr;
        }

        Expr And()
        {
            Expr expr = Equality();

            while (Match(TokenType.AND))
            {
                Token @operator = Previous();
                Expr right = Equality();
                expr = new Logical(expr, @operator, right);
            }

            return expr;
        }

        Expr Equality()
        {
            Expr expr = Comparison();
            while (Match(TokenType.BANG_EQUAL, TokenType.EQUAL_EQUAL))
            {
                Token @operator = Previous();
                Expr right = Comparison();
                expr = new Binary(expr, @operator, right);
            }

            return expr;
        }

        Expr Comparison()
        {
            Expr expr = Term();

            while (Match(TokenType.GREATER, TokenType.GREATER_EQUAL, TokenType.LESS, TokenType.LESS_EQUAL))
            {
                Token @operator = Previous();
                Expr right = Term();
                expr = new Binary(expr, @operator, right);
            }

            return expr;
        }

        Expr Term()
        {
            Expr expr = Factor();
            while (Match(TokenType.MINUS, TokenType.PLUS))
            {
                Token @operator = Previous();
                Expr right = Factor();
                expr = new Binary(expr, @operator, right);
            }

            return expr;
        }

        Expr Factor()
        {
            Expr expr = @Unary();
            while (Match(TokenType.SLASH, TokenType.STAR))
            {
                Token @operator = Previous();
                Expr right = @Unary();
                expr = new Binary(expr, @operator, right);
            }

            return expr;
        }

        Expr @Unary()
        {
            if (Match(TokenType.BANG, TokenType.MINUS))
            {
                Token @opeartor = Previous();
                Expr right = @Unary();
                return new Unary(@opeartor, right);
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

            if (Match(TokenType.IDENTIFIER))
            {
                return new Variable(Previous());
            }


            throw Error(Peek(), "Expect expression.");
        }

        List<Stmt> BlockStmts()
        {
            List<Stmt> statements = new();
            while (!Check(TokenType.RIGHT_BRACE) && !IsAtEnd())
            {
                statements.Add(Declaration());
            }

            Consume(TokenType.RIGHT_BRACE, "Expect '}' after block.");
            return statements;
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

        Token Consume(TokenType type, string message)
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