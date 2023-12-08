// <copyright file="Day7Tests.cs" company="rclim95">
// Copyright (c) rclim95
//
// Licensed under the MIT License. For more info, see the LICENSE.md at the root of this repo.
// </copyright>

using AdventOfCode.Problems.Days;
using static AdventOfCode.Problems.Days.Hand;

namespace AdventOfCode.Tests.Day7;

public class HandTests
{
    [Fact]
    public void TestHighCardHandType()
    {
        Hand hand = new()
        {
            Cards = [Card.Ace, Card.Four, Card.Five, Card.Jack, Card.Eight]
        };

        HandType actualHandType = hand.Type;

        Assert.Equal(HandType.HighCard, actualHandType);
    }

    [Fact]
    public void TestHighCardHandTypeWithJoker()
    {
        Hand hand = new()
        {
            Cards = [Card.Ace, Card.Four, Card.Five, Card.Joker, Card.Eight]
        };

        HandType actualHandType = hand.Type;

        Assert.Equal(HandType.OnePair, actualHandType);
    }

    [Fact]
    public void TestHighCardHandTypeWithTwoJokers()
    {
        Hand hand = new()
        {
            Cards = [Card.Ace, Card.Four, Card.Five, Card.Joker, Card.Joker]
        };

        HandType actualHandType = hand.Type;

        Assert.Equal(HandType.ThreeOfAKind, actualHandType);
    }

    [Fact]
    public void TestOnePairHandType()
    {
        Hand hand = new()
        {
            Cards = [Card.Ace, Card.Two, Card.Three, Card.Ace, Card.Four]
        };

        HandType actualHandType = hand.Type;

        Assert.Equal(HandType.OnePair, actualHandType);
    }

    [Fact]
    public void TestOnePairHandTypeWithJoker()
    {
        Hand hand = new()
        {
            Cards = [Card.Ace, Card.Two, Card.Joker, Card.Ace, Card.Four]
        };

        HandType actualHandType = hand.Type;

        Assert.Equal(HandType.ThreeOfAKind, actualHandType);
    }

    [Fact]
    public void TestTwoPairHandType()
    {
        Hand hand = new()
        {
            Cards = [Card.Jack, Card.Two, Card.Jack, Card.Two, Card.Four]
        };

        HandType actualHandType = hand.Type;

        Assert.Equal(HandType.TwoPair, actualHandType);
    }

    [Fact]
    public void TestTwoPairHandTypeWithJoker()
    {
        Hand hand = new()
        {
            Cards = [Card.King, Card.Two, Card.King, Card.Two, Card.Joker]
        };

        HandType actualHandType = hand.Type;

        Assert.Equal(HandType.FullHouse, actualHandType);
    }

    [Fact]
    public void TestThreeOfAKindHandType()
    {
        Hand hand = new()
        {
            Cards = [Card.Queen, Card.Five, Card.Queen, Card.Two, Card.Queen]
        };

        HandType actualHandType = hand.Type;

        Assert.Equal(HandType.ThreeOfAKind, actualHandType);
    }

    [Fact]
    public void TestThreeOfAKindHandTypeWithJoker()
    {
        Hand hand = new()
        {
            Cards = [Card.Queen, Card.Five, Card.Queen, Card.Joker, Card.Queen]
        };

        HandType actualHandType = hand.Type;

        Assert.Equal(HandType.FourOfAKind, actualHandType);
    }

    [Fact]
    public void TestFullHouseHandType()
    {
        Hand hand = new()
        {
            Cards = [Card.Queen, Card.King, Card.Queen, Card.King, Card.Queen]
        };

        HandType actualHandType = hand.Type;

        Assert.Equal(HandType.FullHouse, actualHandType);
    }

    [Fact]
    public void TestFullHouseHandTypeWithJoker()
    {
        Hand hand = new()
        {
            Cards = [Card.Queen, Card.King, Card.Queen, Card.Joker, Card.Queen]
        };

        HandType actualHandType = hand.Type;

        Assert.Equal(HandType.FourOfAKind, actualHandType);
    }

