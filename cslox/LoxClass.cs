

namespace cslox
{
    public class LoxClass : ILoxCallable
    {
        public readonly string name;
        private Dictionary<string, LoxFunction> methods;

        public LoxClass(string name, Dictionary<string, LoxFunction> methods)
        {
            this.name = name;
            this.methods = methods;
        }


        public int Arity()
        {
            LoxFunction? initiallizer = FindMethod("init");
            if (initiallizer is null) return 0;

            return initiallizer.Arity();         
        }

        public object? Call(Interpreter interpreter, List<object> arguments)
        {
            LoxInstance instance = new LoxInstance(this);
            LoxFunction? initializer = FindMethod("init");

            if (initializer is not null)
            {
                ((LoxFunction)initializer.Bind(instance)).Call(interpreter, arguments);
            }
            
            return instance;
        }

        public override string ToString()
        {
            return name;
        }

        internal LoxFunction? FindMethod(string name)
        {
            if (methods.TryGetValue(name, out LoxFunction? fun))
            {
                return fun;
            }

            return null;
        }
    }
}