// <copyright file="Day5.cs" company="rclim95">
// Copyright (c) rclim95
//
// Licensed under the MIT License. For more info, see the LICENSE.md at the root of this repo.
// </copyright>
using System.Text.RegularExpressions;
using Spectre.Console;
using InputRule = (long Start, long End, long Offset);

namespace AdventOfCode.Problems.Days.Day05;

/// <summary>
/// Provides the solution for Day 5 of Advent of Code.
/// </summary>
internal class Day05 : IPuzzle
{
    /// <inheritdoc />
    public static int Day => 5;

    /// <inheritdoc />
    public static string GetPart1Answer(StreamReader reader)
    {
        // The first line should contain the seeds that we're mapping. The line should be
        // in the format of the following:
        // seeds: ## ## ## ##
        string? seedLine = reader.ReadLine();
        ArgumentException.ThrowIfNullOrWhiteSpace(seedLine, nameof(reader));

        // NB: We skip 1 because the first element would be the token "seeds:"
        IEnumerable<long> seeds = seedLine.Split(' ').Skip(1).Select(long.Parse);

        // Get our mappings in the form of a LinkedList (which will be "sorted" based on the X to Y
        // provided in the input file).
        LinkedList<Mapping> mappings = GetMappings(reader);

        // Now go through each seed and get their locations!
        List<(long Seed, long Location)> seedToLocations = [];
        foreach (long seed in seeds)
        {
            // NB: value starts off as the seed, but after going through our mappings, it'll be
            // the location.
            long value = seed;
            for (LinkedListNode<Mapping>? currentMapping = mappings.First;
                currentMapping != null;
                currentMapping = currentMapping.Next)
            {
                value = currentMapping.Value.Map(value);
            }

            seedToLocations.Add((seed, value));
        }

        // We want the smallest location number.
        return seedToLocations
            .OrderBy(sl => sl.Location)
            .First()
            .Location
            .ToString();
    }

    /// <inheritdoc />
    public static string GetPart2Answer(StreamReader reader)
    {
        // Surprise! The first line is actually describing a seed range! The first number is the starting
        // seed number and the second number after it is the length of the range.
        string? seedLine = reader.ReadLine();
        ArgumentException.ThrowIfNullOrWhiteSpace(seedLine, nameof(reader));

        List<(long Start, long End)> seedRanges = seedLine.Split(' ')
            .Skip(1)                           // NB: We skip 1 because the first element would be the token "seeds:"
            .Select(long.Parse)
            .Chunk(2)
            .Select(c => (Start: c[0], End: c[0] + c[1] - 1))  // NB: The tuple is the start and end seed numbers.
            .OrderBy(c => c.Start)
            .ToList();

        // Get our mappings in the form of a LinkedList (which will be "sorted" based on the X to Y
        // provided in the input file).
        LinkedList<Mapping> mappings = GetMappings(reader);
        for (LinkedListNode<Mapping>? currentMapping = mappings.First;
            currentMapping != null;
            currentMapping = currentMapping.Next)
        {
            // Go through each seed range and run it through our current mapping to get the output
            // mappings.
            List<(long OutputItemStart, long OutputItemEnd)> outputRanges = [];
            foreach ((long inputStart, long inputEnd) in seedRanges)
            {
                IEnumerable<(long OutputItemStart, long OutputItemEnd)> result =
                    currentMapping.Value.MapRange(inputStart, inputEnd);
                outputRanges.AddRange(result);
            }

            // Now that we've completed the mapping for all the ranges, set seedRanges to the
            // outputRanges we've collected and move on.
            seedRanges = outputRanges;
        }

        // At this point, seedRanges should be a collection of tuples representing a bunch of location
        // ranges. Find the smallest one and return it!
        return seedRanges.Min(r => r.Start).ToString();
    }

    private static LinkedList<Mapping> GetMappings(TextReader reader)
    {
        // Skip the next line so we're at our first mapping.
        reader.ReadLine();

        LinkedList<Mapping> mappings = new();

        // Now we need to read in all our maps (i.e., X to Y converters) and put it in order
        // (technically the input file should have it in order, but you can never be sure. :)).
        Mapping? currentMapping = null;
        while (true)
        {
            string? currentLine = reader.ReadLine();
            if (currentLine == null)
            {
                // We've read to the end of the file--break out of our loop.
                break;
            }
            else if (string.IsNullOrEmpty(currentLine) && currentMapping != null)
            {
                // We've reached an empty string--we're done parsing the current map, so add it
                // to our linked list.
                if (mappings.Count > 0)
                {
                    LinkedListNode<Mapping>? nodeToInsertMapAfter = FindOutputFromEnd(mappings, currentMapping.InputItem);
                    if (nodeToInsertMapAfter != null)
                    {
                        mappings.AddAfter(nodeToInsertMapAfter, currentMapping);
                    }
                }
                else
                {
                    mappings.AddFirst(currentMapping);
                }
            }
            else if (currentLine.EndsWith("map:"))
            {
                // We're parsing a new map. Set up the current map to reflect the new map.
                Match inputToOutputMatch = Regex.Match(currentLine, @"(?'input'\w+)-to-(?'output'\w+)");
                currentMapping = new Mapping()
                {
                    InputItem = inputToOutputMatch.Groups["input"].Value,
                    OutputItem = inputToOutputMatch.Groups["output"].Value
                };
            }
            else
            {
                // We're in the middle of parsing a mapping rule. Add it to the current map.
                currentMapping?.AddRule(currentLine);
            }
        }

        // Don't forget to add the current mapping to our list!
        if (currentMapping != null)
        {
            LinkedListNode<Mapping>? nodeToInsertMapAfter = FindOutputFromEnd(mappings, currentMapping.InputItem);
            if (nodeToInsertMapAfter != null)
            {
                mappings.AddAfter(nodeToInsertMapAfter, currentMapping);
            }
            else
            {
                mappings.AddFirst(currentMapping);
            }
        }

        return mappings;
    }

