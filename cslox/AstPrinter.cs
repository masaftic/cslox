using System.Text;

namespace cslox {
    class AstPrinter : Expr.IVisitor<string>
    {
        public string Print(Expr expr) {
            return expr.Accept(this);
        }

        public string VisitBinaryExpr(Binary expr)
        {
            return Parenthesize(expr.@operator.lexeme, expr.left, expr.right);
        }
        
        public string VisitGroupingExpr(Grouping expr)
        {
            return Parenthesize("group", expr.expression);
        }

        public string VisitLiteralExpr(Literal expr)
        {
            if (expr.value == null) return "nil";
            return expr.value.ToString();
        }

        public string VisitUnaryExpr(Unary expr)
        {
            return Parenthesize(expr.@operator.lexeme, expr.right);
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