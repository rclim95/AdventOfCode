// <copyright file="Day7.cs" company="rclim95">
// Copyright (c) rclim95
//
// Licensed under the MIT License. For more info, see the LICENSE.md at the root of this repo.
// </copyright>

using System.Diagnostics;
using System.Linq;

namespace AdventOfCode.Problems.Days.Day07;

/// <summary>
/// Represents a hand for the game of Camel Cards.
/// </summary>
[DebuggerDisplay("{DebuggerDisplay, nq}")]
public sealed class Hand : IComparable<Hand>
{
    private readonly Lazy<HandType> _handType;

    /// <summary>
    /// Initializes a new instance of the <see cref="Hand" /> class.
    /// </summary>
    public Hand()
    {
        _handType = new Lazy<HandType>(() =>
        {
            if (Cards == null || Cards.Length != 5)
            {
                throw new ArgumentException("There must be 5 cards in a hand.", nameof(Cards));
            }

            // Count the number of unique instances for the card that's part of this hand.
            var cardOccurrence = Cards
                .GroupBy(c => c)
                .Select(c => (Card: c.Key, Count: c.Count()))

                // NB: Sort by the number of card occurrence in descending order so that the
                //     card with the highest frequency appears first.
                .OrderByDescending(c => c.Count)

                // NB: Then the card in descending order so that non-jokers appear first and jokers
                //     appear second, as jokers have the smallest value.
                .ThenByDescending(c => c.Card)
                .ToList();

            // Look at the first card in the cardOccurrence list that isn't a joker so we can
            // convert the jokers to this card and make our hand stronger. This should be the card
            // that appeared the most.
            (Card Card, int Count) firstCard = cardOccurrence.FirstOrDefault(c => c.Card != Card.Joker);
            if (firstCard != default)
            {
                // Do we have jokers in this hand? If so, change the joker so that it matches the
                // firstCard so that our card has higher value in hand type, making our hand stronger. :)
                //
                // ...unless we happen to have all jokers (then this hand is as strong as it can be).
                int indexOfJoker = cardOccurrence.FindIndex(co => co.Card == Card.Joker);
                if (indexOfJoker >= 0)
                {
                    (Card _, int numberOfJokers) = cardOccurrence[indexOfJoker];
                    firstCard = (firstCard.Card, firstCard.Count + numberOfJokers);

                    // Remove the joker so that when we're figuring out what kind of hand we're working
                    // with, the assumption that relies on the number of card occurrence still works (e.g.,
                    // the full-house vs. three-of-a-kind, etc.).
                    //
                    // ..unless this is the only card, then ignore. :)
                    if (cardOccurrence.Count > 1)
                    {
                        cardOccurrence.RemoveAt(indexOfJoker);
                    }
                }
            }
            else
            {
                // The only possible way to reach this case is if our hand are all jokers (as we
                // couldn't find a non-joker card). In that case, cardOccurrence really only
                // have one items: a five-of-a-kind hand with all jokers.
                firstCard = cardOccurrence[0];
            }

            // Now determine what kind of hand we're working with.
            if (firstCard.Count == 5)
            {
                return HandType.FiveOfAKind;
            }
            else if (firstCard.Count == 4)
            {
                return HandType.FourOfAKind;
            }
            else if (firstCard.Count == 3)
            {
                // Is this a full house (the remaining two cards are the same) or is this
                // is this a three-of-a-kind (the remaining two cards are different)?
                if (cardOccurrence.Count == 2)
                {
                    // There's only two unique groups: the three cards that are the same and the
                    // two cards that are the same.
                    return HandType.FullHouse;
                }
                else
                {
                    // There must be three unique groups: the three cards that are the same,
                    // and the two _different_ cards.
                    return HandType.ThreeOfAKind;
                }
            }
            else if (firstCard.Count == 2)
            {
                // Is this a two-pair (two cards share the same rank, two other cards also share
                // the rank, and a different one) or a one-pair (two cards share the same rank, and
                // all the other pairs are different?)
                if (cardOccurrence.Count == 3)
                {
                    // There are three unique occurrences: this pair of card we checked earlier,
                    // another pair of card, and one unique card.
                    return HandType.TwoPair;
                }
                else
                {
                    // There must be four unique occurrence: this pair of card we checked earlier,
                    // and three unique cards.
                    return HandType.OnePair;
                }
            }
            else
            {
                // The only other option is that it's unique.
                return HandType.HighCard;
            }
        });
    }

