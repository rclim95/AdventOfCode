using AdventOfCode.Problems;
using Spectre.Console.Cli;

namespace AdventOfCode
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            var app = new CommandApp<SolveCommand>();
            app.Configure(config =>
            {
                app.WithDescription("Run a specific problem for an Advent of Code 2023 puzzle using a specific input file.");
            });
            app.Run(args);
        }
    }
}
