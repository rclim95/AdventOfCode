// <copyright file="Day10.cs" company="rclim95">
// Copyright (c) rclim95
//
// Licensed under the MIT License. For more info, see the LICENSE.md at the root of this repo.
// </copyright>

using System.Diagnostics;
using System.Text;
using MoreLinq;

namespace AdventOfCode.Problems.Days.Day10;

/// <summary>
/// Represents the available directions when going through a pipe.
/// </summary>
public enum Direction
{
    // NB: Suppress CS1591: Missing XML comment for publicly visible type or member.
    //     These values are pretty self-explanatory. :)
#pragma warning disable CS1591
    North,
    East,
    South,
    West,
#pragma warning restore CS1591
}

/// <summary>
/// Represents the available pipe type.
/// </summary>
public enum PipeType : ushort
{
    /// <summary>
    /// This pipe is invalid.
    /// </summary>
    Invalid,

    /// <summary>
    /// This pipe is vertical (<code>|</code>).
    /// </summary>
    Vertical = '|',

    /// <summary>
    /// This pipe is horizontal (<code>-</code>).
    /// </summary>
    Horizontal = '-',

    /// <summary>
    /// This pipe is a 90-degree bend connecting north and east (<tt>L</tt>).
    /// </summary>
    NorthEast = 'L',

    /// <summary>
    /// This pipe is a 90-degree bend connecting north and west (<tt>J</tt>).
    /// </summary>
    NorthWest = 'J',

    /// <summary>
    /// This pipe is a 90-degree bend connecting south and west (<tt>7</tt>).
    /// </summary>
    SouthWest = '7',

    /// <summary>
    /// This pipe is a 90-degree bend connecting south and east (<tt>F</tt>).
    /// </summary>
    SouthEast = 'F'
}

/// <summary>
/// Implements the solutions for Day 10 of Advent of Code.
/// </summary>
internal class Day10 : IPuzzle
{
    /// <inheritdoc />
    public static int Day => 10;

    /// <inheritdoc />
    public static string GetPart1Answer(TextReader reader)
    {
        // Read the entire input file (representing our pipe map) into a list
        // as we're going to need to traverse through this entire map to understand the big picture.
        (int startX, int startY) = (0, 0);
        List<string> mapLines = new(200);
        while (true)
        {
            string? currentLine = reader.ReadLine();
            if (currentLine == null)
            {
                break;
            }

            mapLines.Add(currentLine);

            // Does this line have the start position?
            int indexOfS = currentLine.IndexOf('S');
            if (indexOfS >= 0)
            {
                // Yep, it does--record this start position. It'll become important later, as we're
                // going to need to figure out what kind of pipe the animal is in by inferring its
                // surrounding.
                (startX, startY) = (indexOfS, mapLines.Count - 1);
            }
        }

        // Figure out the start pipe based on the (StartX, StartY) provided and see where we can
        // "exit".
        Pipe startPipe = new(mapLines, startX, startY);
        var startPipeExits = startPipe.GetExits();

        // Keep track of the number of steps we've taken, the current direction we're coming from,
        // and the pipe we're traversing.
        int steps = 1;
        Pipe[] currentPipes = [startPipeExits[0].NeighborPipe, startPipeExits[1].NeighborPipe];
        Direction[] currentDirections = [startPipeExits[0].EntranceDirection, startPipeExits[1].EntranceDirection];
        while (currentPipes[0] != currentPipes[1])
        {
            // As long as the current pipes we're following through isn't the same, keep on following.
            (currentPipes[0], currentDirections[0]) = currentPipes[0].Enter(currentDirections[0]);
            (currentPipes[1], currentDirections[1]) = currentPipes[1].Enter(currentDirections[1]);
            steps++;
        }

        return steps.ToString();
    }

    /// <inheritdoc />
    public static string GetPart2Answer(TextReader reader)
    {
        throw new NotImplementedException();
    }
}

/// <summary>
/// Represents a pipe.
/// </summary>
internal class Pipe : IEquatable<Pipe?>
{
    private readonly Lazy<Pipe?> _northPipe;
    private readonly Lazy<Pipe?> _southPipe;
    private readonly Lazy<Pipe?> _eastPipe;
    private readonly Lazy<Pipe?> _westPipe;

