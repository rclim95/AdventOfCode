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
        /// <summary>
        /// Part 1 should be executed.
        /// </summary>
        Part1 = 1,

        /// <summary>
        /// Part 2 should be executed.
        /// </summary>
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
        /// Solves a specific part of the puzzle for a particular day.
        /// </summary>
        /// <param name="day">The particular day.</param>
        /// <param name="part">The particular part.</param>
        /// <param name="inputFile">The input file that should be passed to the part to solve.</param>
        /// <returns>The output of the part.</returns>
        public string Run(int day, PuzzlePart part, StreamReader inputFile)
        {
            var selectedPuzzle = _puzzles.FirstOrDefault(p => p.Day == day);
            if (selectedPuzzle == null)
            {
                throw new ArgumentException($"Day {day} is not supported.", nameof(day));
            }

            switch (part)
            {
                case PuzzlePart.Part1:
                    return selectedPuzzle.Part1(inputFile);

                case PuzzlePart.Part2:
                    return selectedPuzzle.Part2(inputFile);

                default:
                    throw new ArgumentException($"Part {part} is not supported.", nameof(part));
            }
        }

        private void RegisterPuzzle<TPuzzle>() where TPuzzle : IPuzzle
        {
            _puzzles.Add(new PuzzleRecord(
                TPuzzle.Day,
                TPuzzle.GetPart1Answer,
                TPuzzle.GetPart2Answer
            ));
        }
    }
}
