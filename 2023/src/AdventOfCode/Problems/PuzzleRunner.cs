﻿// <copyright file="PuzzleRunner.cs" company="rclim95">
// Copyright (c) rclim95
//
// Licensed under the MIT License. For more info, see the LICENSE.md at the root of this repo.
// </copyright>

using AdventOfCode.Problems.Days;
using Spectre.Console;

namespace AdventOfCode.Problems;

/// <summary>
/// Defines the available parts that can be executed for a puzzle.
/// </summary>
internal enum PuzzlePart
{
    /// <summary>
    /// Part 1 should be executed.
    /// </summary>
    Part1 = 1,

    /// <summary>
    /// Part 2 should be executed.
    /// </summary>
    Part2 = 2
}

/// <summary>
/// Provide a class responsible for running a <see cref="IPuzzle" /> class.
/// </summary>
internal sealed class PuzzleRunner
{
    private readonly List<PuzzleRecord> _puzzles;

    /// <summary>
    /// Initializes a new instance of the <see cref="PuzzleRunner" /> class.
    /// </summary>
    public PuzzleRunner()
    {
        _puzzles = [];

        RegisterPuzzle<Day1>();
        RegisterPuzzle<Day2>();
        RegisterPuzzle<Day3>();
        RegisterPuzzle<Day4>();
        RegisterPuzzle<Day5>();
        RegisterPuzzle<Day6>();
        RegisterPuzzle<Day7>();
        RegisterPuzzle<Day8>();
        RegisterPuzzle<Day9>();
    }

    /// <summary>
    /// Gets the available days that the runner supports.
    /// </summary>
    public IReadOnlyCollection<int> Days => _puzzles.Select(p => p.Day).ToList();

    /// <summary>
    /// Solves a specific part of the puzzle for a particular day.
    /// </summary>
    /// <param name="day">The particular day.</param>
    /// <param name="part">The particular part.</param>
    /// <param name="inputFile">The input file that should be passed to the part to solve.</param>
    /// <returns>The output of the part.</returns>
    public string Run(int day, PuzzlePart part, StreamReader inputFile)
    {
        var selectedPuzzle = _puzzles.FirstOrDefault(p => p.Day == day);
        if (selectedPuzzle == null)
        {
            throw new ArgumentException($"Day {day} is not supported.", nameof(day));
        }

        switch (part)
        {
            case PuzzlePart.Part1:
                return selectedPuzzle.Part1(inputFile);

            case PuzzlePart.Part2:
                return selectedPuzzle.Part2(inputFile);

            default:
                throw new ArgumentException($"Part {part} is not supported.", nameof(part));
        }
    }

    private void RegisterPuzzle<TPuzzle>()
        where TPuzzle : IPuzzle
    {
        _puzzles.Add(new PuzzleRecord(
            TPuzzle.Day,
            TPuzzle.GetPart1Answer,
            TPuzzle.GetPart2Answer));
    }
}
