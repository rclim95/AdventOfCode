// <copyright file="IPuzzle.cs" company="rclim95">
// Copyright (c) rclim95
//
// Licensed under the MIT License. For more info, see the LICENSE.md at the root of this repo.
// </copyright>

namespace AdventOfCode.Problems;

/// <summary>
/// Provides a contract for a class that is used to solve an Advent of Code puzzle.
/// </summary>
internal interface IPuzzle
{
    /// <summary>
    /// Gets the day that this puzzle is solving.
    /// </summary>
    static abstract int Day { get; }

    /// <summary>
    /// Gets the answer for solving the first part of the puzzle.
    /// </summary>
    /// <param name="reader">A handle to the input file that should be used for solving the part.</param>
    /// <returns>The answer to the first part.</returns>
    static abstract string GetPart1Answer(TextReader reader);

    /// <summary>
    /// Gets the answer for solving the second part of the puzzle.
    /// </summary>
    /// <param name="reader">A handle to the input file that should be used for solving the part.</param>
    /// <returns>The answer to the second part.</returns>
    static abstract string GetPart2Answer(TextReader reader);
}
