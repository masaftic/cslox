using System;
using System.Collections.Generic;
using System.Linq;
using System.Resources;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace cslox
{
    public partial class Interpreter : Expr.IVisitor<object>,
                        Stmt.IVisitor<object>
    {

        private class BreakException : Exception { }

        public readonly Environment globals = new();
        private Environment environment;

        public Interpreter()
        {
            globals.Define("clock", new NativeClock());

            
            environment = globals;
        }

        public void Interpret(List<Stmt> statements)
        {
            try
            {
                foreach (Stmt statement in statements)
                {
                    Execute(statement);
                }
            }
            catch (RuntimeError error)
            {
                Lox.RuntimeError(error);
            }
        }

        public void Execute(Stmt statement)
        {
            statement.Accept(this);
        }

        public void ExecuteBlock(List<Stmt> statements, Environment environment)
        {
            Environment previous = this.environment;

            try
            {
                this.environment = environment;
                foreach (Stmt statement in statements)
                {
                    Execute(statement);
                }
            }
            finally
            {
                this.environment = previous;
            }
        }

        public object Evaluate(Expr expr)
        {
            return expr.Accept(this);
        }

        public object VisitIfStmt(If stmt)
        {
            if (IsTruthy(Evaluate(stmt.condition)))
            {
                Execute(stmt.thenBranch);
            }
            else if (stmt.elseBranch != null)
            {
                Execute(stmt.elseBranch);
            }

            return null;
        }

        public object VisitLogicalExpr(Logical expr)
        {
            object left = Evaluate(expr.left);

            if (expr.@operator.type == TokenType.OR)
            {
                if (IsTruthy(left)) return left;
            }
            else 
            {
                if (!IsTruthy(left)) return left;
            }

            return Evaluate(expr.right);
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

        public object VisitBlockStmt(Block block)
        {
            ExecuteBlock(block.statements, new Environment(environment));
            return null;
        }

        public object VisitWhileStmt(While stmt)
        {
            try {
                while (IsTruthy(Evaluate(stmt.condition)))
                {
                    Execute(stmt.body);
                }
            }
            catch (BreakException)
            {

            }
            return null;
        }

        public object VisitBreakStmt(Break stmt)
        {
            throw new BreakException();
        }

        public object VisitReturnStmt(Return stmt)
        {
            object? value = null;
            if (stmt.value is not null) value = Evaluate(stmt.value);

            throw new ReturnException(value);
        }

        public object VisitFunctionStmt(Function stmt)
        {
            LoxFunction function = new(stmt);
            environment.Define(stmt.name.lexeme, function);

            return null;
        }

        public object VisitVarStmt(Var stmt)
        {
            object? value = null;
            if (stmt.initializer != null)
            {
                value = Evaluate(stmt.initializer);
            }

            environment.Define(stmt.name.lexeme, value);
            return null;
        }
        
        public object VisitVariableExpr(Variable expr)
        {
            return environment.Get(expr.name);
        }

        public object VisitAssignExpr(Assign expr)
        {
            object value = Evaluate(expr.value);
            environment.Assign(expr.name, value);
            return value;
        }
       
        public object VisitCallExpr(Call expr)
        {
            object callee = Evaluate(expr.callee);

            if (callee is not ILoxCallable) 
            {
                throw new RuntimeError(expr.paren, "Can only call functions and classes.");
            }

            List<object> arguments = new();

            foreach (Expr argument in expr.arguments)
            {
                arguments.Add(Evaluate(argument));
            }

            ILoxCallable function = (ILoxCallable)callee;

            if (arguments.Count != function.Arity())
            {
                throw new RuntimeError(expr.paren, "Expected " +
                    function.Arity() + " arguments but got " +
                    arguments.Count + ".");
            }

            return function.Call(this, arguments);
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

        private static bool IsTruthy(object obj)
        {
            if (obj is null) return false;
            if (obj.GetType() == typeof(bool)) return (bool)obj;
            return true;
        }

        private static bool IsEqual(object left, object right)
        {
            return Equals(left, right);
        }

        private static string Stringify(object value)
        {
            if (value is null) return "nil";

            if (value is double)
            {
                string? text = value.ToString();
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
