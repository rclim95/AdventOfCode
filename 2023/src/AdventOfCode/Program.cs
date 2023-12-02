using AdventOfCode.Problems;
using Spectre.Console.Cli;

namespace AdventOfCode
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            var app = new CommandApp<SolveCommand>();
            app.Run(args);
        }
    }
}
