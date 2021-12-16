import sys
import typing
from advent2021.core import run

PAIRS = [
    ("(", ")"),
    ("{", "}"),
    ("<", ">"),
    ("[", "]")
]
BEGINNING_PAIR = [pair[0] for pair in PAIRS]
ENDING_PAIR = [pair[1] for pair in PAIRS]

def run_part1(file: typing.TextIO) -> int:
    POINTS = {
        ")": 3,
        "]": 57,
        "}": 1197,
        ">": 25137
    }

    # Go through each line and figure out whether the line is corrupted (i.e., the start and end
    # brackets don't match) or if it's incomplete (a line contains an opened start bracket that wasn't
    # closed with an ending bracket).
    points = 0
    for line in file:
        # A stack will be helpful for ensuring that our brackets are being opened and closed properly.
        # We push the opening brackets and when we come across one of the ENDING_BRAKCET, pop the
        # starting bracket we got earlier, and ensure the pair matches. :)
        opened_braces = []
        for bracket in line:
            if bracket in BEGINNING_PAIR:
                opened_braces.append(bracket)
            elif bracket in ENDING_PAIR:
                # Pop the bracket off our stack and ensure it matches the starting we got earlier
                begin_pair = opened_braces.pop()
                index = BEGINNING_PAIR.index(begin_pair)
                if PAIRS[index][1] != bracket:
                    print("Error! Expected", PAIRS[index][1], "but got", bracket, "for a total of", POINTS[bracket], "points!", file=sys.stderr)
                    points += POINTS[bracket]
                    continue

    return points

def run_part2(file: typing.TextIO) -> int:
    POINTS = {
        ")": 1,
        "]": 2,
        "}": 3,
        ">": 4
    }

    # Go through each line and figure out whether the line is corrupted (i.e., the start and end
    # brackets don't match) or if it's incomplete (a line contains an opened start bracket that wasn't
    # closed with an ending bracket).
    scores = []
    for line in file:
        # A stack will be helpful for ensuring that our brackets are being opened and closed properly.
        # We push the opening brackets and when we come across one of the ENDING_BRAKCET, pop the
        # starting bracket we got earlier, and ensure the pair matches. :)
        #
        # If the pair doesn't match, it's a corrupted line. For Part 2, we ignore them.
        corrupted = False
        opened_braces = []
        for bracket in line:
            if bracket in BEGINNING_PAIR:
                opened_braces.append(bracket)
            elif bracket in ENDING_PAIR:
                # Pop the bracket off our stack and ensure it matches the starting we got earlier
                begin_pair = opened_braces.pop()
                index = BEGINNING_PAIR.index(begin_pair)
                if PAIRS[index][1] != bracket:
                    # Corrupted--set a flag so that we don't continue processing once we
                    # exit this loop.
                    print("Corrupted! Expected", PAIRS[index][1], "but got", bracket, "so ignoring.", file=sys.stderr)
                    corrupted = True
                    break

        if corrupted:
            continue

        # Once we're here, opened_braces will contain the incomplete opening braces that needs
        # its sibling to be considered "closed". Go through them one by one...
        this_score = 0
        expected_closing = ""
        while len(opened_braces) > 0:
            # Pop the opened brace that needs its sibling
            current = opened_braces.pop()

            # Figure out its closing sibling by getting the index of the beginning brace and cross
            # referencing it with the PAIRS tuple
            index = BEGINNING_PAIR.index(current)
            closing_pair = PAIRS[index][1]
            expected_closing += closing_pair

            # Now calculate the score, which is the current score multiplied by 5, with the points
            # (defined in the POINTS dictionary) added to it.
            this_score = (this_score * 5) + POINTS[closing_pair]

        # Add it to our scores list
        print("Incomplete!", this_score, "points earned from", expected_closing)
        scores.append(this_score)

    # Per the problem, autocompleters figure out the winner by sorting all the scores found
    # and then taking the middle score (median).
    scores.sort()
    print(len(scores), scores)
    return scores[len(scores) // 2]

run(__package__, run_part1, run_part2)
