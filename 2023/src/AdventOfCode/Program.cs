// <copyright file="Program.cs" company="rclim95">
// Copyright (c) rclim95
//
// Licensed under the MIT License. For more info, see the LICENSE.md at the root of this repo.
// </copyright>

using Spectre.Console.Cli;

namespace AdventOfCode;

/// <summary>
/// Provides the main entry point for the app.
/// </summary>
internal class Program
{
    private static void Main(string[] args)
    {
        var app = new CommandApp<SolveCommand>();
        app.Configure(config =>
        {
            app.WithDescription("Run a specific problem for an Advent of Code 2023 puzzle using a specific input file.");
        });
        app.Run(args);
    }
}
