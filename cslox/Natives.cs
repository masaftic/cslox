using System.Linq.Expressions;

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

        private class Input : ILoxCallable
        {
            public int Arity() => 0;

            public object? Call(Interpreter interpreter, List<object> arguments)
            {
                return new LoxInput();
            }

            private class LoxInput : LoxInstance
            {
                public LoxInput() : base(null)
                {
                }

                public override object Get(Token name)
                {
                    if (name.lexeme == "read")
                    {
                        return new ReadCallable();
                    }
                    else if (name.lexeme == "readNum")
                    {
                        return new ReadNumCallable(name);
                    }

                    throw new RuntimeError(name, $"Undefined property '{name.lexeme}'.");
                }

                public override void Set(Token name, object value)
                {
                    throw new RuntimeError(name, "Can't add properties to input.");
                }

                public override string ToString()
                {
                    return "<Instance of input object>";
                }


                private class ReadCallable : ILoxCallable
                {
                    public int Arity() => 0;

                    public object? Call(Interpreter interpreter, List<object> arguments)
                    {
                        string? line = Console.ReadLine();
                        while (line == "") line = Console.ReadLine();
                        return line;
                    }
                }

                private class ReadNumCallable : ILoxCallable
                {
                    private readonly Token name;

                    public ReadNumCallable(Token name)
                    {
                        this.name = name;
                    }
                    public int Arity() => 0;

                    public object? Call(Interpreter interpreter, List<object> arguments)
                    {
                        if (double.TryParse(Console.ReadLine(), out double number))
                        {
                            return number;
                        }
                        throw new RuntimeError(name, $"Cannot convert string to number '{name.lexeme}'.");
                    }
                }
            }
        }

        private class List : ILoxCallable
        {
            public int Arity()
            {
                return 1;
            }

            public object? Call(Interpreter interpreter, List<object> arguments)
            {
                int size = (int)(double)arguments.ElementAt(0);
                return new LoxList(size);
            }

            private class LoxList : LoxInstance
            {
                private readonly List<object> elements;
                public LoxList(int size) : base(null)
                {
                    elements = new List<object>(new object[size]);
                }

                public override object Get(Token name)
                {
                    switch (name.lexeme)
                    {
                        case "get": return new GetLoxCallable(elements, name);
                        case "set": return new SetLoxCallable(elements, name);
                        case "append": return new AppendLoxCallable(elements, name);
                        case "remove": return new RemoveLoxCallable(elements, name);
                        case "pop": return new PopLoxCallable(elements, name);
                        case "length": return (double)elements.Count;

                        default:
                            throw new RuntimeError(name, $"Undefined property '{name.lexeme}'.");
                    }
                }
                public override void Set(Token name, object value)
                {
                    throw new RuntimeError(name, "Can't add properties to arrays.");

                }

                public override string ToString()
                {
                    return '[' + string.Join(", ", elements) + ']';
                }



                private class GetLoxCallable : ILoxCallable
                {
                    private readonly List<object> elements;
                    private readonly Token name;
                    public GetLoxCallable(List<object> elements, Token name)
                    {
                        this.elements = elements;
                        this.name = name;
                    }
                    public int Arity() => 1;
                    public object? Call(Interpreter interpreter, List<object> arguments)
                    {
                        int index = (int)(double)arguments[0];
                        try
                        {
                            return elements[index];
                        }
                        catch (System.ArgumentOutOfRangeException)
                        {
                            throw new RuntimeError(name, $"Out of bounds access '{name.lexeme}'.");
                        }
                        catch (Exception)
                        {
                            throw new RuntimeError(name, $"Runtime Error '{name.lexeme}'.");
                        }
                    }
                }

                private class SetLoxCallable : ILoxCallable
                {
                    private readonly List<object> elements;
                    private readonly Token name;
                    public SetLoxCallable(List<object> elements, Token name)
                    {
                        this.elements = elements;
                        this.name = name;
                    }
                    public int Arity() => 2;
                    public object? Call(Interpreter interpreter, List<object> arguments)
                    {
                        int index = (int)(double)arguments[0];
                        try
                        {
                            object value = arguments[1];
                            return elements[index] = value;
                        }
                        catch (System.ArgumentOutOfRangeException)
                        {
                            throw new RuntimeError(name, $"Out of bounds access '{name.lexeme}'.");
                        }
                        catch (Exception)
                        {
                            throw new RuntimeError(name, $"Runtime Error '{name.lexeme}'.");
                        }
                    }
                }

                private class AppendLoxCallable : ILoxCallable
                {
                    private List<object> elements;


                    private readonly Token name; public AppendLoxCallable(List<object> elements, Token name)
                    {
                        this.elements = elements;
                        this.name = name;
                    }

                    public int Arity() => 1;

                    public object? Call(Interpreter interpreter, List<object> arguments)
                    {
                        int index = (int)(double)arguments[0];
                        try
                        {
                            object value = arguments[1];
                            elements.Add(value);
                        }
                        catch (Exception)
                        {
                            throw new RuntimeError(name, $"Runtime Error '{name.lexeme}'.");
                        }
                        return null;
                    }
                }

                private class RemoveLoxCallable : ILoxCallable
                {
                    private List<object> elements;
                    private readonly Token name;

                    public RemoveLoxCallable(List<object> elements, Token name)
                    {
                        this.elements = elements;
                        this.name = name;
                    }

                    public int Arity() => 1;

                    public object? Call(Interpreter interpreter, List<object> arguments)
                    {
                        try
                        {
                            elements.Remove(arguments[0]);
                        }
                        catch (Exception)
                        {
                            throw new RuntimeError(name, $"Runtime Error '{name.lexeme}'.");
                        }
                        return null;
                    }
                }

                private class PopLoxCallable : ILoxCallable
                {
                    private List<object> elements;
                    private readonly Token name;

                    public PopLoxCallable(List<object> elements, Token name)
                    {
                        this.elements = elements;
                        this.name = name;
                    }

                    public int Arity() => 1;

                    public object? Call(Interpreter interpreter, List<object> arguments)
                    {
                        int index = (int)(double)arguments[0];
                        try
                        {
                            elements.RemoveAt(index);
                        }
                        catch (System.ArgumentOutOfRangeException)
                        {
                            throw new RuntimeError(name, $"Out of bounds access '{name.lexeme}'.");
                        }
                        catch (Exception)
                        {
                            throw new RuntimeError(name, $"Runtime Error '{name.lexeme}'.");
                        }

                        return null;
                    }
                }
            }


        }

    }
}