    /// <summary>
    /// Defines the available cards that makes up the game of Camel Card.
    /// </summary>
    public enum Card
    {
        // NB: Suppress CS1591: Missing XML comment for publicly visible type or member.
        //     These values are pretty self-explanatory. :)
#pragma warning disable CS1591
        Joker = 1,
        Two = 2,
        Three,
        Four,
        Five,
        Six,
        Seven,
        Eight,
        Nine,
        Ten,
        Jack,
        Queen,
        King,
        Ace
#pragma warning restore CS1591
    }

    /// <summary>
    /// Defines the kind of hand types.
    /// </summary>
    public enum HandType
    {
        /// <summary>
        /// The cards that makes up this hand are all unique.
        /// </summary>
        HighCard,

        /// <summary>
        /// This hand has two cards that are the same; all other cards are different.
        /// </summary>
        OnePair,

        /// <summary>
        /// This hand has two pairs of cards that are the same; all other cards are different.
        /// </summary>
        TwoPair,

        /// <summary>
        /// This hand has three cards that are the same; all other cards are different.
        /// </summary>
        ThreeOfAKind,

        /// <summary>
        /// This hand has three cards that are the same and two cards that are the same.
        /// </summary>
        FullHouse,

        /// <summary>
        /// This hand has four cards that are the same; only one is different.
        /// </summary>
        FourOfAKind,

        /// <summary>
        /// This hand has five cards that are all the same.
        /// </summary>
        FiveOfAKind
    }

    /// <summary>
    /// Gets the five cards that makes up the hand in the order they appear.
    /// </summary>
    public required Card[] Cards { get; init; }

    /// <summary>
    /// Gets the kind of hand that this hand has, based on the provided <see cref="Cards" />.
    /// </summary>
    public HandType Type => _handType.Value;

    private string DebuggerDisplay
    {
        get
        {
            string cardAscii = string.Join(string.Empty, Cards.Select(c => c switch
            {
                Card.Ace => "A",
                Card.King => "K",
                Card.Queen => "Q",
                Card.Jack => "J",
                Card.Joker => "J",
                Card.Ten => "T",
                _ => ((int)c).ToString()
            }));

            return $"Card = {cardAscii}, Hand = {Type}";
        }
    }

    /// <summary>
    /// Creates a new <see cref="Hand" /> instance from a string.
    /// </summary>
    /// <param name="hand">The string that contains the hand to translate.</param>
    /// <param name="hasJokers">Indicates whether <code>J</code> should be treated as <see cref="Card.Joker" /> instead of <see cref="Card.Jack" />.</param>
    /// <returns>The new <see cref="Hand" /> created from the string provided.</returns>
    public static Hand FromString(string hand, bool hasJokers = false)
    {
        Card[] cards = new Card[5];
        for (int i = 0; i < hand.Length; i++)
        {
            cards[i] = hand[i] switch
            {
                'A' => Card.Ace,
                'K' => Card.King,
                'Q' => Card.Queen,
                'J' => hasJokers ? Card.Joker : Card.Jack,
                'T' => Card.Ten,
                '9' => Card.Nine,
                '8' => Card.Eight,
                '7' => Card.Seven,
                '6' => Card.Six,
                '5' => Card.Five,
                '4' => Card.Four,
                '3' => Card.Three,
                '2' => Card.Two,
                _ => throw new ArgumentException($"\"{hand[i]}\" is not a valid card.")
            };
        }

        return new Hand()
        {
            Cards = cards
        };
    }

