using System;
using System.Linq;
using N.Analysis;

namespace N
{
    internal static class Program
    {
        private static void Main()
        {
            Console.ForegroundColor = ConsoleColor.Green;
            var showTree = false;
            while (true)
            {
                Console.Write("$ ");
                var line = Console.ReadLine();
                if (string.IsNullOrWhiteSpace(line))
                    break;

                if (line == "toggletree")
                {
                    showTree = !showTree;
                    Console.WriteLine(showTree ? "Showing parsed tree" : "Not showing parsed tree");
                    continue;
                }
                if (line == "cls")
                {
                    Console.Clear();
                    continue;
                }
                if (line == "exit")
                {
                    break;
                }
                
                var parser = new Parser(line);
                var syntaxTree = parser.Parse();
                if (showTree)
                {
                    Console.ForegroundColor = ConsoleColor.White;
                    PrettyPrint(syntaxTree.root);
                    Console.ResetColor();
                }

                if (syntaxTree.diagnostics.Any())
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    foreach(var diag in parser.Diagnostics)
                        Console.WriteLine(diag);
                    Console.ResetColor();
                }
                else
                {
                    var e = new Executor(syntaxTree);
                    var result = e.Eval();
                    Console.WriteLine(result); 
                }
            }
        return;
        }

        static void PrettyPrint(Node _node, string _indent = "")
        {
            Console.Write(_indent);
            Console.Write(_node.type);
            if (_node is Token token && token.value != null)
            {
                Console.Write(" ");
                Console.Write(token.value);
            }

            Console.Write('\n');

            _indent += "    ";
            foreach (var child in _node.GetChildren())
                PrettyPrint(child, _indent);
        }
    }
}