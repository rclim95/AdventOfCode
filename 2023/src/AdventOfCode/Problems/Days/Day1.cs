// <copyright file="Day1.cs" company="rclim95">
// Copyright (c) rclim95
//
// Licensed under the MIT License. For more info, see the LICENSE.md at the root of this repo.
// </copyright>

namespace AdventOfCode.Problems.Days;

/// <summary>
/// Provides the solution for Day 1 of Advent of Code.
/// </summary>
internal sealed class Day1 : IPuzzle
{
    /// <inheritdoc />
    public static int Day => 1;

    /// <inheritdoc />
    public static string GetPart1Answer(StreamReader reader)
    {
        int sum = 0;
        while (true)
        {
            string? currentLine = reader.ReadLine();
            if (currentLine == null)
            {
                // We've read everything in this file, so abort.
                break;
            }

            // Look for the first digit that we come across at the beginning of the string and
            // the first digit we come across at the end of the string and concatenate the two.
            // This will be our calibration value of interest.
            char firstDigit = currentLine.First(char.IsAsciiDigit);
            char lastDigit = currentLine.Last(char.IsAsciiDigit);

            sum += int.Parse(string.Concat(firstDigit, lastDigit));
        }

        return sum.ToString();
    }

    /// <inheritdoc />
    public static string GetPart2Answer(StreamReader reader)
    {
        int sum = 0;
        while (true)
        {
            string? currentLine = reader.ReadLine();
            if (currentLine == null)
            {
                // We've read everything in this file, so abort.
                break;
            }

            // The second problem throws a wrench into this puzzle: we now need to consider digits
            // spelled out, e.g., "one" => 1, "two" => 2, etc.
            //
            // Find the spelled out substrings (in addition to the digits) at the beginning
            // of the string and the end of the string, and concatenate the two together.
            int firstDigit = FindFirstDigit(currentLine);
            int lastDigit = FindLastDigit(currentLine);

            sum += (firstDigit * 10) + lastDigit;
        }

        return sum.ToString();
    }

    private static int FindFirstDigit(string input)
    {
        for (int i = 0; i < input.Length; i++)
        {
            char currentCharacter = input[i];
            if (char.IsAsciiDigit(currentCharacter))
            {
                // NB: Given the fact that ASCII is nothing more than a numerical encoding of
                // characters, we can convert the character interpretation of a digit to its
                // integer form by subtracting it from ASCII '0'.
                return currentCharacter - '0';
            }
            else
            {
                if (IsSubstringMatchAtIndex(input, i, "one"))
                {
                    return 1;
                }
                else if (IsSubstringMatchAtIndex(input, i, "two"))
                {
                    return 2;
                }
                else if (IsSubstringMatchAtIndex(input, i, "three"))
                {
                    return 3;
                }
                else if (IsSubstringMatchAtIndex(input, i, "four"))
                {
                    return 4;
                }
                else if (IsSubstringMatchAtIndex(input, i, "five"))
                {
                    return 5;
                }
                else if (IsSubstringMatchAtIndex(input, i, "six"))
                {
                    return 6;
                }
                else if (IsSubstringMatchAtIndex(input, i, "seven"))
                {
                    return 7;
                }
                else if (IsSubstringMatchAtIndex(input, i, "eight"))
                {
                    return 8;
                }
                else if (IsSubstringMatchAtIndex(input, i, "nine"))
                {
                    return 9;
                }
            }
        }

        throw new ArgumentException("The input string does not contain a valid digit.", nameof(input));
    }

    private static bool IsSubstringMatchAtIndex(string input, int index, string substring)
    {
        // How many characters are there remaining at the index? If the substring exceeds this,
        // then we definitely know there is not a match.
        int characterRemaining = input.Length - index;
        if (substring.Length > characterRemaining)
        {
            return false;
        }

        return input.Substring(index, substring.Length) == substring;
    }

    private static int FindLastDigit(string input)
    {
        for (int i = input.Length - 1; i >= 0; i--)
        {
            char currentCharacter = input[i];
            if (char.IsAsciiDigit(currentCharacter))
            {
                // NB: Given the fact that ASCII is nothing more than a numerical encoding of
                // characters, we can convert the character interpretation of a digit to its
                // integer form by subtracting it from ASCII '0'.
                return currentCharacter - '0';
            }
            else
            {
                if (IsSubstringMatchAtEndIndex(input, i, "one"))
                {
                    return 1;
                }
                else if (IsSubstringMatchAtEndIndex(input, i, "two"))
                {
                    return 2;
                }
                else if (IsSubstringMatchAtEndIndex(input, i, "three"))
                {
                    return 3;
                }
                else if (IsSubstringMatchAtEndIndex(input, i, "four"))
                {
                    return 4;
                }
                else if (IsSubstringMatchAtEndIndex(input, i, "five"))
                {
                    return 5;
                }
                else if (IsSubstringMatchAtEndIndex(input, i, "six"))
                {
                    return 6;
                }
                else if (IsSubstringMatchAtEndIndex(input, i, "seven"))
                {
                    return 7;
                }
                else if (IsSubstringMatchAtEndIndex(input, i, "eight"))
                {
                    return 8;
                }
                else if (IsSubstringMatchAtEndIndex(input, i, "nine"))
                {
                    return 9;
                }
            }
        }

        throw new ArgumentException("The input string does not contain a valid digit.", nameof(input));
    }

    private static bool IsSubstringMatchAtEndIndex(string input, int endIndex, string substring)
    {
        // Since we're trying to find a substring match at the end of the index (e.g.,
        // given the string "hello" and endIndex is 4, if substring is "ello", it would be
        // a match because "ello" starts at 1 and ends at 4), we need to figure out the
        // supposed starting index.
        int startIndex = (endIndex + 1) - substring.Length;
        if (startIndex < 0)
        {
            // This definitely can't be a match--the substring would be too long to match the
            // input string at this end index.
            return false;
        }

        return input.Substring(startIndex, substring.Length) == substring;
    }
}
