using System;
namespace cslox
{
        public abstract class Stmt
        {
            public abstract R Accept<R>(IVisitor<R> visitor);
    public interface IVisitor<R>
    {
        R VisitBlockStmt(Block stmt);
        R VisitExpressionStmt(Expression stmt);
        R VisitIfStmt(If stmt);
        R VisitPrintStmt(Print stmt);
        R VisitVarStmt(Var stmt);
    }
    }
    public class Block : Stmt
    {
        public Block(List<Stmt> statements)
        {
            this.statements = statements;
        }

        public override R Accept<R>(IVisitor<R> visitor)
        {
            return visitor.VisitBlockStmt(this);
        }


        public readonly List<Stmt> statements;
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
    public class If : Stmt
    {
        public If(Expr condition, Stmt thenBranch, Stmt? elseBranch)
        {
            this.condition = condition;
            this.thenBranch = thenBranch;
            this.elseBranch = elseBranch;
        }

        public override R Accept<R>(IVisitor<R> visitor)
        {
            return visitor.VisitIfStmt(this);
        }


        public readonly Expr condition;
        public readonly Stmt thenBranch;
        public readonly Stmt? elseBranch;
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
