// <copyright file="Day15.cs" company="rclim95">
// Copyright (c) rclim95
//
// Licensed under the MIT License. For more info, see the LICENSE.md at the root of this repo.
// </copyright>

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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
            runningSum += Hash(step);
        }

        return runningSum.ToString();
    }

    /// <inheritdoc />
    public static string GetPart2Answer(TextReader reader)
    {
        // Initialize our boxes. We're expecting a total of 256 boxes to deal with. :)
        Dictionary<int, List<(string Label, int FocalLength)>> boxesAndLenses = Enumerable.Range(0, 256)
            .ToDictionary(b => b, b => new List<(string Label, int FocalLength)>());

        foreach (string step in GetInitializeSequenceSteps(reader))
        {
            Match match = Regex.Match(step, @"(?'label'\w+)(?'operation'=|-)(?'length'\d)*");
            string label = match.Groups["label"].Value;
            string operation = match.Groups["operation"].Value;
            int box = Hash(label);

            if (operation == "-")
            {
                // We are removing a lens with the specified label. Go to the box, look for the lens
                // with the provided label, and then remove it.
                List<(string Label, int FocalLength)> lenses = boxesAndLenses[box];
                int indexOfLabel = lenses.FindIndex(l => l.Label == label);
                if (indexOfLabel >= 0)
                {
                    lenses.RemoveAt(indexOfLabel);
                }
            }
            else if (operation == "=")
            {
                // We are adding a lens with the specified label and focal length into the box at the
                // end. Go to the box, and add it _or_ if the label already exists, replace it!
                List<(string Label, int FocalLength)> lenses = boxesAndLenses[box];
                int indexOfLabel = lenses.FindIndex(l => l.Label == label);
                int length = int.Parse(match.Groups["length"].Value);
                if (indexOfLabel >= 0)
                {
                    lenses[indexOfLabel] = (label, length);
                }
                else
                {
                    lenses.Add((label, length));
                }
            }
        }

        // Now sum up all the focusing power of each lenses.
        int totalFocusPower = 0;
        foreach (KeyValuePair<int, List<(string Label, int FocalLength)>> boxAndLenses in boxesAndLenses)
        {
            if (boxAndLenses.Value.Count == 0)
            {
                continue;
            }

            foreach (((string label, int focalLength), int slot) in boxAndLenses.Value.Select((l, i) => (l, i + 1)))
            {
                totalFocusPower += (1 + boxAndLenses.Key) * slot * focalLength;
            }
        }

        return totalFocusPower.ToString();
    }

    private static int Hash(string input)
    {
        // The conversion process is as follow:
        // - Determine the ASCII code for the current character.
        // - Increase the current value by that ASCII code.
        // - Multiply the current value by 17.
        // - Divide the current value by 256 and take the modulo (remainder).
        //
        // Rinse and repeat for each step. :)
        int currentValue = 0;
        foreach (char character in input)
        {
            currentValue += (int)character;
            currentValue *= 17;
            currentValue %= 256;
        }

        return currentValue;
    }

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
            if (!char.IsWhiteSpace(currentCharacter))
            {
                sb.Append(currentCharacter);
            }
        }
    }
}
