using System.Resources;
using System.Runtime.InteropServices;

namespace cslox
{
    public class Resolver : Expr.IVisitor<object?>, Stmt.IVisitor<object?>
    {
        private readonly Interpreter interpreter;
        private readonly Stack<Dictionary<string, bool>> scopes = new();
        private FunctionType currentFunction = FunctionType.NONE;

        private ClassType currentClass = ClassType.NONE;

        public Resolver(Interpreter interpreter)
        {
            this.interpreter = interpreter;
        }


        private enum FunctionType
        {
            NONE,
            FUNCTION,
            INITIALIZER,
            METHOD
        }

        private enum ClassType
        {
            NONE,
            CLASS,
            SUBCLASS
        }

        public object? VisitBlockStmt(Block stmt)
        {
            BeginScope();
            Resolve(stmt.statements);
            EndScope();

            return null;
        }

        public object? VisitClassStmt(Class stmt)
        {
            ClassType enclosingClass = currentClass;
            currentClass = ClassType.CLASS;


            Declare(stmt.name);
            Define(stmt.name);

            if (stmt.superclass is not null &&
                stmt.name.lexeme == stmt.superclass.name.lexeme)
            {
                Lox.Error(stmt.superclass.name, "A class can't inherit from itself.");
            }

            if (stmt.superclass is not null)
            {
                currentClass = ClassType.SUBCLASS;
                Resolve(stmt.superclass);
            }
            
            if (stmt.superclass is not null)
            {
                BeginScope();
                scopes.Peek()["super"] = true;
            }
            

            BeginScope();
            scopes.Peek()["this"] = true;

            foreach (Function method in stmt.methods)
            {
                FunctionType declaration = FunctionType.METHOD;
                if (method.name.lexeme == "init")
                {
                    declaration = FunctionType.INITIALIZER;
                }
                ResolveFunction(method, declaration);
            }

            EndScope();

            if (stmt.superclass is not null)
            {
                EndScope();
            }

            currentClass = enclosingClass;
            return null;
        }

        public object? VisitFunctionStmt(Function stmt)
        {
            Declare(stmt.name);
            Define(stmt.name);

            ResolveFunction(stmt, FunctionType.FUNCTION);
            return null;
        }

        public object? VisitVarStmt(Var stmt)
        {
            Declare(stmt.name);
            if (stmt.initializer is not null) Resolve(stmt.initializer);
            Define(stmt.name);

            return null;
        }

        public object? VisitAssignExpr(Assign expr)
        {
            Resolve(expr.value);
            ResolveLocal(expr, expr.name);

            return null;
        }

        public object? VisitVariableExpr(Variable expr)
        {
            if (scopes.Count != 0 && scopes.Peek().TryGetValue(expr.name.lexeme, out bool found) && found == false)
            {
                Lox.Error(expr.name, "Can't read local variable in its own initializer.");
            }

            ResolveLocal(expr, expr.name);
            return null;
        }


        public void Resolve(List<Stmt> statements)
        {
            foreach (Stmt stmt in statements)
            {
                Resolve(stmt);
            }
        }

        private void Resolve(Stmt stmt) => stmt.Accept(this);

        private void Resolve(Expr expr) => expr.Accept(this);


        private void ResolveFunction(Function function, FunctionType type)
        {
            FunctionType enclosingFunction = currentFunction;
            currentFunction = type;

            BeginScope();

            foreach (Token param in function.parameters)
            {
                Declare(param);
                Define(param);
            }
            Resolve(function.body);

            EndScope();

            currentFunction = enclosingFunction;
        }


        private void BeginScope()
        {
            scopes.Push(new Dictionary<string, bool>());
        }

        private void EndScope()
        {
            scopes.Pop();
        }


        private void Declare(Token name)
        {
            if (scopes.Count == 0) return;

            var scope = scopes.Peek();

            if (scope.ContainsKey(name.lexeme))
            {
                Lox.Error(name, "Already a variable with this name in this scope.");
            }
            else
            {
                scope.Add(name.lexeme, false);
            }
        }

        private void Define(Token name)
        {
            if (scopes.Count == 0) return;
            scopes.Peek()[name.lexeme] = true;
        }

        private void ResolveLocal(Expr expr, Token name)
        {
            for (int i = 0; i < scopes.Count; i++)
            {
                if (scopes.ElementAt(i).ContainsKey(name.lexeme))
                {
                    interpreter.Resolve(expr, i);
                    return;
                }
            }
        }

        public object? VisitBinaryExpr(Binary expr)
        {
            Resolve(expr.left);
            Resolve(expr.right);
            return null;
        }

        public object? VisitBreakStmt(Break stmt)
        {
            return null;
        }

        public object? VisitCallExpr(Call expr)
        {
            Resolve(expr.callee);

            foreach (Expr argument in expr.arguments)
            {
                Resolve(argument);
            }

            return null;
        }

        public object? VisitGetExpr(Get expr)
        {
            Resolve(expr.@object);
            return null;
        }

        public object? VisitExpressionStmt(Expression stmt)
        {
            Resolve(stmt.expression);
            return null;
        }

        public object? VisitGroupingExpr(Grouping expr)
        {
            Resolve(expr.expression);
            return null;
        }

        public object? VisitIfStmt(If stmt)
        {
            Resolve(stmt.condition);
            Resolve(stmt.thenBranch);
            if (stmt.elseBranch is not null) Resolve(stmt.elseBranch);
            return null;
        }

        public object? VisitLiteralExpr(Literal expr)
        {
            return null;
        }

        public object? VisitLogicalExpr(Logical expr)
        {
            Resolve(expr.left);
            Resolve(expr.right);
            return null;
        }

        public object? VisitSetExpr(Set expr)
        {
            Resolve(expr.value);
            Resolve(expr.@object);
            return null;
        }

        public object? VisitSuperExpr(Super expr)
        {
            if (currentClass == ClassType.NONE)
            {
                Lox.Error(expr.keyword, "Can't use 'super' outside of a class.");
            }
            else if (currentClass != ClassType.SUBCLASS)
            {
                Lox.Error(expr.keyword, "Can't use 'super' in a class with no superclass.");
            }
            ResolveLocal(expr, expr.keyword);   
            return null;
        }

        public object? VisitThisExpr(This expr)
        {
            if (currentClass == ClassType.NONE)
            {
                Lox.Error(expr.keyword, "Can't use 'this' outside of a class.");
                return null;
            }

            ResolveLocal(expr, expr.keyword);
            return null;
        }

        public object? VisitPrintStmt(Print stmt)
        {
            Resolve(stmt.expression);
            return null;
        }

        public object? VisitReturnStmt(Return stmt)
        {
            if (currentFunction == FunctionType.NONE)
            {
                Lox.Error(stmt.keyword, "Can't return from top-level code.");
            }

            if (stmt.value is not null)
            {
                if (currentFunction == FunctionType.INITIALIZER)
                {
                    Lox.Error(stmt.keyword, "Can't return a value from an initializer.");
                }
                Resolve(stmt.value);
            }
            return null;
        }

        public object? VisitUnaryExpr(Unary expr)
        {
            Resolve(expr.right);
            return null;
        }

        public object? VisitWhileStmt(While stmt)
        {
            Resolve(stmt.condition);
            Resolve(stmt.body);
            return null;
        }

    }



}