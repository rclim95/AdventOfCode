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
                PuzzleRunner runner = new();

                if (settings.Debug)
                {
                    Debugger.Launch();
                }

                if (settings.Interactive)
                {
                    settings = PromptForArgumentsFromUser(runner);
                }

                // NB: These can't possibly be null, as this will be provided either in interaction
                // mode or on the command line args. The command line args will be validated so that
                // the day, problem, and (input) file are provided if the app isn't running in
                // interactive mode. :)
                using TextReader reader = File.OpenText(settings.File!);
                runner.Solve(settings.Day!.Value, settings.Problem!.Value, reader);

                return 0;
            }
            catch (Exception ex)
            {
                AnsiConsole.WriteException(ex);

                return 1;
            }
        }

        private Settings PromptForArgumentsFromUser(PuzzleRunner runner)
        {
            var day = AnsiConsole.Prompt(new SelectionPrompt<int>()
                .Title("Which day would you like to run?")
                .AddChoices(runner.Days));

            var problem = AnsiConsole.Prompt(new SelectionPrompt<string>()
                .Title("Which problem would you like to run?")
                .AddChoices(
                    "Problem 1",
                    "Problem 2"
                ));

            string filePath = string.Empty;
            while (true)
            {
                filePath = AnsiConsole.Ask<string>("Enter the path to the input file to use.");
                if (File.Exists(filePath))
                {
                    break;
                }

                // The file doesn't exist.
                AnsiConsole.MarkupLine("[bold red]Error:[/] The file at the specified path does not exist.");
            }

            return new Settings()
            {
                Day = day,
                Problem = problem switch
                {
                    "Problem 1" => PuzzlePart.Part1,
                    "Problem 2" => PuzzlePart.Part2,
                    _ => throw new NotSupportedException()
                },
                File = filePath
            };
        }

        public sealed class Settings : CommandSettings
        {
            [Description("The specific Advent of Day puzzle to solve.")]
            [CommandArgument(0, "[day]")]
            public int? Day { get; init; }

            [Description("The specific problem of the puzzle to solve (either \"Problem1\" or \"Problem2\"). If omitted, both problems will be outputted.")]
            [CommandArgument(1, "[problem]")]
            public PuzzlePart? Problem { get; init; }

            [Description("The path to the input file that should be used for solving the puzzle/problem. If omitted, it is assumed that the input will come from STDIN.")]
            [CommandArgument(2, "[input]")]
            public string? File { get; init; }

            [Description("Launch the JIT debugger on startup.")]
            [CommandOption("-d|--debug")]
            [DefaultValue(false)]
            public bool Debug { get; init; }

            [Description("Allow interactive operations with the user. All command line arguments passed in will be ignored.")]
            [CommandOption("-i|--interactive")]
            [DefaultValue(false)]
            public bool Interactive { get; init; }

            public override ValidationResult Validate()
            {
                if (!Interactive && (Day == null || Problem == null || File == null))
                {
                    return ValidationResult.Error( "The day, problem, and input arguments are required when --interactive isn't passed.");
                }

                return ValidationResult.Success();
            }
        }
    }
}
