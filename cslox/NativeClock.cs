namespace cslox
{
    public partial class Interpreter
    {
        private class NativeClock : ILoxCallable
        {
            public int Arity()
            {
                return 0;
            }

            public object Call(Interpreter interpreter, List<object> arguments)
            {
                return DateTimeOffset.UtcNow.ToUnixTimeMilliseconds() / 1000.0;
            }
            public override string ToString() { return "<native fn>"; }
        }
        
    }
}
