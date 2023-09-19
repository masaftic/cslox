
using System.Resources;

namespace cslox
{
    public class LoxFunction : ILoxCallable
    {
        private readonly Function declaration;
        private readonly Environment closure;
        private readonly bool isInitializer;

        public LoxFunction(Function declaration, Environment closure, bool isInitializer)
        {
            this.isInitializer = isInitializer;
            this.closure = closure;
            this.declaration = declaration;
        }

        public int Arity()
        {
            return declaration.parameters.Count;
        }

        public object? Call(Interpreter interpreter, List<object> arguments)
        {
            Environment environment = new(closure);

            for (int i = 0; i < declaration.parameters.Count; i++)
            {
                environment.Define(declaration.parameters.ElementAt(i).lexeme, arguments.ElementAt(i));
            }

            try
            {
                interpreter.ExecuteBlock(declaration.body, environment);
            }
            catch (ReturnException returnValue)
            {
                if (isInitializer) return closure.GetAt(0, "this");
                return returnValue.value;
            }

            if (isInitializer) return closure.GetAt(0, "this");
            
            return null;
        }

        public override string ToString()
        {
            return "<fn " + declaration.name.lexeme + ">";
        }

        internal object Bind(LoxInstance instance)
        {
            Environment environment = new Environment(closure);
            environment.Define("this", instance);
            return new LoxFunction(declaration, environment, isInitializer);
        }
    }


}