using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace cslox
{
    [Serializable]
    public class RuntimeError : Exception
    {
        private Token @operator;

        public RuntimeError(Token @operator, string message)
        {
            this.@operator = @operator;
            Lox.Error(@operator, message);
        }

    }
}
