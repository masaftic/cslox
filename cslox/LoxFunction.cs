
using System.Resources;

namespace cslox
{
    public class LoxFunction : ILoxCallable
    {
        private readonly Function declaration;

        public LoxFunction(Function declaration)
        {
            this.declaration = declaration;
        }

        public int Arity()
        {
            return declaration.parameters.Count;
        }

        public object? Call(Interpreter interpreter, List<object> arguments)
        {
            Environment environment = new(interpreter.globals);

            for (int i = 0; i < declaration.parameters.Count; i++)
            {
                environment.Define(declaration.parameters.ElementAt(i).lexeme, arguments.ElementAt(i));
            }


            interpreter.ExecuteBlock(declaration.body, environment);
            return null;
        }

        public override string ToString()
        {
            return "<fn " + declaration.name.lexeme + ">";
        }
    }


}