// <copyright file="Day2.cs" company="rclim95">
// Copyright (c) rclim95
//
// Licensed under the MIT License. For more info, see the LICENSE.md at the root of this repo.
// </copyright>

namespace AdventOfCode.Problems.Days;

/// <summary>
/// Provides the solution for Day 2 of Advent of Code.
/// </summary>
internal sealed class Day2 : IPuzzle
{
    /// <inheritdoc />
    public static int Day => 2;

    /// <inheritdoc />
    public static string GetPart1Answer(StreamReader reader)
    {
        const int MaximumRedCube = 12;
        const int MaximumGreenCube = 13;
        const int MaximumBlueCube = 14;

        // Go through each game and determine which are possible, i.e., the number of
        // red, green, and blue cubes found doesn't reach the specified maximum.
        int sumOfPossibleGameIds = 0;
        foreach (var game in GetGames(reader))
        {
            bool isPossibleRound = true;
            foreach (var round in game.Rounds)
            {
                if (round.TotalRedCubes > MaximumRedCube ||
                    round.TotalBlueCubes > MaximumBlueCube ||
                    round.TotalGreenCubes > MaximumGreenCube)
                {
                    isPossibleRound = false;
                    break;
                }
            }

            if (isPossibleRound)
            {
                // This round was possible, so add it to the count.
                sumOfPossibleGameIds += game.Id;
            }
        }

        return sumOfPossibleGameIds.ToString();
    }

    /// <inheritdoc />
    public static string GetPart2Answer(StreamReader reader)
    {
        int sumOfPowers = 0;
        foreach (var game in GetGames(reader))
        {
            // We're trying to figure out what is the minimum amount of cubes needed to play
            // the rounds in this game. We can easily do that by going through each round
            // and record the largest amount of cubes collected for each color.
            int maxRed = 0;
            int maxGreen = 0;
            int maxBlue = 0;
            foreach (var round in game.Rounds)
            {
                maxRed = Math.Max(maxRed, round.TotalRedCubes);
                maxGreen = Math.Max(maxGreen, round.TotalGreenCubes);
                maxBlue = Math.Max(maxBlue, round.TotalBlueCubes);
            }

            // Now we need to calculate the power of the set of cube, i.e., multiply the
            // number of red, green, and blue cubes needed to complete each round.
            int power = maxRed * maxGreen * maxBlue;

            // Add it to our running sum.
            sumOfPowers += power;
        }

        return sumOfPowers.ToString();
    }

    private static IEnumerable<Game> GetGames(StreamReader reader)
    {
        while (true)
        {
            string? currentGame = reader.ReadLine();
            if (currentGame == null)
            {
                // There are no more lines to parse, break out.
                break;
            }

            // Split up the current line by whitespace. When we do that, we know:
            // - The first token should be "Game"
            // - The second token should be the game ID follow by a colon (which we'll remove)
            // - For the remaining tokens:
            //   - The odd tokens are the number of cubes taken
            //   - The even tokens are the color of the cube taken from the previous token
            string[] tokens = currentGame.Split(' ');
            int gameId = int.Parse(tokens[1].TrimEnd(':'));

            // Now go through each ball that was taken out and then determine if we exceeded the
            // red, green, or blue limit. Note that we're skipping by two so that we're going to
            // the next token.
            List<Round> rounds = [];
            int currentRed = 0;
            int currentGreen = 0;
            int currentBlue = 0;
            for (int i = 2; i < tokens.Length; i += 2)
            {
                int currentCount = int.Parse(tokens[i]);
                string currentCube = tokens[i + 1];

                // What cube color are we working with?
                //
                // NB: We're using StartsWith() instead of equality, because the last character
                // of the current cube could be a comma (which indicates more cube colors were
                // pulled out of the current round), a semicolon (which indicates we're moving
                // on to the next round in the game), or something else (which indicates we're
                // done for this game).
                if (currentCube.StartsWith("red"))
                {
                    currentRed = currentCount;
                }
                else if (currentCube.StartsWith("green"))
                {
                    currentGreen = currentCount;
                }
                else if (currentCube.StartsWith("blue"))
                {
                    currentBlue = currentCount;
                }

                // Are we finish with this round? We can determine by checking to see if
                // currentCube ends with a semicolon.
                if (currentCube.EndsWith(';'))
                {
                    // If so, add this round into our list and reset everything.
                    rounds.Add(new Round(currentRed, currentGreen, currentBlue));
                    currentRed = 0;
                    currentGreen = 0;
                    currentBlue = 0;
                }
            }

            // Don't forget to add the cubes collected for the last round as well!
            rounds.Add(new Round(currentRed, currentGreen, currentBlue));

            // Now that we finish parsing all the rounds for this game, return it.
            yield return new Game(gameId, rounds);
        }
    }
}

internal record class Round(int TotalRedCubes, int TotalGreenCubes, int TotalBlueCubes);

internal record class Game(int Id, List<Round> Rounds);
