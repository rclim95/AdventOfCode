// <copyright file="Day4.cs" company="rclim95">
// Copyright (c) rclim95
//
// Licensed under the MIT License. For more info, see the LICENSE.md at the root of this repo.
// </copyright>

using System.Collections.Immutable;
using System.Text.RegularExpressions;

namespace AdventOfCode.Problems.Days;

/// <summary>
/// Provides the solution for Day 4 of Advent of Code.
/// </summary>
internal sealed class Day4 : IPuzzle
{
    /// <inheritdoc />
    public static int Day => 4;

    /// <inheritdoc />
    public static string GetPart1Answer(StreamReader reader)
    {
        int sumOfPoints = 0;
        while (true)
        {
            string? currentLine = reader.ReadLine();
            if (currentLine == null)
            {
                // No more lines to read, we can stop.
                break;
            }

            // Parse the current line into a Card, and use our handy-dandy CalculatePoints()
            // to get the number of points the card is worth (based off its winning), and then 
            // sum of it.
            Card card = Card.FromInput(currentLine);
            sumOfPoints += card.CalculatePoints();
        }

        return sumOfPoints.ToString();
    }

    /// <inheritdoc />
    public static string GetPart2Answer(StreamReader reader)
    {
        Dictionary<int, int> cardToCopiesCount = new();

        while (true)
        {
            string? currentLine = reader.ReadLine();
            if (currentLine == null)
            {
                // No more lines to read, we can stop.
                break;
            }

            // Parse the current line into a Card and determine the number of matches we won.
            Card card = Card.FromInput(currentLine);
            int totalMatches = card.CalculateNumberOfMatches();

            // Now the fun part: every time you get a matching number, you win a copy of the
            // next card up, e.g., per the problem statement, if Card X yield Y matching numbers,
            // you will win one copy of Card X + 1, Card X + 2, ... Card X + Y.
            //
            // The problem also mentions that copies of a scratch card are scored like normal
            // scratch cards and have the same number as the card copied, e.g., if Card X and
            // the Y instances you've earned had 3 matching numbers, all Y instances would win
            // 3 matching numbers for those copies of Card X. This repeats until there are no
            // more copies to win.

            // To solve the problem, first get the number of copies we got for the current card ID
            // provided.
            if (!cardToCopiesCount.TryGetValue(card.Id, out int numberOfCopies))
            {
                // We haven't gotten any additional copies (except for the one we just read in,
                // i.e., the original).
                cardToCopiesCount.Add(card.Id, 1);
                numberOfCopies = 1;
            }
            else
            {
                // Don't forget to include the original we just read in as part of our
                // copies count!
                numberOfCopies++;
                cardToCopiesCount[card.Id] = numberOfCopies;
            }

            // Next, figure out which cards we've won as a result of the number of matches
            // made, and update the cardToInstanceCount to keep track of the number of instances of
            // each card we've earned from our current win (key being the card ID, value being
            // the copies/instances awarded).
            for (int i = 0; i < totalMatches; i++)
            {
                int cardIdWon = card.Id + 1 + i;

                // NB: Remember: we're adding numberOfCopies to the count because all copies
                // have the same winnings. :)
                if (cardToCopiesCount.TryGetValue(cardIdWon, out int cardIdCurrentCopies))
                {
                    // This card ID exists, add to the current count.
                    cardToCopiesCount[cardIdWon] = cardIdCurrentCopies + numberOfCopies;
                }
                else
                {
                    // This card ID wasn't added to our dictionary yet; add it to our dictionary
                    // and initialize it with the number of copies of the card we're processing.
                    cardToCopiesCount.Add(cardIdWon, numberOfCopies);
                }
            }
        }

        // How many instances we've accumulated in all?
        return cardToCopiesCount.Values.Sum().ToString();
    }
}

/// <summary>
/// Represents a scratch card.
/// </summary>
internal partial class Card
{
    /// <summary>
    /// Gets the ID for this card.
    /// </summary>
    public required int Id { get; init; }

    /// <summary>
    /// Gets the winning numbers for the card.
    /// </summary>
    public required ImmutableSortedSet<int> WinningNumbers { get; init; }

    /// <summary>
    /// Gets the number picked for the card.
    /// </summary>
    public required ImmutableSortedSet<int> PickedNumbers { get; init; }

    /// <summary>
    /// Creates a <see cref="Card" /> from the specified input line (provided in the input file).
    /// </summary>
    /// <remarks>
    /// It is assumed that <paramref name="input" /> will be in the following format:
    /// <code>
    /// Card CC: (WW )+ | (PP )+
    /// </code>
    /// Where:
    /// <list type="bullet">
    ///     <item><strong>CC</strong> represents the card number.</item>
    ///     <item><strong>WW</strong> represents the winning number for that card.</item>
    ///     <item><strong>PP</strong> represents the number that was picked for that card.</item>
    /// </list>
    /// </remarks>
    /// <param name="input">The input to parse into a card, i.e., the card number, the winning numbers, and the numbers picked.</param>
    /// <returns>A <see cref="Card" /> instance that matches the provided input.</returns>
    public static Card FromInput(string input)
    {
        // Use this _monstrous_ regular expression to parse the input provided. It'll take care
        // of splitting up the input into three parts: the card number, the winning number, and
        // the number picked.
        Match match = InputParts().Match(input);
        int cardNumber = int.Parse(match.Groups["card"].Value);
        ImmutableSortedSet<int> winningNumbers = match.Groups["win"].Captures.Cast<Capture>().Select(c => int.Parse(c.Value)).ToImmutableSortedSet();
        ImmutableSortedSet<int> pickedNumbers = match.Groups["pick"].Captures.Cast<Capture>().Select(c => int.Parse(c.Value)).ToImmutableSortedSet();

        return new Card()
        {
            Id = cardNumber,
            WinningNumbers = winningNumbers,
            PickedNumbers = pickedNumbers
        };
    }

    /// <summary>
    /// Calculate the number of matches between <see cref="PickedNumbers" /> amd
    /// <see cref="WinningNumbers" />.
    /// </summary>
    /// <returns>
    /// The number of numbers in the <see cref="PickedNumbers" /> set that ended up being
    /// a winning number in the <see cref="WinningNumbers" /> set.
    /// </returns>
    public int CalculateNumberOfMatches()
    {
        // Count the number of matching numbers. Because we're using sets, we can take figure
        // this out by taking the intersection between the WinningNumbers and PickedNumbers set.
        return PickedNumbers.Intersect(WinningNumbers).Count;
    }

    /// <summary>
    /// Calculate the amount of points earned for this card, given the winning numbers and
    /// the numbers picked.
    /// </summary>
    /// <returns>
    /// The number of points earned. A card is worth 1 point for the first match, and then
    /// the points are doubled for any remaining match (i.e., a power of 2).
    /// </returns>
    public int CalculatePoints()
    {
        int numberOfMatches = CalculateNumberOfMatches();

        if (numberOfMatches == 0)
        {
            return 0;
        }
        else
        {
            return (int)Math.Pow(2.0, (double)(numberOfMatches - 1));
        }
    }

    [GeneratedRegex(@"Card\s+(?'card'\d+): (?:\s*(?'win'\d+))+ \| (?:\s*(?'pick'\d+))+")]
    private static partial Regex InputParts();
}
