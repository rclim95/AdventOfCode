using AdventOfCode.Problems.Days;
using Spectre.Console;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdventOfCode.Problems
{
    /// <summary>
    /// Defines the available parts that can be executed for a puzzle.
    /// </summary>
    internal enum PuzzlePart
    {
        Part1 = 1,
        Part2 = 2
    }

    /// <summary>
    /// Provide a class responsible for running a <see cref="IPuzzle" /> class.
    /// </summary>
    internal sealed class PuzzleRunner
    {
        private readonly List<PuzzleRecord> _puzzles;

        /// <summary>
        /// Initializes a new instance of the <see cref="PuzzleRunner" /> class.
        /// </summary>
        public PuzzleRunner()
        {
            _puzzles = new List<PuzzleRecord>();

            RegisterPuzzle<Day1>();
            RegisterPuzzle<Day2>();
            RegisterPuzzle<Day3>();
        }

        /// <summary>
        /// Gets the available days that the runner supports.
        /// </summary>
        public IReadOnlyCollection<int> Days => _puzzles.Select(p => p.Day).ToList();

        /// <summary>
        /// Solves a problem for a particular day.
        /// </summary>
        /// <param name="day">The particular day.</param>
        /// <param name="part">The particular problem.</param>
        /// <param name="inputFile">The input file that should be passed to the problem to solve.</param>
        public void Solve(int day, PuzzlePart part, TextReader inputFile)
        {
            var selectedPuzzle = _puzzles.FirstOrDefault(p => p.Day == day);
            if (selectedPuzzle == null)
            {
                AnsiConsole.MarkupLineInterpolated($"[bold red]ERROR:[/] Day {day} does not exist.");
                return;
            }

            switch (part)
            {
                case PuzzlePart.Part1:
                    AnsiConsole.MarkupLineInterpolated($"[bold]Day {day}, Part 1 Answer:[/] {selectedPuzzle.Part1(inputFile)}");
                    break;

                case PuzzlePart.Part2:
                    AnsiConsole.MarkupLineInterpolated($"[bold]Day {day}, Part 2 Answer:[/] {selectedPuzzle.Part2(inputFile)}");
                    break;
            }
        }

        private void RegisterPuzzle<TPuzzle>() where TPuzzle : IPuzzle
        {
            _puzzles.Add(new PuzzleRecord(
                TPuzzle.Day,
                (reader) => TPuzzle.GetPart1Answer(reader),
                (reader) => TPuzzle.GetPart2Answer(reader)
            ));
        }
    }
}
