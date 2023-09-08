using System;
namespace cslox
{
    public abstract class Stmt
    {
        public abstract R Accept<R>(IVisitor<R> visitor);
        public interface IVisitor<R>
        {
            R VisitExpressionStmt(Expression expression);
            R VisitPrintStmt(Print print);
            R VisitVarStmt(Var var);
        }
    }
    public class Expression : Stmt
    {
        public Expression(Expr expression)
        {
            this.expression = expression;
        }

        public override R Accept<R>(IVisitor<R> visitor)
        {
            return visitor.VisitExpressionStmt(this);
        }


        public readonly Expr expression;
    }
    public class Print : Stmt
    {
        public Print(Expr expression)
        {
            this.expression = expression;
        }

        public override R Accept<R>(IVisitor<R> visitor)
        {
            return visitor.VisitPrintStmt(this);
        }


        public readonly Expr expression;
    }
    public class Var : Stmt
    {
        public Var(Token name, Expr? initializer)
        {
            this.name = name;
            this.initializer = initializer;
        }

        public override R Accept<R>(IVisitor<R> visitor)
        {
            return visitor.VisitVarStmt(this);
        }


        public readonly Token name;
        public readonly Expr? initializer;
    }
}
