using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace cslox
{
    public class Environment
    {
        public readonly Environment? enclosing;
        public readonly Dictionary<string, object?> values = new();

        public Environment()
        {
            enclosing = null;
        }

        public Environment(Environment? enclosing)
        {
            this.enclosing = enclosing;
        }


        public void Define(string name, object? value)
        {
            values[name] = value;
        }

        public object Get(Token name)
        {
            if (values.ContainsKey(name.lexeme))
            {
                return values[name.lexeme];
            }

            if (enclosing is not null)
            {
                return enclosing.Get(name);
            }

            throw new RuntimeError(name, "Undefined variable '" + name.lexeme + "'.");
        }

        public void Assign(Token name, object value)
        {
            if (values.ContainsKey(name.lexeme))
            {
                values[name.lexeme] = value;
                return;
            }

            if (enclosing is not null)
            {
                enclosing.Assign(name, value);
                return;
            }

            throw new RuntimeError(name, "Undefined variable '" + name.lexeme + "'.");
        }

        internal object GetAt(int distance, string name)
        {
            return Ancestor(distance).values[name];
        }

        private Environment Ancestor(int distance)
        {
            Environment environment = this;
            for (int i = 0; i < distance; i++)
            {
                environment = environment.enclosing;
            }
            return environment;
        }

        internal void AssignAt(int distance, Token name, object value)
        {
            Ancestor(distance).values[name.lexeme] = value;
        }
    }
}