    private static LinkedListNode<Mapping>? FindOutputFromEnd(LinkedList<Mapping> mapping, string outputItem)
    {
        // NB: Start from the last node in the LinkedList. We're making the assumption that the
        // input file should have the mappings in order (i.e., first A to B, then B to C, etc., as
        // we go through the file), but we can never be sure. :)
        LinkedListNode<Mapping>? currentNode = mapping.Last;
        while (currentNode != null)
        {
            if (currentNode.Value.OutputItem == outputItem)
            {
                return currentNode;
            }

            currentNode = currentNode.Previous;
        }

        return null;
    }
}

/// <summary>
/// Represents a mapping.
/// </summary>
internal class Mapping
{
    private readonly List<InputRule> _rules = [];

    /// <summary>
    /// Gets the name of the item that this mapping takes in as a "source".
    /// </summary>
    public required string InputItem { get; init; }

    /// <summary>
    /// Gets the name of the item that this mapping spits out as a "destination".
    /// </summary>
    public required string OutputItem { get; init; }

    /// <summary>
    /// Maps a given <paramref name="inputItem" /> to its proper output item equivalent based
    /// on the mapping rules associated with this mapping.
    /// </summary>
    /// <remarks>
    /// The mapping rule works as follow:
    /// <list type="bullet">
    ///     <item>If <paramref name="inputItem" /> matches a mapping rule, the mapping rule will be used.</item>
    ///     <item>Otherwise, it is assumed the mapping is 1-to-1 (no conversion required) so <paramref name="inputItem" /> will be returned as-is.</item>
    /// </list>
    /// </remarks>
    /// <param name="inputItem">The input item (represented as an integer).</param>
    /// <returns>The output item (represented as an integer) based on the current mapping rule.</returns>
    public long Map(long inputItem)
    {
        // Loop through all our rules and see if we can a source that would include this input item.
        (long sourceStart, long sourceEnd, long offset) = _rules.FirstOrDefault(r => inputItem >= r.Start && inputItem <= r.End);
        if (sourceStart == 0 && sourceEnd == 0 && offset == 0)
        {
            // No rules applies to this item, so it's a 1-to-1 mapping.
            return inputItem;
        }

        // The offset will determine much to add or subtract from the inputItem.
        return inputItem + offset;
    }

