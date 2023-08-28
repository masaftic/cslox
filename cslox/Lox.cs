

namespace cslox
{
    class Lox
    {
        static bool hadError = false;
        static void Main(string[] args)
        {
            // if (args.Length > 1)
            // {
            //     Console.WriteLine("Usage: cslox [script]");
            //     Environment.Exit(64);
            // }
            // else if (args.Length == 1)
            // {
            //     RunFile(args[0]);
            // }
            // else
            // {
            //     RunPrompt();
            // }
            Expr expression = new Binary(
                new Unary(
                    new Token(TokenType.MINUS, "-", null, 1),
                    new Literal(123)
                ),
                new Token(TokenType.STAR, "*", null, 1),
                new Grouping(
                    new Literal(45.67)
                )
            );

            Console.WriteLine(new AstPrinter().print(expression));
        }

        private static void RunFile(string path)
        {
            byte[]? bytes = null;
            try
            {
                bytes = File.ReadAllBytes(path);   
            }
            catch (FileNotFoundException) {
                Console.WriteLine("File not Found");
                hadError = true;
            }
            catch (IOException e)
            {
                Console.WriteLine("An error occurred while accessing the file: " + e.Message);
                hadError = true;
            }
            catch (Exception e)
            {
                Console.WriteLine("An unexpected error occurred: " + e.Message);
                hadError = true;
            }
            if (hadError)
            {
                Environment.Exit(65);
            }
            Run(System.Text.Encoding.Default.GetString(bytes));
        }

        private static void RunPrompt() 
        {
            using (StreamReader input = new StreamReader(Console.OpenStandardInput()))
            {
                for (;;)
                {
                    Console.Write("> ");
                    string? line = input.ReadLine();
                    if (line == null) break;
                    Run(line);
                    hadError = false;
                }
            }
        }
       
        private static void Run(string source)
        {
            Scanner scanner = new Scanner(source);
            List<Token> tokens = scanner.ScanTokens();

            foreach (Token token in tokens)
            {
                Console.WriteLine(token.ToString());
            }
        }

        public static void Error(int line, string message)
        {
            Report(line, "", message);
        }

        private static void Report(int line, string where, string message)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.Error.WriteLine("[line {0}] Error{1}: {2}", line, where, message);
            Console.ResetColor();
            hadError = true;
        }
    }
}