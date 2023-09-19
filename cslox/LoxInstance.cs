
namespace cslox
{
    class LoxInstance
    {
        private LoxClass @class;
        private readonly Dictionary<string, object> fields = new();

        public LoxInstance(LoxClass @class)
        {
            this.@class = @class;
        }

        public object Get(Token name)
        {
            if (fields.ContainsKey(name.lexeme))
            {
                return fields[name.lexeme];
            }

            LoxFunction? method = @class.FindMethod(name.lexeme);
            if (method is not null) return method;
            
            throw new RuntimeError(name, "Undefined property '" + name.lexeme + "'.");
        }

        public override string ToString()
        {
            return @class.name + " instance";
        }

        public void Set(Token name, object value)
        {
            fields[name.lexeme] = value;
        }
    }

}