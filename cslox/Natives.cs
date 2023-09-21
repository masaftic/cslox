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

        private class Read : ILoxCallable
        {
            public int Arity()
            {
                return 0;
            }

            public object? Call(Interpreter interpreter, List<object> arguments)
            {
                string? input = Console.ReadLine();
                return input;
            }
        }

        private class ReadNum : ILoxCallable
        {
            public int Arity()
            {
                return 0;
            }

            public object? Call(Interpreter interpreter, List<object> arguments)
            {
                if (double.TryParse(Console.ReadLine(), out double number))
                {
                    return number;
                }

                throw new RuntimeError("Can't convert string to number");
            }
        }

    }
}