    /// <summary>
    /// Maps multiple input items in a specific range (i.e., from <paramref name="inputItemStart" />
    /// to <paramref name="inputItemEnd" />, inclusive) to its proper output item equivalent based
    /// on the mapping rules associated with this mapping, breaking up the range into multiple
    /// subranges as needed, for each mapping rule defined.
    /// </summary>
    /// <param name="inputItemStart">The start of the input item (inclusive).</param>
    /// <param name="inputItemEnd">The end of the input item (inclusive).</param>
    /// <returns>
    /// <para>
    /// An <see cref="IEnumerable{T}" /> that will yield at a minimum one output start and end range.
    /// Multiple output ranges can be returned depending on how large the input start and end range
    /// are and the number of rules defined in the mapping.
    /// </para>
    /// <para>
    /// For example, if the input range to consider was [1, 100] and the rules defined were:
    /// <list type="bullet">
    ///     <item>For inputs [50, 70], offset them by 2 so its output will be [52, 72]</item>
    ///     <item>For inputs [71, 100], offset them by 4 so its output will be [75, 104]</item>
    /// </list>
    /// Then this method would return:
    /// <list type="bullet">
    ///     <item>[1, 49] (no mapping rule applied, so it's one-to-one.</item>
    ///     <item>[52, 72] (the first mapping rule applied for [50, 70])</item>
    ///     <item>[75, 104] (the second mapping rule applied for [71, 100])</item>
    /// </list>
    /// </para>
    /// </returns>
    public IEnumerable<(long OutputItemStart, long OutputItemEnd)> MapRange(long inputItemStart, long inputItemEnd)
    {
        // We need to keep track of the current "start" boundary of the subsets of output ranges we're
        // going to return as we break up our input range based on the current mapping rules (if
        // needed, of course).
        long currentInputRangeStart = inputItemStart;

        // Given the fact that the rule mapping list is sorted by the range it covers, we can jump
        // to the first rule where the input start range is _within_ the rule _or_ just before it.
        //
        // If we don't find any, that's probably because the input range is outside the bounds of all
        // the supported mapping ranges we found.
        int startIndex = _rules.FindIndex(r => inputItemStart <= r.End);
        if (startIndex >= 0)
        {
            for (int i = startIndex; i < _rules.Count; i++)
            {
                InputRule currentRule = _rules[i];

                // Compare the current start range of the "subset" of the input range we're working with
                // with the current rule: is it before the start?
                if (currentInputRangeStart < currentRule.Start)
                {
                    // Now, is our input range going to ever reach this rule, i.e.,
                    // inputItemEnd >= currentRule.Start? If not, then we can stop processing right
                    // here, and return the current range subset as-is (it's a 1-to-1 mapping, after
                    // all).
                    if (inputItemEnd < currentRule.Start)
                    {
                        yield return (currentInputRangeStart, inputItemEnd);
                        yield break;
                    }

                    // Otherwise, it looks like we will reach the range this rule encompasses. Since
                    // we're moving forward in our sorted rules list, this must mean that the input
                    // range [currentInputRangeStart, currentRule.Start - 1] is a 1-to-1 mapping
                    // (no rules associated with it).
                    //
                    // Break up the input range to account for this and return this 1-to-1 mapping.
                    yield return (currentInputRangeStart, currentRule.Start - 1);

                    // Set our new start to this rule. Now the currentInputRangeStart is after
                    // or equal to the start of this rule, making it ready to be *actually* processed
                    // by the current rule.
                    currentInputRangeStart = currentRule.Start;
                }

                // NB: Once we're here, we know currentInputRangeStart is greater than or equal to
                //     currentRule.Start.
                //
                // Is the end of our input range contained in this current rule?
                if (inputItemEnd <= currentRule.End)
                {
                    // It is, so that means we don't need to process any more rules for mapping. Return
                    // the final input range subset and stop.
                    yield return (currentInputRangeStart + currentRule.Offset, inputItemEnd + currentRule.Offset);
                    yield break;
                }

                // NB: If we're here, that means inputItemEnd > currentRule.End, so we need to move
                // on to the next rule. Return the input range subset that's affected by this mapping,
                // and continue.
                yield return (currentInputRangeStart + currentRule.Offset, currentRule.End + currentRule.Offset);
                currentInputRangeStart = currentRule.End + 1;
            }
        }

        // If we've reached this part, then we know we went through all of our rules. So
        // the range [currentInputRangeStart, currentRule.End] is a one-to-one mapping.
        yield return (currentInputRangeStart, inputItemEnd);
        yield break;
    }

    /// <summary>
    /// Adds a mapping rule (described as a string) to the map
    /// </summary>
    /// <param name="line">
    /// The current line describing the mapping. This should be three integers (space-separated):
    /// <list type="bullet">
    ///     <item>The first integer is assumed to be the destination starting number.</item>
    ///     <item>The second integer is assumed to be the source starting number.</item>
    ///     <item>The third integer is assumed to be the range that this mapping covers.</item>
    /// </list>
    /// </param>
    public void AddRule(string line)
    {
        long[] parts = line.Split(' ').Select(long.Parse).ToArray();

        AddRule(parts[1], parts[0], parts[2]);
    }

    /// <summary>
    /// Adds a mapping rule to the map.
    /// </summary>
    /// <param name="sourceStart">The start of the mapping for the source.</param>
    /// <param name="destinationStart">The start of the mapping for the destination.</param>
    /// <param name="range">The range that this source-destination mapping covers.</param>
    public void AddRule(long sourceStart, long destinationStart, long range)
    {
        // Figure out the end range that this rule encompasses by adding the range passed in
        // to the source start.
        long sourceEnd = sourceStart + range - 1;

        // If we think about it, we _really_ don't care what the destination start/end range are--
        // we can easily derive that by figuring out the offset. Given the fact we have the
        // sourceStart and destinationStart, if we subtract the destination from the sourceStart,
        // that'll give us the offset we need to apply to the input item to map it to the proper
        // output item. :)
        long offset = destinationStart - sourceStart;

        if (_rules.Count > 0)
        {
            // Make it easier to traverse through this list by sorting it by the start index.
            for (int i = 0; i < _rules.Count; i++)
            {
                if (sourceStart < _rules[i].Start)
                {
                    _rules.Insert(i, (sourceStart, sourceEnd, offset));
                    return;
                }
            }
        }

        _rules.Add((sourceStart, sourceEnd, offset));
    }
}