    /// <summary>
    /// Initializes a new instance of the <see cref="Pipe" /> class.
    /// </summary>
    /// <param name="x">The X position of this pipe with respect to the provided <paramref name="map" />.</param>
    /// <param name="y">The Y position of this map with respect to the provided <paramref name="map" />.</param>
    /// <param name="map">The map that this pipe exists in.</param>
    /// <exception cref="ArgumentException">The pipe character at the specified (X, Y) position is not supported.</exception>
    public Pipe(List<string> map, int x, int y)
    {
        _northPipe = new Lazy<Pipe?>(() => CreateNeighborPipe(map, Direction.North));
        _southPipe = new Lazy<Pipe?>(() => CreateNeighborPipe(map, Direction.South));
        _eastPipe = new Lazy<Pipe?>(() => CreateNeighborPipe(map, Direction.East));
        _westPipe = new Lazy<Pipe?>(() => CreateNeighborPipe(map, Direction.West));

        X = x;
        Y = y;

        char pipeLegend = map[y][x];
        if (pipeLegend == 'S')
        {
            // This is a starting pipe. We need to do more work to figure out what kind of pipe it is.
            Type = GetStartPipeType(map, x, y);
        }
        else if (Enum.IsDefined((PipeType)pipeLegend) && ((PipeType)pipeLegend) != PipeType.Invalid)
        {
            // This is a valid pipe legend.
            Type = (PipeType)pipeLegend;
        }
        else
        {
            throw new ArgumentException($"The pipe character at ({x}, {y}) is not supported.");
        }
    }

    /// <summary>
    /// Gets the pipe linked to the east side of this pipe, if one is available.
    /// </summary>
    public Pipe? East => _eastPipe.Value;

    /// <summary>
    /// Gets the pipe linked to the north side of this pipe, if one is available.
    /// </summary>
    public Pipe? North => _northPipe.Value;

    /// <summary>
    /// Gets the pipe linked to the south side of this pipe, if one is available.
    /// </summary>
    public Pipe? South => _southPipe.Value;

    /// <summary>
    /// Gets the type of pipe this pipe connection is.
    /// </summary>
    public PipeType Type { get; init; }

    /// <summary>
    /// Gets the pipe linked to the west side of this pipe, if one is available.
    /// </summary>
    public Pipe? West => _westPipe.Value;

    /// <summary>
    /// Gets the X position of this pipe with respect to the map.
    /// </summary>
    public int X { get; init; }

    /// <summary>
    /// Gets the Y position of this pipe with respect to the map.
    /// </summary>
    public int Y { get; init; }

    /// <inheritdoc />
    public static bool operator ==(Pipe? left, Pipe? right) => EqualityComparer<Pipe>.Default.Equals(left, right);

    /// <inheritdoc />
    public static bool operator !=(Pipe? left, Pipe? right) => !(left == right);

    /// <inheritdoc />
    public override bool Equals(object? obj) => Equals(obj as Pipe);

    /// <inheritdoc />
    public override int GetHashCode() => HashCode.Combine(X, Y);

    /// <inheritdoc />
    public bool Equals(Pipe? other) => other is not null && X == other.X && Y == other.Y;

    /// <summary>
    /// Gets the neighboring <see cref="Pipe" /> (and the direction we're entering from) if we were
    /// to go through this pipe.
    /// </summary>
    /// <param name="from">The direction we are entering into this pipe.</param>
    /// <returns>
    /// The neighboring pipe you'll go into and the direction of the neighboring pipe you'll be
    /// entering from or <see langword="null" /> if it's not possible.
    /// </returns>
    public (Pipe NextPipe, Direction EntranceDirection) Enter(Direction from)
    {
        // NB: We're using the null-forgiving (null!) operator to elide null-checking, as these
        // should be correct. If they aren't, we got a logic error somewhere.
        //
        // NB: The directions are flipped here because with respect to returning the neighboring pipe,
        // the direction is what we expect (relative to this pipe). But in order to go into the
        // neighboring pipe, we need to come from the opposite direction, e.g., if a pipe is
        // accessible to the _north_ of this pipe, then from the neighboring pipe's perspective,
        // we're entering from the _south_ of it.
        switch (from)
        {
            case Direction.North:
                switch (Type)
                {
                    case PipeType.NorthEast:
                        return (East!, Direction.West);
                    case PipeType.NorthWest:
                        return (West!, Direction.East);
                    case PipeType.Vertical:
                        return (South!, Direction.North);
                }

                break;

            case Direction.South:
                switch (Type)
                {
                    case PipeType.SouthEast:
                        return (East!, Direction.West);
                    case PipeType.SouthWest:
                        return (West!, Direction.East);
                    case PipeType.Vertical:
                        return (North!, Direction.South);
                }

                break;

            case Direction.East:
                switch (Type)
                {
                    case PipeType.SouthEast:
                        return (South!, Direction.North);
                    case PipeType.NorthEast:
                        return (North!, Direction.South);
                    case PipeType.Horizontal:
                        return (West!, Direction.East);
                }

                break;

            case Direction.West:
                switch (Type)
                {
                    case PipeType.SouthWest:
                        return (South!, Direction.North);
                    case PipeType.NorthWest:
                        return (North!, Direction.South);
                    case PipeType.Horizontal:
                        return (East!, Direction.West);
                }

                break;
        }

        // If we're here, then we must've entered the pipe in a way that we weren't expecting.
        throw new ArgumentException("The direction provided couldn't be followed.", nameof(from));
    }

