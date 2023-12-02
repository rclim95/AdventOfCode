using AdventOfCode.Problems;
using Spectre.Console;
using Spectre.Console.Cli;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdventOfCode
{
    /// <summary>
    /// Provides a command for solving an Advent of Code problem.
    /// </summary>
    internal sealed class SolveCommand : Command<SolveCommand.Settings>
    {
        public override int Execute(CommandContext context, Settings settings)
        {
            try
            {
                if (settings.Debug)
                {
                    Debugger.Launch();
                }

                using TextReader reader = File.OpenText(settings.File);

                PuzzleRunner runner = new();
                runner.Solve(settings.Day, settings.Problem, reader);

                return 0;
            }
            catch (Exception ex)
            {
                AnsiConsole.WriteException(ex);

                return 1;
            }
        }

        public sealed class Settings : CommandSettings
        {
            [Description("Launch the JIT debugger on start.")]
            [CommandOption("-d|--debug")]
            [DefaultValue(false)]
            public bool Debug { get; set; }

            [Description("The path to the input file that should be used for solving the puzzle/problem.")]
            [CommandArgument(0, "<input>")]
            public required string File { get; init; }

            [Description("The specific Advent of Day puzzle to solve.")]
            [CommandArgument(1, "<day>")]
            public int Day { get; init; }

            [Description("The specific problem of the puzzle to solve.")]
            [CommandArgument(2, "<problem>")]
            public PuzzleProblem Problem { get; init; }
        }
    }
}
