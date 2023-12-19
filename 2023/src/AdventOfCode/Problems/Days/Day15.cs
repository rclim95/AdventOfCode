// <copyright file="Day15.cs" company="rclim95">
// Copyright (c) rclim95
//
// Licensed under the MIT License. For more info, see the LICENSE.md at the root of this repo.
// </copyright>

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdventOfCode.Problems.Days.Day15;

/// <summary>
/// Implements the solution to Day 15 of Advent of Code.
/// </summary>
internal class Day15 : IPuzzle
{
    /// <inheritdoc />
    public static int Day => 15;

    /// <inheritdoc />
    public static string GetPart1Answer(TextReader reader)
    {
        int runningSum = 0;
        foreach (string step in GetInitializeSequenceSteps(reader))
        {
            // The conversion process is as follow:
            // - Determine the ASCII code for the current character.
            // - Increase the current value by that ASCII code.
            // - Multiply the current value by 17.
            // - Divide the current value by 256 and take the modulo (remainder).
            //
            // Rinse and repeat for each step. :)
            int currentValue = 0;
            foreach (char character in step)
            {
                currentValue += (int)character;
                currentValue *= 17;
                currentValue %= 256;
            }

            Console.WriteLine($"{step} = {currentValue}");
            runningSum += currentValue;
        }

        return runningSum.ToString();
    }

    /// <inheritdoc />
    public static string GetPart2Answer(TextReader reader) => throw new NotImplementedException();

    private static IEnumerable<string> GetInitializeSequenceSteps(TextReader reader)
    {
        var sb = new StringBuilder();

        // The input is basically a comma-separated list of steps. We'll manually implement
        // the splitting for efficiency. :)
        while (true)
        {
            int peekValue = reader.Peek();
            if (peekValue == -1)
            {
                // There are no more values to read, so return whatever remains in our buffer, and then
                // stop.
                yield return sb.ToString();
                yield break;
            }

            char peekCharacter = (char)peekValue;
            if (peekCharacter == ',')
            {
                // We've read a step, so go ahead and return the current string, advance to
                // our next token!
                yield return sb.ToString();
                sb.Clear();
                reader.Read();
            }

            char currentCharacter = (char)reader.Read();
            if (currentCharacter != '\n')
            {
                sb.Append(currentCharacter);
            }
        }
    }
}
