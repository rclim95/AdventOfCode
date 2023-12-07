// <copyright file="Day6.cs" company="rclim95">
// Copyright (c) rclim95
//
// Licensed under the MIT License. For more info, see the LICENSE.md at the root of this repo.
// </copyright>

using System.Diagnostics;

namespace AdventOfCode.Problems.Days;

/// <summary>
/// Implements the solutions for Day 6 of Advent of Code.
/// </summary>
internal class Day6 : IPuzzle
{
    /// <inheritdoc />
    public static int Day => 6;

    /// <inheritdoc />
    public static string GetPart1Answer(StreamReader reader)
    {
        IEnumerable<RaceStats> races = GetRaceStats(reader);
        int runningProduct = 1;
        foreach (RaceStats race in races)
        {
            // Per the problem statements, this toy boat that you're using for this race increases
            // its speed by 1 mm/ms the longer you hold the button.
            //
            // Therefore, figure out the minimum number of milliseconds you need to hold the button
            // so that you'll beat the current record distance by looping through the possible
            // amount of times we can hold the button, and figuring out the distance the boat would've
            // moved.
            //
            // We can solve this two ways: we can take the naïve approach and loop through all the
            // possible "hold button" duration until we get a result that beats the maximum distance
            // _or_ use the quadratic formula from high school algebra. :)
            //
            // Let:
            // - h = hold duration.
            // - d = race duration (race.Duration).
            // - t = distance traveled. 
            //
            // The distance traveled can be determined as follow:
            // t = h * (d - h).
            //
            // If we factor the h, we will get:
            // t = dh * -h².
            //
            // We know that the record distance traveled (race.RecordDistance). Let's set t =
            // race.RecordDistance traveled and solve for h, i.e., the amount of milliseconds needed
            // so that the toy boat traveled that record distance. To solve for h, get t to the
            // other side and set our equation to equal 0.
            // 0 = -h² * dh - t
            //
            // Let's multiply -1 on both side so that the h² is positive, and we will get:
            // 0 = h² * -dh + t
            //
            // If we look closely, we notice that this is basically a quadratic equation! Thinking
            // back to high school algebra, we can easily use the quadratic formula to solve for h
            // and get _two_ solutions (because of that plus/minus).
            // h = ( -(-d) ± √((-d)² - 4t) ) / 2
            //
            // By doing this, we now know what is the exact h (hold time) used to reach
            // the record distance--both the minimum and maximum.
            double minHoldDistanceForRecord = (race.Duration - Math.Sqrt(Math.Pow(race.Duration, 2) - (4 * race.RecordDistance))) / 2.0;
            double maxHoldDistanceForRecord = (race.Duration + Math.Sqrt(Math.Pow(race.Duration, 2) - (4 * race.RecordDistance))) / 2.0;

            // If the doubles are integer, we need to offset it by ± 1 to get the new record. Otherwise,
            // if they are fractional, we can use ceiling/floor to round up/down.
            int minHold = double.IsInteger(minHoldDistanceForRecord) ? (int)(minHoldDistanceForRecord + 1) : (int)Math.Ceiling(minHoldDistanceForRecord);
            int maxHold = double.IsInteger(maxHoldDistanceForRecord) ? (int)(maxHoldDistanceForRecord - 1) : (int)Math.Floor(maxHoldDistanceForRecord);

            int waysToWin = (maxHold - minHold) + 1; // NB: Need to include maxHold as part of the count, hence add 1.

            // Multiply it to our winning product.
            runningProduct *= waysToWin;
        }

        return runningProduct.ToString();
    }

    /// <inheritdoc />
    public static string GetPart2Answer(StreamReader reader)
    {
        // For Part 2, apparently those spaces should be ignored (bad kerning added spaces that
        // shouldn't have been interpreted as so). The rest of the problem is the same though, so
        // use our handy quadratic formula to figure out the number of ways we can beat this (larger)
        // record.
        string duration = reader.ReadLine()?.Split(":", StringSplitOptions.TrimEntries)[1] ?? throw new ArgumentNullException(nameof(reader));
        string distance = reader.ReadLine()?.Split(":", StringSplitOptions.TrimEntries)[1] ?? throw new ArgumentNullException(nameof(reader));
        RaceStats race = new(
            Duration: long.Parse(duration.Replace(" ", string.Empty)),
            RecordDistance: long.Parse(distance.Replace(" ", string.Empty)));

        double minHoldDistanceForRecord = (race.Duration - Math.Sqrt(Math.Pow(race.Duration, 2) - (4 * race.RecordDistance))) / 2.0;
        double maxHoldDistanceForRecord = (race.Duration + Math.Sqrt(Math.Pow(race.Duration, 2) - (4 * race.RecordDistance))) / 2.0;

        long minHold = double.IsInteger(minHoldDistanceForRecord) ? (long)(minHoldDistanceForRecord + 1) : (long)Math.Ceiling(minHoldDistanceForRecord);
        long maxHold = double.IsInteger(maxHoldDistanceForRecord) ? (long)(maxHoldDistanceForRecord - 1) : (long)Math.Floor(maxHoldDistanceForRecord);

        return ((maxHold - minHold) + 1).ToString();
    }

    private static IEnumerable<RaceStats> GetRaceStats(StreamReader reader)
    {
        // The input files is basically two lines: the first line contains the duration of the
        // race and the second line contains the record distance of the race. Therefore, read in
        // both lines and parse each race column into a RaceStats class.
        int[] durations = reader.ReadLine()?.Split(" ", StringSplitOptions.RemoveEmptyEntries)
            .Skip(1)
            .Select(int.Parse)
            .ToArray() ?? throw new ArgumentNullException(nameof(reader));
        int[] distances = reader.ReadLine()?.Split(" ", StringSplitOptions.RemoveEmptyEntries)
            .Skip(1)
            .Select(int.Parse)
            .ToArray() ?? throw new ArgumentNullException(nameof(reader));

        for (int i = 0; i < durations.Length; i++)
        {
            yield return new RaceStats(durations[i], distances[i]);
        }
    }
}

/// <summary>
/// Represents the statistics of a race.
/// </summary>
/// <param name="Duration">Gets the duration of the race (in milliseconds).</param>
/// <param name="RecordDistance">Gets the record distance recorded for the race (in millimeters).</param>
internal record class RaceStats(long Duration, long RecordDistance);
