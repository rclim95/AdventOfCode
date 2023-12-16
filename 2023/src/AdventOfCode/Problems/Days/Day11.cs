// <copyright file="Day11.cs" company="rclim95">
// Copyright (c) rclim95
//
// Licensed under the MIT License. For more info, see the LICENSE.md at the root of this repo.
// </copyright>
using System.Linq;
using System.Reflection.PortableExecutable;
using MoreLinq;
using Spectre.Console;
using Point = (long X, long Y);

namespace AdventOfCode.Problems.Days.Day11;

/// <summary>
/// Implements the solutions for Day 11 of Advent of Code.
/// </summary>
internal class Day11 : IPuzzle
{
    /// <inheritdoc />
    public static int Day => 11;

    /// <inheritdoc />
    public static string GetPart1Answer(TextReader reader)
    {
        // Calculate the sum of the distances between the galaxies pair of the universe as represented
        // in the input file.
        long result = CalculateSumOfShortestPaths(reader, 2);

        return result.ToString();
    }

    public static string GetPart2Answer(TextReader reader)
    {
        // Part 2 is basically the same as Part 1, but instead of expanding the empty rows and columns
        // of the universe by 2, we're not doing it by a million.
        long result = CalculateSumOfShortestPaths(reader, 1000000);

        return result.ToString();
    }

    private static long CalculateSumOfShortestPaths(TextReader reader, int factor)
    {
        // Parse the universe and find the unique combination of galaxies we can travel between.
        Universe universe = Universe.FromInput(reader, factor);
        IEnumerable<(Point Start, Point End)> pointPairs = GetUniquePointPairs(universe.Galaxies.Select(g => g.Coordinates).ToList());

        // The first problem wants us to figure out the shortest path necessary to reach between two
        // galaxies using a combination of up, left, right, and down movements—no diagonals. We can
        // use the Manhattan distance to figure this out.
        //
        // See: https://en.wikipedia.org/wiki/Taxicab_geometry
        long sumOfShortestPaths = 0;
        foreach ((Point start, Point end) in pointPairs)
        {
            // NB: Manhattan distance is adding the absolute value of the difference between the
            // X position of the two points and the absolute value of the difference between the Y
            // position of the two points.
            //
            // One way to think about it: How far is the first point from the second point? This would
            // be the difference between the points' X positions and the points Y positions. If we
            // add the absolute values of these two togethers, this would give us the steps we would
            // need to take (again, no diagonals, but only moving up, down, left, and right) to get
            // from the first point to to the second point.
            sumOfShortestPaths += Math.Abs(start.X - end.X) + Math.Abs(start.Y - end.Y);
        }

        return sumOfShortestPaths;
    }

    private static IEnumerable<(Point Start, Point End)> GetUniquePointPairs(IList<Point> points)
    {
        for (int i = 0; i < points.Count; i++)
        {
            for (int j = i + 1; j < points.Count; j++)
            {
                yield return (points[i], points[j]);
            }
        }
    }
}

/// <summary>
/// Encapsulates metadata about the observable universe that's described in an input file.
/// </summary>
internal class Universe
{
    /// <summary>
    /// Gets the galaxies associated with this universe.
    /// </summary>
    public required IList<Galaxy> Galaxies { get; init; }

    /// <summary>
    /// Gets the maximum number of columns in the universe.
    /// </summary>
    public long MaximumColumns { get; init; }

    /// <summary>
    /// Gets the maximum number of rows in the universe.
    /// </summary>
    public long MaximumRows { get; init; }

