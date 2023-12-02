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
    /// <param name="Problem1">The function that will return the solution to the first problem of the puzzle.</param>
    /// <param name="Problem2">The function that will return the solution to the second problem of the puzzle.</param>
    internal record class PuzzleRecord(int Day, Func<TextReader, string> Problem1, Func<TextReader, string> Problem2);
}
