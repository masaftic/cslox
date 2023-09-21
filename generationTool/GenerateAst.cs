using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

public class GenerateAst
{
    public static void Main(string[] args)
    {
        if (args.Length != 1)
        {
            Console.Error.WriteLine("Usage: generate_ast <output directory>");
            Environment.Exit(64);
        }
        string outputDir = args[0];
        //Console.Write(outputDir);
        DefineAst(outputDir, "Expr", new List<string>
        {
            "Assign   : Token name, Expr value",
            "Binary   : Expr left, Token @operator, Expr right",
            "Call     : Expr callee, Token paren, List<Expr> arguments",
            "Get      : Expr @object, Token name",
            "Set      : Expr @object, Token name, Expr value",
            "Super    : Token keyword, Token method",
            "This     : Token keyword",
            "Grouping : Expr expression",
            "Literal  : object? value",
            "Logical  : Expr left, Token @operator, Expr right",
            "Unary    : Token @operator, Expr right",
            "Variable : Token name"
        });

        DefineAst(outputDir, "Stmt", new List<string>
        {
            "Block      : List<Stmt> statements",
            "Break      : ",
            "Class      : Token name, Variable? superclass, List<Function> methods",
            "Expression : Expr expression",
            "If         : Expr condition, Stmt thenBranch, Stmt? elseBranch",
            "Function   : Token name, List<Token> parameters, List<Stmt> body",
            "Return     : Token keyword, Expr? value",
            "Print      : Expr expression",
            "Var        : Token name, Expr? initializer",
            "While      : Expr condition, Stmt body"
        });
    }

    static void DefineAst(string outputDir, string baseName, List<string> types)
    {
        string path = Path.Combine(outputDir, baseName + ".cs");
        using (StreamWriter writer = new StreamWriter(path))
        {
            // Write your AST generation code here.
            // This is just a placeholder for demonstration purposes.
            writer.WriteLine("using System;");
            writer.WriteLine("namespace cslox");
            writer.WriteLine("{");
            writer.WriteLine("        public abstract class " + baseName);
            writer.WriteLine("        {");
            writer.WriteLine("            public abstract R Accept<R>(IVisitor<R> visitor);");
            DefineVisitor(writer, baseName, types);

            writer.WriteLine("    }");
            foreach (string type in types)
            {
                string className = type.Split(':')[0].Trim();
                string fields = type.Split(':')[1].Trim();
                DefineType(writer, baseName, className, fields);
            }
            writer.WriteLine("}");
        }
    }

    //        "Binary : Expr Left, Token Operator, Expr Right",


    static void DefineVisitor(StreamWriter writer, string baseName, List<string> types)
    {
        writer.WriteLine("    public interface IVisitor<R>");
        writer.WriteLine("    {");
        foreach (string type in types)
        {
            string typeName = type.Split(':')[0].Trim();
            writer.WriteLine($"        R Visit{typeName}{baseName}({typeName} {char.ToLower(baseName[0]) + baseName.Substring(1)});");
        }
        writer.WriteLine("    }");
    }

    //        "Binary : Expr Left, Token Operator, Expr Right",

    static void DefineType(StreamWriter writer, string baseName, string className, string fieldList)
    {
        // Write your class definition here based on className and fieldList.
        // This is just a placeholder for demonstration purposes.
        writer.WriteLine("    public class " + className + " : " + baseName);
        writer.WriteLine("    {");
        string[] fields;
        if (fieldList.Length == 0)
        {
            fields = new string[0];
        }
        else
        {
            fields = fieldList.Split(',');
        }
        //Console.WriteLine(fieldList);


        // constructor
        writer.WriteLine("        public " + className + "(" + fieldList + ")");
        writer.WriteLine("        {");
        foreach (string field in fields)
        {
            string name = field.Trim().Split(' ')[1];
            writer.WriteLine("            this." + name + " = " + name + ";");
        }

        writer.WriteLine("        }");

        writer.WriteLine();

        writer.WriteLine($"        public override R Accept<R>(IVisitor<R> visitor)");
        writer.WriteLine("        {");
        writer.WriteLine($"            return visitor.Visit{className}{baseName}(this);");
        writer.WriteLine("        }");

        writer.WriteLine();
        writer.WriteLine();

        // fields
        foreach (string field in fields)
        {
            writer.WriteLine("        public readonly " + field.Trim() + ";");
        }
        writer.WriteLine("    }");
    }
}

