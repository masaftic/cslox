using System.Text;

namespace cslox
{
    class AstPrinter : Expr.IVisitor<string>
    {
        public string Print(Expr expr)
        {
            return expr.Accept(this);
        }

        public string VisitAssignExpr(Assign assign)
        {
            throw new NotImplementedException();
        }

        public string VisitBinaryExpr(Binary expr)
        {
            return Parenthesize(expr.@operator.lexeme, expr.left, expr.right);
        }

        public string VisitCallExpr(Call expr)
        {
            throw new NotImplementedException();
        }

        public string VisitGetExpr(Get expr)
        {
            throw new NotImplementedException();
        }

        public string VisitGroupingExpr(Grouping expr)
        {
            return Parenthesize("group", expr.expression);
        }

        public string VisitLiteralExpr(Literal expr)
        {
            if (expr.value is null) return "nil";
            return expr.value.ToString();
        }

        public string VisitLogicalExpr(Logical expr)
        {
            throw new NotImplementedException();
        }

        public string VisitSetExpr(Set expr)
        {
            throw new NotImplementedException();
        }

        public string VisitThisExpr(This expr)
        {
            throw new NotImplementedException();
        }

        public string VisitUnaryExpr(Unary expr)
        {
            return Parenthesize(expr.@operator.lexeme, expr.right);
        }

        public string VisitVariableExpr(Variable variable)
        {
            throw new NotImplementedException();
        }

        private string Parenthesize(string name, params Expr[] exprs)
        {
            var builder = new StringBuilder();
            builder.Append('(').Append(name);

            foreach (var expr in exprs)
            {
                builder.Append(' ');
                builder.Append(expr.Accept(this));
            }

            builder.Append(')');
            return builder.ToString();
        }
    }

}