using System;
namespace cslox
{
    public abstract class Expr
    {
        public abstract R Accept<R>(IVisitor<R> visitor);
        public interface IVisitor<R>
        {
            R VisitAssignExpr(Assign expr);
            R VisitBinaryExpr(Binary expr);
            R VisitCallExpr(Call expr);
            R VisitGetExpr(Get expr);
            R VisitSetExpr(Set expr);
            R VisitThisExpr(This expr);
            R VisitGroupingExpr(Grouping expr);
            R VisitLiteralExpr(Literal expr);
            R VisitLogicalExpr(Logical expr);
            R VisitUnaryExpr(Unary expr);
            R VisitVariableExpr(Variable expr);
        }
    }
    public class Assign : Expr
    {
        public Assign(Token name, Expr value)
        {
            this.name = name;
            this.value = value;
        }

        public override R Accept<R>(IVisitor<R> visitor)
        {
            return visitor.VisitAssignExpr(this);
        }


        public readonly Token name;
        public readonly Expr value;
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
    public class Call : Expr
    {
        public Call(Expr callee, Token paren, List<Expr> arguments)
        {
            this.callee = callee;
            this.paren = paren;
            this.arguments = arguments;
        }

        public override R Accept<R>(IVisitor<R> visitor)
        {
            return visitor.VisitCallExpr(this);
        }


        public readonly Expr callee;
        public readonly Token paren;
        public readonly List<Expr> arguments;
    }
    public class Get : Expr
    {
        public Get(Expr @object, Token name)
        {
            this.@object = @object;
            this.name = name;
        }

        public override R Accept<R>(IVisitor<R> visitor)
        {
            return visitor.VisitGetExpr(this);
        }


        public readonly Expr @object;
        public readonly Token name;
    }
    public class Set : Expr
    {
        public Set(Expr @object, Token name, Expr value)
        {
            this.@object = @object;
            this.name = name;
            this.value = value;
        }

        public override R Accept<R>(IVisitor<R> visitor)
        {
            return visitor.VisitSetExpr(this);
        }


        public readonly Expr @object;
        public readonly Token name;
        public readonly Expr value;
    }
    public class This : Expr
    {
        public This(Token keyword)
        {
            this.keyword = keyword;
        }

        public override R Accept<R>(IVisitor<R> visitor)
        {
            return visitor.VisitThisExpr(this);
        }


        public readonly Token keyword;
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


        public readonly object? value;
    }
    public class Logical : Expr
    {
        public Logical(Expr left, Token @operator, Expr right)
        {
            this.left = left;
            this.@operator = @operator;
            this.right = right;
        }

        public override R Accept<R>(IVisitor<R> visitor)
        {
            return visitor.VisitLogicalExpr(this);
        }


        public readonly Expr left;
        public readonly Token @operator;
        public readonly Expr right;
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
    public class Variable : Expr
    {
        public Variable(Token name)
        {
            this.name = name;
        }

        public override R Accept<R>(IVisitor<R> visitor)
        {
            return visitor.VisitVariableExpr(this);
        }


        public readonly Token name;
    }
}