    /// <summary>
    /// Creates a <see cref="Universe" /> map from the provided input file.
    /// </summary>
    /// <param name="reader">The <see cref="TextReader" /> that contains the input file</param>
    /// <param name="expansionFactor">How much to expand the empty spaces in the universe. The default is <tt>2</tt>.</param>
    /// <returns>A <see cref="Universe" /> instance containing the galaxies in this universe.</returns>
    public static Universe FromInput(TextReader reader, int expansionFactor = 2)
    {
        List<Galaxy> galaxies = [];

        // Read in the galaxies from our input file, and keep track of the maximum number of rows
        // and columns.
        long maxCols = -1;
        int id = 1;
        long y = 0;
        while (true)
        {
            string? currentLine = reader.ReadLine();
            if (currentLine == null)
            {
                // No more lines to read, we're done. :)
                break;
            }

            if (maxCols < 0)
            {
                maxCols = currentLine.Length;
            }

            // A galaxy is represented with the number sign (#). Look for all number signs in the
            // current line and return the coordinates.
            int x = 0;
            do
            {
                x = currentLine.IndexOf('#', x);
                if (x != -1)
                {
                    galaxies.Add(new Galaxy(id++, x, y));
                    x++;
                }
            }
            while (x != -1);

            y++;
        }

        // Before we can return our universe, we must expand the rows and columns that don't have
        // a galaxy by one. This is apparently due to some kind of gravitational effect between
        // the galaxies, and this wasn't captured at the time our input file "observed" the galaxy
        // because of the speed of light?
        //
        // At any rate: expand the (empty) columns. Note that we need to transform the empty column
        // index to transform so that it'll increase the next empty column index appropriately.
        var columnsToExpand = Enumerable.Range(0, (int)maxCols)
            .Select(c => (long)c)
            .Except(galaxies.Select(g => g.X).Distinct())
            .Select((c, i) => c + (i * (expansionFactor - 1)))
            .ToList();
        foreach (int emptyColumnIndex in columnsToExpand)
        {
            foreach (var galaxy in galaxies.Where(g => g.X > emptyColumnIndex))
            {
                galaxy.ShiftRight(expansionFactor - 1);
            }
        }

        maxCols += columnsToExpand.Count;

        // And expand the (empty) rows.
        var rowsToExpand = Enumerable.Range(0, (int)y)
            .Select(r => (long)r)
            .Except(galaxies.Select(g => g.Y).Distinct())
            .Select((c, i) => c + (i * (expansionFactor - 1)))
            .ToList();
        foreach (int emptyRowIndex in rowsToExpand)
        {
            foreach (var galaxy in galaxies.Where(g => g.Y > emptyRowIndex))
            {
                galaxy.ShiftDown(expansionFactor - 1);
            }
        }

        y += rowsToExpand.Count;

        // Now that we've accounted for the space gravitational anomalies, return the universe.
        return new Universe()
        {
            Galaxies = galaxies,
            MaximumColumns = maxCols,
            MaximumRows = y + 1
        };
    }
}

/// <summary>
/// Encapsulates information about a single galaxy that exists in a universe.
/// </summary>
internal class Galaxy
{
    /// <summary>
    /// Initializes a new instance of the <see cref="Galaxy" /> class.
    /// </summary>
    /// <param name="id">The ID for this galaxy.</param>
    /// <param name="x">The X position of the galaxy in the universe.</param>
    /// <param name="y">The Y position of the galaxy in the universe.</param>
    public Galaxy(int id, long x, long y)
    {
        ID = id;
        X = x;
        Y = y;
    }

    /// <summary>
    /// Gets the ID of the galaxy.
    /// </summary>
    public int ID { get; init; }

    /// <summary>
    /// Gets the coordinates of the galaxy.
    /// </summary>
    public Point Coordinates => (X, Y);

    /// <summary>
    /// Gets the X position of the galaxy with respect to the universe it's in.
    /// </summary>
    public long X { get; private set; }

    /// <summary>
    /// Gets the Y position of the galaxy with respect to the universe it's in.
    /// </summary>
    public long Y { get; private set; }

    /// <summary>
    /// Shifts the galaxy to the right.
    /// </summary>
    /// <param name="amount">The amount to shift.</param>
    public void ShiftRight(long amount = 1)
    {
        X += amount;
    }

    /// <summary>
    /// Shifts the galaxy down.
    /// </summary>
    /// <param name="amount">The amount to shift.</param>
    public void ShiftDown(long amount = 1)
    {
        Y += amount;
    }
}
