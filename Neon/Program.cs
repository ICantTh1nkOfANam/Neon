using System;
using System.Linq;
using System.Collections.Generic;
using Neon.Analysis;

namespace Neon
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            bool showTree = false;
            while (true)
            {
                Console.Write("$ ");
                var line = Console.ReadLine();
                if (string.IsNullOrWhiteSpace(line))
                    return;

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
                var color = Console.ForegroundColor;
                if (showTree)
                {
                    Console.ForegroundColor = ConsoleColor.White;
                    PrettyPrint(syntaxTree.root);
                    Console.ForegroundColor = color;
                }

                if (syntaxTree.diagnostics.Any())
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    foreach(var diag in parser.Diagnostics)
                        Console.WriteLine(diag);
                    Console.ForegroundColor = color;
                }
                else
                {
                    var e = new Executor(syntaxTree);
                    var result = e.Eval();
                    Console.WriteLine(result); 
                }
            }
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