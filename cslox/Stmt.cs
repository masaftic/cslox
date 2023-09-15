using System;
namespace cslox
{
    public abstract class Stmt
    {
        public abstract R Accept<R>(IVisitor<R> visitor);
        public interface IVisitor<R>
        {
            R VisitBlockStmt(Block stmt);
            R VisitBreakStmt(Break stmt);
            R VisitExpressionStmt(Expression stmt);
            R VisitIfStmt(If stmt);
            R VisitFunctionStmt(Function stmt);
            R VisitReturnStmt(Return stmt);
            R VisitPrintStmt(Print stmt);
            R VisitVarStmt(Var stmt);
            R VisitWhileStmt(While stmt);
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
    public class Break : Stmt
    {
        public Break()
        {
        }

        public override R Accept<R>(IVisitor<R> visitor)
        {
            return visitor.VisitBreakStmt(this);
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
    public class Function : Stmt
    {
        public Function(Token name, List<Token> parameters, List<Stmt> body)
        {
            this.name = name;
            this.parameters = parameters;
            this.body = body;
        }

        public override R Accept<R>(IVisitor<R> visitor)
        {
            return visitor.VisitFunctionStmt(this);
        }


        public readonly Token name;
        public readonly List<Token> parameters;
        public readonly List<Stmt> body;
    }
    public class Return : Stmt
    {
        public Return(Token keyword, Expr? value)
        {
            this.keyword = keyword;
            this.value = value;
        }

        public override R Accept<R>(IVisitor<R> visitor)
        {
            return visitor.VisitReturnStmt(this);
        }


        public readonly Token keyword;
        public readonly Expr? value;
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
    public class While : Stmt
    {
        public While(Expr condition, Stmt body)
        {
            this.condition = condition;
            this.body = body;
        }

        public override R Accept<R>(IVisitor<R> visitor)
        {
            return visitor.VisitWhileStmt(this);
        }


        public readonly Expr condition;
        public readonly Stmt body;
    }
}
