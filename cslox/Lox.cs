

using System.Linq.Expressions;
using System.Runtime.ConstrainedExecution;

namespace cslox
{
    class Lox
    {
        static bool hadError = false;
        static bool hadRuntimeError;
        static readonly Interpreter interpreter = new Interpreter();


        static void Main(string[] args)
        {
            if (args.Length > 1)
            {
                Console.WriteLine("Usage: cslox [script]");
                System.Environment.Exit(64);
            }
            else if (args.Length == 1)
            {
                RunFile(args[0]);
            }
            else
            {
                RunPrompt();
            }
            
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
                System.Environment.Exit(65);
            }
            if (hadRuntimeError)
            {
                System.Environment.Exit(70);
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

            Parser parser = new Parser(tokens);
            List<Stmt> statements = parser.Parse();

            if (hadError) return;
            if (hadRuntimeError) return;

            interpreter.Interpret(statements);
            
      
        }


        public static void Error(Token token, string message)
        {
            if (token.type == TokenType.EOF)
            {
                Report(token.line, " at end", message);
            }
            else
            {
                Report(token.line, " at '" + token.lexeme + "'", message);
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

        public static void RuntimeError(RuntimeError error)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            // Console.Error.WriteLine(error.Message + "\n[line " + error.token.line + "]");
            Console.Error.WriteLine(error.Message);
            Console.ResetColor();
            hadRuntimeError = true;
        }
    }
}