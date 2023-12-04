using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdventOfCode.Problems
{
    /// <summary>
    /// Encapsulates information about an Advent of Code puzzle.
    /// </summary>
    /// <param name="Day">The day that this puzzle is for.</param>
    /// <param name="Part1">The function that will return the solution to the first part of the puzzle.</param>
    /// <param name="Part2">The function that will return the solution to the second part of the puzzle.</param>
    internal sealed record class PuzzleRecord(int Day, Func<TextReader, string> Part1, Func<TextReader, string> Part2);
}
