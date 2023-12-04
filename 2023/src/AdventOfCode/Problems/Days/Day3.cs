using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace AdventOfCode.Problems.Days
{
    internal sealed partial class Day3 : IPuzzle
    {
        /// <inheritdoc />
        public static int Day => 3;

        /// <inheritdoc />
        public static string GetPart1Answer(TextReader reader)
        {
            int sumOfPartNumbers = 0;
            List<string> schematicLines = ReadLinesIntoList(reader);

            // Go through each line in the schematic.
            for (int y = 0; y < schematicLines.Count; y++)
            {
                string currentLine = schematicLines[y];

                // Use regular expression to locate potential part numbers.
                MatchCollection matches = Digits().Matches(currentLine);
                if (matches.Count == 0)
                {
                    // We didn't find any potential part numbers, so skip it.
                    continue;
                }

                foreach (Match match in matches.Cast<Match>())
                {
                    // Look around the match to see if we can find a symbol that isn't a period. If
                    // we find said symbol, then we know that this number _must_ be a part number!
                    //
                    // Start from the top (if possible).
                    if (y - 1 >= 0)
                    {
                        // Read the previous line and get the substring of characters that
                        // are adjacent to the current match above.
                        string previousLine = schematicLines[y - 1];
                        string adjacentCharacters = previousLine.Substring(Math.Max(0, match.Index - 1), Math.Min(match.Length + 2, currentLine.Length - match.Index));

                        // Do a find/replace of "." with an empty string--if the resulting substitution
                        // leads to an non-empty string, then we know we have a symbol, so this
                        // match should be considered a part number.
                        if (adjacentCharacters.Replace(".", string.Empty).Length > 0)
                        {
                            sumOfPartNumbers += int.Parse(match.Value);
                            continue;
                        }
                    }

                    if (y + 1 < schematicLines.Count)
                    {
                        // Read the next line and get the substring of characters that are adjacent
                        // to the current match underneath.
                        string nextLine = schematicLines[y + 1];
                        string adjacentCharacters = nextLine.Substring(Math.Max(0, match.Index - 1), Math.Min(match.Length + 2, currentLine.Length - match.Index));

                        // Do a find/replace of "." with an empty string--if the resulting substitution
                        // leads to an non-empty string, then we know we have a symbol, so this
                        // match should be considered a part number.
                        if (adjacentCharacters.Replace(".", string.Empty).Length > 0)
                        {
                            sumOfPartNumbers += int.Parse(match.Value);
                            continue;
                        }
                    }

                    // Look to the left and right of the match for a symbol—if we find a non-period
                    // character, then we need to consider the match to be a part number.
                    bool hasLeftSymbol = match.Index - 1 >= 0 && currentLine[match.Index - 1] != '.';
                    bool hasRightSymbol = match.Index + match.Length < currentLine.Length && currentLine[match.Index + match.Length] != '.';
                    if (hasLeftSymbol || hasRightSymbol)
                    {
                        sumOfPartNumbers += int.Parse(match.Value);
                        continue;
                    }
                }
            }

            return sumOfPartNumbers.ToString();
        }

        public static string GetPart2Answer(TextReader reader)
        {
            int sumOfGearRatios = 0;
            List<string> schematicLines = ReadLinesIntoList(reader);

            // Go through each line in the schematic.
            for (int y = 0; y < schematicLines.Count; y++)
            {
                string currentLine = schematicLines[y];

                // Look for asterisks (representing a gear) in the current line.
                int indexOfGear = currentLine.IndexOf('*');
                while (indexOfGear >= 0)
                {
                    int partNumbersProduct = 1;
                    int partNumbersFound = 0;

                    // Figure out the start and end index of where the digit of a part number
                    // must be contained in for us to consider it as a part associated with the gear.
                    int startIndex = Math.Max(0, indexOfGear - 1);
                    int endIndex = Math.Min(indexOfGear + 1, currentLine.Length - 1);

                    // Look around the gear to see if we can find part numbers. Note that a gear
                    // is only valid if there are _exactly_ two part numbers--no more and no less.
                    if (y - 1 >= 0)
                    {
                        // Read the previous line and look for nearby part numbers using
                        // regular expression.
                        string previousLine = schematicLines[y - 1];
                        IEnumerable<Match> adjacentPartNumbers = Digits().Matches(previousLine)
                            .Where(m => IsPartNumberMatchInRange(m, startIndex, endIndex))
                            .Cast<Match>();
                        foreach (var match in adjacentPartNumbers)
                        {
                            partNumbersFound++;
                            partNumbersProduct *= int.Parse(match.Value);
                        }
                    }

                    if (y + 1 < schematicLines.Count)
                    {
                        // Read the next line and look for nearby part numbers using
                        // regular expression.
                        string nextLine = schematicLines[y + 1];
                        IEnumerable<Match> adjacentPartNumbers = Digits().Matches(nextLine)
                            .Where(m => IsPartNumberMatchInRange(m, startIndex, endIndex))
                            .Cast<Match>();
                        foreach (var match in adjacentPartNumbers)
                        {
                            partNumbersFound++;
                            partNumbersProduct *= int.Parse(match.Value);
                        }
                    }

                    {
                        // Now look at the current line and look for adjacent part number around the gear.
                        IEnumerable<Match> adjacentPartNumbers = Digits().Matches(currentLine)
                            .Where(m => IsPartNumberMatchInRange(m, startIndex, endIndex))
                            .Cast<Match>();
                        foreach (var match in adjacentPartNumbers)
                        {
                            partNumbersFound++;
                            partNumbersProduct *= int.Parse(match.Value);
                        }
                    }

                    // Make sure there are only two numbers. If there aren't, this isn't a valid gear.
                    if (partNumbersFound == 2)
                    {
                        sumOfGearRatios += partNumbersProduct;
                    }

                    indexOfGear = currentLine.IndexOf('*', indexOfGear + 1);
                }
            }

            return sumOfGearRatios.ToString();
        }

        private static List<string> ReadLinesIntoList(TextReader reader)
        {
            List<string> lines = [];
            while (true)
            {
                string? currentLine = reader.ReadLine();
                if (currentLine == null)
                {
                    break;
                }

                lines.Add(currentLine);
            }

            return lines;
        }

        private static bool IsPartNumberMatchInRange(Match partNumber, int startIndex, int endIndex)
        {
            int partNumberStartIndex = partNumber.Index;
            int partNumberEndIndex = partNumber.Index + partNumber.Length - 1;

            // Is the part number trailing out to the left of the gear? For example, for these
            // cases ('x' indicating adjacent to the '*', but no number there).
            // 123xx.
            // 123*x.
            // .123x.
            if (partNumberStartIndex <= startIndex && partNumberEndIndex >= startIndex)
            {
                return true;
            }
            // Is the part number trailing out to the right of the gear? For example, for these
            // cases ('x' indicating adjacent to the '*', but no number there).
            // .xx123
            // .x*123
            // .x123.
            else if (partNumberStartIndex <= endIndex && partNumberEndIndex >= endIndex)
            {
                return true;
            }
            // Is the part number adjacent to the gear, without trailing out? For example, for
            // these cases ('x' indicating adjacent to the '*', but no number there).
            // .123.
            // .x*5.
            // .12x.
            else if (partNumberStartIndex >= startIndex && partNumberEndIndex <= endIndex)
            {
                return true;
            }

            // No other cases, return false.
            return false;
        }

        [GeneratedRegex(@"\d+")]
        private static partial Regex Digits();
    }
}
