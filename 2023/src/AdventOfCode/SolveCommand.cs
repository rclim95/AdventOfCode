using AdventOfCode.Problems;
using Spectre.Console;
using Spectre.Console.Cli;
using System.ComponentModel;
using System.Diagnostics;

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
                using StreamReader reader = File.OpenText(settings.File!);
                if (settings.Part!.Value.HasFlag(Settings.Parts.Part1))
                {
                    try
                    {
                        string result = runner.Run(settings.Day!.Value, PuzzlePart.Part1, reader);
                        AnsiConsole.MarkupLineInterpolated($"> Day {settings.Day}, Part 1 Answer: :check_mark:  [bold]{result}[/]");
                    }
                    catch (NotImplementedException)
                    {
                        AnsiConsole.MarkupLineInterpolated($"> Day {settings.Day}, Part 1 Answer: :warning:  [bold red]Not Implemented[/]");
                    }
                    catch (Exception ex)
                    {
                        AnsiConsole.MarkupLineInterpolated($"> Day {settings.Day}, Part 1 Answer: :cross_mark: [bold red]Error[/]");
                        AnsiConsole.WriteException(ex);
                    }
                }

                if (settings.Part!.Value.HasFlag(Settings.Parts.Part2))
                {
                    // In case part 1 was executed, reset the stream to the beginning so that the
                    // input file will be read properly.
                    reader.BaseStream.Position = 0;
                    reader.DiscardBufferedData();

                    try
                    {
                        string result = runner.Run(settings.Day!.Value, PuzzlePart.Part2, reader);
                        AnsiConsole.MarkupLineInterpolated($"> Day {settings.Day}, Part 2 Answer: :check_mark:  [bold]{result}[/]");
                    }
                    catch (NotImplementedException)
                    {
                        AnsiConsole.MarkupLineInterpolated($"> Day {settings.Day}, Part 2 Answer: :warning:  [bold red]Not Implemented[/]");
                    }
                    catch (Exception ex)
                    {
                        AnsiConsole.MarkupLineInterpolated($"> Day {settings.Day}, Part 2 Answer: :cross_mark:  [bold red]Error[/]");
                        AnsiConsole.WriteException(ex);
                    }
                }

                return 0;
            }
            catch (Exception ex)
            {
                AnsiConsole.MarkupLine("[bold red]ERROR:[/] An unhandled exception occurred:");
                AnsiConsole.WriteException(ex);

                return 1;
            }
        }

        private Settings PromptForArgumentsFromUser(PuzzleRunner runner)
        {
            var day = AnsiConsole.Prompt(new SelectionPrompt<int>()
                .Title("Which day would you like to run?")
                .AddChoices(runner.Days));

            Settings.Parts partsToRun = Settings.Parts.None;
            var parts = AnsiConsole.Prompt(new MultiSelectionPrompt<string>()
                .Title("Which part(s) would you like to run?")
                .AddChoices(
                    "Part 1",
                    "Part 2"
                ));
            if (parts.Contains("Part 1"))
            {
                partsToRun &= Settings.Parts.Part1;
            }
            if (parts.Contains("Part 2"))
            {
                partsToRun &= Settings.Parts.Part2;
            }

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
                Part = partsToRun,
                File = filePath
            };
        }

        public sealed class Settings : CommandSettings
        {
            [Description("The specific Advent of Day puzzle to solve.")]
            [CommandArgument(0, "[day]")]
            public int? Day { get; init; }

            [Description("The specific part of the puzzle to solve.")]
            [CommandArgument(1, "[part]")]
            public Parts? Part { get; init; }

            [Description("The path to the input file that should be used for solving the puzzle/problem.")]
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
                if (!Interactive && (Day == null || Part == null || File == null))
                {
                    return ValidationResult.Error( "The day, part, and input arguments are required when --interactive isn't passed.");
                }

                return ValidationResult.Success();
            }

            public enum Parts
            {
                None = 0,
                Part1 = (1 << 0),
                Part2 = (1 << 1),
                All = Part1 | Part2
            }
        }
    }
}
