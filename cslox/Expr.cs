using System;
namespace cslox
{
    public abstract class Expr
    {
        public abstract R Accept<R>(IVisitor<R> visitor);
    }
    public interface IVisitor<R>
    {
        R VisitBinaryExpr(Binary binary);
        R VisitGroupingExpr(Grouping grouping);
        R VisitLiteralExpr(Literal literal);
        R VisitUnaryExpr(Unary unary);
    }
    public class Binary : Expr
    {
        public Binary(Expr left, Token @operator, Expr right)
        {
            this.left = left;
            this.@operator = @operator;
            this.right = right;

        }

        public override R Accept<R>(IVisitor<R> visitor)
        {
            return visitor.VisitBinaryExpr(this);
        }


        public readonly Expr left;
        public readonly Token @operator;
        public readonly Expr right;
    }
    public class Grouping : Expr
    {
        public Grouping(Expr expression)
        {
            this.expression = expression;
        }

        public override R Accept<R>(IVisitor<R> visitor)
        {
            return visitor.VisitGroupingExpr(this);
        }


        public readonly Expr expression;
    }
    public class Literal : Expr
    {
        public Literal(object? value)
        {
            this.value = value;

        }

        public override R Accept<R>(IVisitor<R> visitor)
        {
            return visitor.VisitLiteralExpr(this);
        }


        public readonly object value;
    }
    public class Unary : Expr
    {
        public Unary(Token @operator, Expr right)
        {
            this.@operator = @operator;
            this.right = right;
        }

        public override R Accept<R>(IVisitor<R> visitor)
        {
            return visitor.VisitUnaryExpr(this);
        }


        public readonly Token @operator;
        public readonly Expr right;
    }
}