    /// <summary>
    /// Compares two hands and return an integer indicating whether this hand precedes, follows, or
    /// occurs in the same position in the sort order as the other object.
    /// </summary>
    /// <param name="other">The other hand.</param>
    /// <returns>
    /// A negative integer if this hand precedes the other hand, 0 if the hand should
    /// be in the same position, or a positive integer if this hands follows the other hand.
    /// </returns>
    /// <seealso cref="IsStrongerThan(Hand)"/>
    public int CompareTo(Hand? other)
    {
        // If other is null, then this hand is always greater.
        if (other == null)
        {
            return 1;
        }

        if (Type == other.Type)
        {
            // We need to iterate through both hands' card one-by-one and find who's the strongest.
            for (int i = 0; i < Cards.Length; i++)
            {
                // NB: The Card enum is already sorted by the weakest to strongest, so we can easily
                // use that to compare.
                Card thisHandCard = Cards[i];
                Card otherHandCard = other.Cards[i];

                if (thisHandCard == otherHandCard)
                {
                    continue;
                }

                return thisHandCard - otherHandCard;
            }
        }

        return Type.CompareTo(other.Type);
    }

    /// <summary>
    /// Determines if this hand is stronger than another hand.
    /// </summary>
    /// <remarks>
    /// The hand's <see cref="Type" /> is used to determine whether a hand is stronger than another
    /// hand. If both hands have the same type, then the cards in each hand will be compared one by one.
    /// When a difference is detected, the hand with the stronger card rank is considered stronger.
    /// </remarks>
    /// <param name="otherHand">The other hand to compare.</param>
    /// <returns>
    /// <see langword="true" /> if this hand is stronger than the other hand, else
    /// <see langword="false" />.
    /// </returns>
    public bool IsStrongerThan(Hand otherHand)
    {
        return CompareTo(otherHand) > 0;
    }
}

/// <summary>
/// Implements the solution for Day 7 of Advent of Code.
/// </summary>
internal class Day07 : IPuzzle
{
    /// <inheritdoc />
    public static int Day => 7;

    /// <inheritdoc />
    public static string GetPart1Answer(TextReader reader)
    {
        List<(Hand Hand, int Bid)> handAndBids = GetHandAndBids(reader).ToList();

        // To figure out the ranking, use LINQ and sort by the hand itself! We can do this
        // because our Hand class implements the IComparable<Hand> interface. :)
        //
        // Then figure out the ranking in our sorted list by using Select(...) to project the index,
        // then multiply the bid and rank, sum it all up, and print our result to get our total
        // winnings!
        IEnumerable<(Hand Hand, int Bid, int Rank)> rankedHandAndBids = handAndBids
            .OrderBy(hb => hb.Hand)
            .Select((hb, i) => (Hand: hb.Hand, Bid: hb.Bid, Rank: i + 1));

        return rankedHandAndBids
            .Select(i => i.Bid * i.Rank)
            .Sum()
            .ToString();
    }

    /// <inheritdoc />
    public static string GetPart2Answer(TextReader reader)
    {
        // In a twist of events, the "J" in this part of the problem can be jokers! Jokers can
        // convert themselves to other cards that increases the strength of a deck (i.e., the hand type)
        // but their values are less than all the other cards.
        //
        // We do the same thing like Part 1, but handle the "J" a little bit differently.
        List<(Hand Hand, int Bid)> handAndBids = GetHandAndBids(reader, hasJokers: true).ToList();
        IEnumerable<(Hand Hand, int Bid, int Rank)> rankedHandAndBids = handAndBids
            .OrderBy(hb => hb.Hand)
            .Select((hb, i) => (Hand: hb.Hand, Bid: hb.Bid, Rank: i + 1));

        return rankedHandAndBids
            .Select(i => (long)i.Bid * i.Rank)
            .Sum()
            .ToString();
    }

    private static IEnumerable<(Hand Hand, int Bid)> GetHandAndBids(TextReader reader, bool hasJokers = false)
    {
        while (true)
        {
            string? currentLine = reader.ReadLine();
            if (currentLine == null)
            {
                yield break;
            }

            // Each line is composed of two tokens, space-separated: the hand (represented with
            // the a 5 character string) and the bid. Split the line into two by space, and
            // return the hand and bid.
            string[] tokens = currentLine.Split(' ');
            string hand = tokens[0];
            int bid = int.Parse(tokens[1]);

            yield return (Hand.FromString(hand, hasJokers), bid);
        }
    }
}
