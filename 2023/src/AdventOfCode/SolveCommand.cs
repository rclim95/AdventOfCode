// <copyright file="SolveCommand.cs" company="rclim95">
// Copyright (c) rclim95
//
// Licensed under the MIT License. For more info, see the LICENSE.md at the root of this repo.
// </copyright>

using System.ComponentModel;
using System.Diagnostics;
using AdventOfCode.Problems;
using Spectre.Console;
using Spectre.Console.Cli;

namespace AdventOfCode;

/// <summary>
/// Provides a command for solving an Advent of Code problem.
/// </summary>
internal sealed class SolveCommand : Command<SolveCommand.Settings>
{
    /// <inheritdoc />
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
                var stopwatch = new Stopwatch();
                try
                {
                    stopwatch.Start();
                    string result = runner.Run(settings.Day!.Value, PuzzlePart.Part1, reader);
                    stopwatch.Stop();

                    AnsiConsole.MarkupLineInterpolated($"Day {settings.Day}, Part 1 Answer: :check_mark:  [bold]{result}[/]");
                    AnsiConsole.MarkupLineInterpolated($"> :timer_clock:  Execution Time: {stopwatch.Elapsed.TotalMilliseconds:0.00}ms");
                }
                catch (NotImplementedException)
                {
                    AnsiConsole.MarkupLineInterpolated($"Day {settings.Day}, Part 1 Answer: :warning:  [bold red]Not Implemented[/]");
                }
                catch (Exception ex)
                {
                    AnsiConsole.MarkupLineInterpolated($"Day {settings.Day}, Part 1 Answer: :cross_mark: [bold red]Error[/]");
                    AnsiConsole.WriteException(ex);
                }
            }

            if (settings.Part!.Value.HasFlag(Settings.Parts.Part2))
            {
                // In case part 1 was executed, reset the stream to the beginning so that the
                // input file will be read properly.
                reader.BaseStream.Position = 0;
                reader.DiscardBufferedData();

                var stopwatch = new Stopwatch();
                try
                {
                    stopwatch.Start();
                    string result = runner.Run(settings.Day!.Value, PuzzlePart.Part2, reader);
                    stopwatch.Stop();

                    AnsiConsole.MarkupLineInterpolated($"Day {settings.Day}, Part 2 Answer: :check_mark:  [bold]{result}[/]");
                    AnsiConsole.MarkupLineInterpolated($"> :timer_clock:  Execution Time: {stopwatch.Elapsed.TotalMilliseconds:0.00}ms");
                }
                catch (NotImplementedException)
                {
                    AnsiConsole.MarkupLineInterpolated($"Day {settings.Day}, Part 2 Answer: :warning:  [bold red]Not Implemented[/]");
                }
                catch (Exception ex)
                {
                    AnsiConsole.MarkupLineInterpolated($"Day {settings.Day}, Part 2 Answer: :cross_mark:  [bold red]Error[/]");
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
                "Part 2"));
        if (parts.Contains("Part 1"))
        {
            partsToRun |= Settings.Parts.Part1;
        }

        if (parts.Contains("Part 2"))
        {
            partsToRun |= Settings.Parts.Part2;
        }

        string filePath = string.Empty;
        while (true)
        {
            filePath = AnsiConsole.Ask<string>("Enter the path to the input file to use:");
            if (File.Exists(filePath))
            {
                break;
            }

            // The file doesn't exist.
            AnsiConsole.MarkupLineInterpolated(
                $"[bold red]Error:[/] The file [bold]{Path.GetFullPath(filePath)}[/] does not exist.");
        }

        return new Settings()
        {
            Day = day,
            Part = partsToRun,
            File = filePath
        };
    }

    /// <summary>
    /// Defines the command line arguments and options that can be passed to the command line.
    /// </summary>
    public sealed class Settings : CommandSettings
    {
        /// <summary>
        /// Defines the available puzzle parts that the user can run.
        /// </summary>
        [Flags]
        public enum Parts
        {
            /// <summary>
            /// Run no parts.
            /// </summary>
            None = 0,

            /// <summary>
            /// Run the first part.
            /// </summary>
            Part1 = (1 << 0),

            /// <summary>
            /// Run the second part.
            /// </summary>
            Part2 = (1 << 1),

            /// <summary>
            /// Run all parts.
            /// </summary>
            All = Part1 | Part2
        }

        /// <summary>
        /// Gets the Advent of Code puzzle day the user wants to run.
        /// </summary>
        [Description("The specific Advent of Day puzzle to solve.")]
        [CommandArgument(0, "[day]")]
        public int? Day { get; init; }

        /// <summary>
        /// Gets the specific part of the Advent of Code puzzle the user wants to run.
        /// </summary>
        [Description("The specific part of the puzzle to solve.")]
        [CommandArgument(1, "[part]")]
        public Parts? Part { get; init; }

        /// <summary>
        /// Gets the path to the input file.
        /// </summary>
        [Description("The path to the input file that should be used for solving the puzzle/problem.")]
        [CommandArgument(2, "[input]")]
        public string? File { get; init; }

        /// <summary>
        /// Gets a value indicating whether the JIT debugger should launch once the app starts.
        /// </summary>
        [Description("Launch the JIT debugger on startup.")]
        [CommandOption("-d|--debug")]
        [DefaultValue(false)]
        public bool Debug { get; init; }

        /// <summary>
        /// Gets a value indicating whether the app can prompt the user for input.
        /// </summary>
        [Description("Allow interactive operations with the user. All command line arguments passed in will be ignored.")]
        [CommandOption("-i|--interactive")]
        [DefaultValue(false)]
        public bool Interactive { get; init; }

        /// <inheritdoc />
        public override ValidationResult Validate()
        {
            if (!Interactive && (Day == null || Part == null || File == null))
            {
                return ValidationResult.Error( "The day, part, and input arguments are required when --interactive isn't passed.");
            }

            return ValidationResult.Success();
        }
    }
}
