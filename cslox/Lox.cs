

namespace cslox
{
    class Lox
    {
        static bool hadError = false;
        static void Main(string[] args)
        {
            if (args.Length > 1)
            {
                Console.WriteLine("Usage: cslox [script]");
                Environment.Exit(64);
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

            byte[] bytes = File.ReadAllBytes(path);
            Run(System.Text.Encoding.Default.GetString(bytes));
            if (hadError)
            {
                Environment.Exit(65);
            }
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
       
        private static void Run(String source)
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