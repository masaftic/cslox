using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace cslox
{
    class Interpreter : Expr.IVisitor<object>,
                        Stmt.IVisitor<object>
    {

        Environment environment = new();


        public void Interpret(List<Stmt> statements)
        {
            try
            {
                foreach (Stmt statement in statements)
                {
                    Execute(statement);
                }
            } catch (RuntimeError error)
            {
                Lox.RuntimeError(error);
            }
        }

        void Execute(Stmt statement)
        {
            statement.Accept(this);
        }

        public object Evaluate(Expr expr)
        {
            return expr.Accept(this);
        }

        public object VisitExpressionStmt(Expression stmt)
        {
            Evaluate(stmt.expression); 
            return null;
        }

        public object VisitPrintStmt(Print stmt)
        {
            object value = Evaluate(stmt.expression);
            Console.WriteLine(Stringify(value));
            return null;
        }


        public object VisitVariableExpr(Variable expr)
        {
            return environment.Get(expr.name);
        }

        public object VisitVarStmt(Var stmt)
        {
            object value = null;
            if (stmt.initializer != null)
            {
                value = Evaluate(stmt.initializer);
            }

            environment.Define(stmt.name.lexeme, value);
            return null;
        }


        public object VisitBinaryExpr(Binary expr)
        {
            object left = Evaluate(expr.left);
            object right = Evaluate(expr.right);

            switch (expr.@operator.type)
            {
                case TokenType.MINUS:
                    CheckNumberOperands(expr.@operator, left, right);
                    return (double)left - (double)right;
                case TokenType.SLASH:
                    CheckNumberOperands(expr.@operator, left, right);
                    return (double)left / (double)right;
                case TokenType.STAR:
                    CheckNumberOperands(expr.@operator, left, right);
                    return (double)left * (double)right;

                case TokenType.PLUS:
                    if (left.GetType() == typeof(double) && right.GetType() == typeof(double))
                    {
                        return (double)left + (double)right;
                    }
                    if (left.GetType() == typeof(string) && right.GetType() == typeof(string))
                    {
                        return (string)left + (string)right;
                    }
                    throw new RuntimeError(expr.@operator, "Operands must be two numbers or two strings.");

                case TokenType.GREATER:
                    CheckNumberOperands(expr.@operator, left, right);
                    return (double)left > (double)right;
                case TokenType.GREATER_EQUAL:
                    CheckNumberOperands(expr.@operator, left, right);
                    return (double)left >= (double)right;
                case TokenType.LESS:
                    CheckNumberOperands(expr.@operator, left, right);
                    return (double)left < (double)right;
                case TokenType.LESS_EQUAL:
                    CheckNumberOperands(expr.@operator, left, right);
                    return (double)left <= (double)right;

                case TokenType.BANG_EQUAL:
                    return !IsEqual(left, right);
                case TokenType.EQUAL_EQUAL:
                    return IsEqual(left, right);
                default:
                    //unreachable
                    return null;
            }
        }
     
        public object VisitGroupingExpr(Grouping expr)
        {
            return Evaluate(expr.expression);
        }

        public object VisitLiteralExpr(Literal expr)
        {
            return expr.value;
        }

        public object VisitUnaryExpr(Unary expr)
        {
            object right = Evaluate(expr.right);
            switch (expr.@operator.type)
            {
                case TokenType.BANG:
                    return !IsTruthy(right);
                case TokenType.MINUS:
                    CheckNumberOperand(expr.@operator, right);
                    return -(double)right;

            }
            // unreachable
            return null;
        }



        private static void CheckNumberOperands(Token @operator, object left, object right)
        {
            if (left.GetType() == typeof(double) &&
                right.GetType() == typeof(double))
                return;
            throw new RuntimeError(@operator, "Operands must be a number");
        }

        private static void CheckNumberOperand(Token @operator, object operand)
        {
            if (operand.GetType() == typeof(double)) return;
            throw new RuntimeError(@operator, "Operand must be a number");
        }

        static bool IsTruthy(object obj)
        {
            if (obj == null) return false;
            if (obj.GetType() == typeof(bool)) return (bool)obj;
            return true;
        }

        private static bool IsEqual(object left, object right)
        {
            if (left == null || right == null) return true;
            if (left == null) return false;
            return Equals(left, right);
        }

        private static string Stringify(object value)
        {
            if (value == null) return "nil";

            if (value is double)
            {
                string text = value.ToString();
                if (text.EndsWith(".0"))
                {
                    text = text.Substring(0, text.Length - 2);
                }
                return text;
            }

            return value.ToString();
        }

        
    }
}