    /// <summary>
    /// Gets the available exits for this pipe, based on the <see cref="Type" /> for this pipe.
    /// </summary>
    /// <returns>
    /// An array of two elements containing the two ends you can exit from this pipe. Each element
    /// contains the neighboring <see cref="Pipe" /> and the <see cref="Direction" /> you would be
    /// coming from to enter said  neighboring pipe.
    /// </returns>
    public (Pipe NeighborPipe, Direction EntranceDirection)[] GetExits()
    {
        // NB: The directions are flipped here because with respect to returning the neighboring pipe,
        // the direction is what we expect (relative to this pipe). But in order to go into the
        // neighboring pipe, we need to come from the opposite direction, e.g., if a pipe is
        // accessible to the _north_ of this pipe, then from the neighboring pipe's perspective,
        // we're entering from the _south_ of it.
        switch (Type)
        {
            case PipeType.Vertical:
                return [
                    (North!, Direction.South),
                    (South!, Direction.North)
                ];

            case PipeType.Horizontal:
                return [
                    (East!, Direction.West),
                    (West!, Direction.East)
                ];

            case PipeType.NorthEast:
                return [
                    (North!, Direction.South),
                    (East!, Direction.West)
                ];

            case PipeType.NorthWest:
                return [
                    (North!, Direction.South),
                    (West!, Direction.East)
                 ];

            case PipeType.SouthEast:
                return [
                    (South!, Direction.North),
                    (East!, Direction.West)
                ];

            case PipeType.SouthWest:
                return [
                    (South!, Direction.North),
                    (West!, Direction.East)
                ];
        }

        // This shouldn't happen. If it does, that means our constructor failed somewhere.
        throw new ArgumentException("The pipe type is not supported.");
    }

    private static PipeType GetStartPipeType(List<string> map, int startX, int startY)
    {
        // Look around what's adjacent to the start position and see if there's a potential connection.
        bool canConnectN = startY > 0 && (PipeType)map[startY - 1][startX] is PipeType.SouthWest or PipeType.SouthEast or PipeType.Vertical;
        bool canConnectS = startY < map.Count - 1 && (PipeType)map[startY + 1][startX] is PipeType.NorthWest or PipeType.NorthEast or PipeType.Vertical;
        bool canConnectE = startX < map[startY].Length - 1 && (PipeType)map[startY][startX + 1] is PipeType.Horizontal or PipeType.SouthWest or PipeType.NorthWest;
        bool canConnectW = startX > 0 && (PipeType)map[startY][startX - 1] is PipeType.Horizontal or PipeType.SouthEast or PipeType.NorthEast;

        // Deduce what our pipe type is, based on what's connected.
        PipeType startPipeType = (canConnectN, canConnectE, canConnectS, canConnectW) switch
        {
            (true, false, true, false) => PipeType.Vertical,
            (false, true, false, true) => PipeType.Horizontal,
            (true, true, true, true) => PipeType.NorthEast,
            (true, false, false, true) => PipeType.NorthWest,
            (false, false, true, true) => PipeType.SouthWest,
            (false, true, true, false) => PipeType.SouthEast,
            _ => PipeType.Invalid
        };
        if (startPipeType == PipeType.Invalid)
        {
            throw new InvalidOperationException("The start pipe is at a position that is not supported.");
        }

        return startPipeType;
    }

    private Pipe? CreateNeighborPipe(List<string> map, Direction direction)
    {
        // Given the pipe type initialized for _this_ pipe, is the direction valid for this pipe? If not,
        // return null. Otherwise, record the coordinate we would need to go to that direction.
        int neighborX = X;
        int neighborY = Y;
        switch (direction)
        {
            case Direction.North:
                if (Type is PipeType.Horizontal or PipeType.SouthEast or PipeType.SouthWest)
                {
                    return null;
                }

                neighborY--;
                break;

            case Direction.East:
                if (Type is PipeType.Vertical or PipeType.SouthWest or PipeType.NorthWest)
                {
                    return null;
                }

                neighborX++;
                break;

            case Direction.West:
                if (Type is PipeType.Vertical or PipeType.NorthEast or PipeType.SouthEast)
                {
                    return null;
                }

                neighborX--;
                break;

            case Direction.South:
                if (Type is PipeType.Horizontal or PipeType.NorthWest or PipeType.NorthEast)
                {
                    return null;
                }

                neighborY++;
                break;
        }

        return new Pipe(map, neighborX, neighborY);
    }
}
