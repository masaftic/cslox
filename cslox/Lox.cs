

namespace cslox
{
    class Lox
    {
        static bool hadError = false;
        static void Main(string[] args)
        {
            if (args.Length > 1)
            {
                Console.WriteLine("Usage: jlox [script]");
                Environment.Exit(64);
            }
            else if (args.Length == 1)
            {
                runFile(args[0]);
            }
            else
            {
                runPrompt();
            }
        }

        private static void runFile(string path)
        {
            byte[] bytes = File.ReadAllBytes(path);
            run(System.Text.Encoding.Default.GetString(bytes));
            if (hadError)
            {
                Environment.Exit(65);
            }
        }

        private static void runPrompt() 
        {
            using (StreamReader input = new StreamReader(Console.OpenStandardInput()))
            {
                for (;;)
                {
                    Console.Write("> ");
                    string? line = input.ReadLine();
                    if (line == null) break;
                    run(line);
                    hadError = false;
                }
            }
        }
       
        private static void run(String source)
        {
            Scanner scanner = new Scanner(source);
            List<Token> tokens = scanner.Scantokens();

            foreach (Token token in tokens)
            {
                Console.WriteLine(token);
            }
        }

        static void error(int line, string message)
        {
            report(line, "", message);
        }

        private static void report(int line, string where, string message)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.Error.Write("[line {0}] Error{1}: {2}", line, where, message);
            hadError = true;
        }
    }
}