using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdventOfCode.Problems
{
    /// <summary>
    /// Provides a contract for a class that is used to solve an Advent of Code puzzle.
    /// </summary>
    internal interface IPuzzle
    {
        /// <summary>
        /// Gets the day that this problem is solving.
        /// </summary>
        static abstract int Day { get; }

        /// <summary>
        /// Gets the answer for solving the first problem.
        /// </summary>
        /// <param name="reader">A handle to the input file that should be used for solving the problem.</param>
        /// <returns>The answer to the first problem.</returns>
        static abstract string GetAnswerForProblem1(TextReader reader);

        /// <summary>
        /// Gets the answer for solving the second problem.
        /// </summary>
        /// <param name="reader">A handle to the input file that should be used for solving the problem.</param>
        /// <returns>The answer to the second problem.</returns>
        static abstract string GetAnswerForProblem2(TextReader reader);
    }
}