    [Fact]
    public void TestFourOfAKindHandType()
    {
        Hand hand = new()
        {
            Cards = [Card.King, Card.King, Card.King, Card.Queen, Card.King]
        };

        HandType actualHandType = hand.Type;

        Assert.Equal(HandType.FourOfAKind, actualHandType);
    }

    [Fact]
    public void TestFourOfAKindHandTypeWithJoker()
    {
        Hand hand = new()
        {
            Cards = [Card.King, Card.King, Card.King, Card.Joker, Card.King]
        };

        HandType actualHandType = hand.Type;

        Assert.Equal(HandType.FiveOfAKind, actualHandType);
    }

    [Fact]
    public void TestFiveOfAKindHandType()
    {
        Hand hand = new()
        {
            Cards = [Card.Eight, Card.Eight, Card.Eight, Card.Eight, Card.Eight]
        };

        HandType actualHandType = hand.Type;

        Assert.Equal(HandType.FiveOfAKind, actualHandType);
    }

    [Fact]
    public void TestFiveOfAKindHandTypeWithJoker()
    {
        Hand hand = new()
        {
            Cards = [Card.Joker, Card.Joker, Card.Joker, Card.Joker, Card.Joker]
        };

        HandType actualHandType = hand.Type;

        Assert.Equal(HandType.FiveOfAKind, actualHandType);
    }

    [Fact]
    public void TestStrongerHandByType()
    {
        // This hand is weak because it's only high cards (all distinct cards).
        Hand weakHand = new()
        {
            Cards = [Card.Two, Card.Three, Card.Four, Card.Five, Card.Six]
        };

        // This hand is strong because it's five of a kind (five same cards)
        Hand strongHand = new()
        {
            Cards = [Card.Ace, Card.Ace, Card.Ace, Card.Ace, Card.Ace]
        };

        bool isStronger = strongHand.IsStrongerThan(weakHand);
        bool isWeaker = !weakHand.IsStrongerThan(strongHand);

        Assert.True(isStronger);
        Assert.True(isWeaker);
    }

    [Fact]
    public void TestStrongerHandByRank()
    {
        // This hand is weak because the third card is only a 3 (despite being a full house).
        Hand weakHand = new()
        {
            Cards = [Card.Two, Card.Five, Card.Three, Card.Seven, Card.Nine]
        };

        // This hand is strong because the third card is a 4 (and it's also a full house).
        Hand strongHand = new()
        {
            Cards = [Card.Two, Card.Five, Card.Ace, Card.Seven, Card.Queen]
        };

        bool sameHandType = strongHand.Type == weakHand.Type;
        bool isStronger = strongHand.IsStrongerThan(weakHand);
        bool isWeaker = !weakHand.IsStrongerThan(strongHand);

        Assert.True(sameHandType);
        Assert.True(isStronger);
        Assert.True(isWeaker);
    }

    [Fact]
    public void TestStrongerHandByRankWithJoker()
    {
        // Both of these hands are four-of-a-kind, however the weakHand has a joker, and joker's values
        // are less than all other cards.
        Hand strongHand = new()
        {
            Cards = [Card.Queen, Card.Queen, Card.Queen, Card.Queen, Card.Two]
        };

        // This hand is strong because the third card is a 4 (and it's also a full house).
        Hand weakHand = new()
        {
            Cards = [Card.Joker, Card.King, Card.King, Card.King, Card.Four]
        };

        bool sameHandType = strongHand.Type == weakHand.Type;
        bool isStronger = strongHand.IsStrongerThan(weakHand);
        bool isWeaker = !weakHand.IsStrongerThan(strongHand);

        Assert.True(sameHandType);
        Assert.True(isStronger);
        Assert.True(isWeaker);
    }
}
