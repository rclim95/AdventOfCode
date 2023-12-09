// <copyright file="Day9.cs" company="rclim95">
// Copyright (c) rclim95
//
// Licensed under the MIT License. For more info, see the LICENSE.md at the root of this repo.
// </copyright>


using System.Reflection.Metadata.Ecma335;
using MoreLinq;

namespace AdventOfCode.Problems.Days;

/// <summary>
/// Implements the solution for solving Day 9 of Advent of Code.
/// </summary>
internal class Day9 : IPuzzle
{
    /// <inheritdoc />
    public static int Day => 9;

    public static string GetPart1Answer(StreamReader reader)
    {
        int sumOfExtrapolatedValues = 0;
        foreach (int[] historicalValue in GetHistoricalValues(reader))
        {
            // Keep track of the last value for each line as we slide through the list.
            List<int> lastValueOfLines = [
                historicalValue.Last()
            ];

            // Work in reverse and use a window function to slide from the end to the beginning,
            // calculating the difference. Keep doing this until the last difference is 0.
            IEnumerable<int> currentLine = historicalValue.Reverse();
            while (true)
            {
                currentLine = currentLine
                    .Window(2)
                    .Select(v => v[0] - v[1])
                    .ToList();

                // NB: Remember that we reversed the list, so the last value for the current line
                //     we're processing is actually the first in currentLine.
                int lastDiffOfLine = currentLine.First();
                if (lastDiffOfLine == 0)
                {
                    // No more to process, we can work backwards to figure out our extrapolated value. :)
                    break;
                }

                lastValueOfLines.Add(lastDiffOfLine);
            }

            // The sum of the values collected in the lines (of the last difference calculated) will
            // be the extrapolated value.
            sumOfExtrapolatedValues += lastValueOfLines.Sum();
        }

        return sumOfExtrapolatedValues.ToString();
    }

    public static string GetPart2Answer(StreamReader reader)
    {
        int sumOfExtrapolatedValues = 0;
        foreach (int[] historicalValue in GetHistoricalValues(reader))
        {
            // Keep track of the first value for each line as we slide through the list.
            List<int> firstValueOfLines = [
                historicalValue.First()
            ];

            // Just like the previous part, use a window function to slide from the beginning to
            // the end, calculating the difference. Keep doing this until the last line has a
            // difference of 0 for all values.
            IEnumerable<int> currentLine = historicalValue;
            while (true)
            {
                currentLine = currentLine
                    .Window(2)
                    .Select(v => v[1] - v[0])
                    .ToList();

                if (!currentLine.Any(v => v != 0))
                {
                    // No more to process, we can work backwards to figure out our previous
                    // extrapolated value. :)
                    break;
                }

                firstValueOfLines.Add(currentLine.First());
            }

            // Reverse firstValuesOfLine so that the bottom-most (close to zero) lines are at the top.
            firstValueOfLines.Reverse();

            // Now that we've collected the first value of each line, use that information to
            // extrapolate the previous value by subtracting the first value of the current line
            // with the current difference that was calculated for the previous line.
            int currentDiff = firstValueOfLines[0];
            foreach (int firstValueOfCurrentLine in firstValueOfLines.Skip(1))
            {
                currentDiff = firstValueOfCurrentLine - currentDiff;
            }

            sumOfExtrapolatedValues += currentDiff;
        }

        return sumOfExtrapolatedValues.ToString();
    }

    private static IEnumerable<int[]> GetHistoricalValues(StreamReader reader)
    {
        while (true)
        {
            string? currentLine = reader.ReadLine();
            if (currentLine == null)
            {
                yield break;
            }

            // Read the current historical values recorded for this line, transforming it from
            // a string array into an int.
            yield return currentLine.Split(" ")
                .Select(int.Parse)
                .ToArray();
        }
    }
}
