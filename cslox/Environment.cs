﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace cslox
{
    class Environment
    {
        public readonly Environment? enclosing;
        readonly Dictionary<string, object> values = new();
        
        public Environment()
        {
            enclosing = null;
        }

        public Environment(Environment? enclosing) 
        { 
            this.enclosing = enclosing; 
        }


        public void Define(string name, object value)
        {
            values.Add(name, value);
        }

        public object Get(Token name)
        {                       
            if (values.ContainsKey(name.lexeme))
            {
                return values[name.lexeme];
            }

            if (enclosing != null)
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

            if (enclosing != null)
            {
                enclosing.Assign(name, value);
                return;
            }

            throw new RuntimeError(name, "Undefined variable '" + name.lexeme + "'.");
        }
    }
}
