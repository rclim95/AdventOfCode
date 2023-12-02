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
    /// Defines the available problems that can be executed for a puzzle.
    /// </summary>
    internal enum PuzzleProblem
    {
        Problem1,
        Problem2
    }

    /// <summary>
    /// Provide a class responsible for running a <see cref="IPuzzle" /> class.
    /// </summary>
    internal class PuzzleRunner
    {
        private List<PuzzleRecord> _puzzles;

        /// <summary>
        /// Initializes a new instance of the <see cref="PuzzleRunner" /> class.
        /// </summary>
        public PuzzleRunner()
        {
            _puzzles = new List<PuzzleRecord>();

            RegisterPuzzle<Day1>();
            RegisterPuzzle<Day2>();
        }

        /// <summary>
        /// Solves a problem for a particular day.
        /// </summary>
        /// <param name="day">The particular day.</param>
        /// <param name="problem">The particular problem.</param>
        /// <param name="inputFile">The input file that should be passed to the problem to solve.</param>
        public void Solve(int day, PuzzleProblem problem, TextReader inputFile)
        {
            var selectedProblem = _puzzles.FirstOrDefault(p => p.Day == day);
            if (selectedProblem == null)
            {
                AnsiConsole.MarkupLineInterpolated($"[bold red]ERROR:[/] Day {day} does not exist.");
                return;
            }

            switch (problem)
            {
                case PuzzleProblem.Problem1:
                    AnsiConsole.MarkupLineInterpolated($"[bold]Day {day}, Problem 1 Answer:[/] {selectedProblem.Problem1(inputFile)}");
                    break;

                case PuzzleProblem.Problem2:
                    AnsiConsole.MarkupLineInterpolated($"[bold]Day {day}, Problem 2 Answer:[/] {selectedProblem.Problem2(inputFile)}");
                    break;
            }
        }

        private void RegisterPuzzle<TPuzzle>() where TPuzzle : IPuzzle
        {
            _puzzles.Add(new PuzzleRecord(
                TPuzzle.Day,
                (reader) => TPuzzle.GetAnswerForProblem1(reader),
                (reader) => TPuzzle.GetAnswerForProblem2(reader)
            ));
        }
    }
}
