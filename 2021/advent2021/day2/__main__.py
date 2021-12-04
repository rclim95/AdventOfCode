import os
import sys
from advent2021.core import run

def part1(input_fp):
    # We're interested in these two numbers as we're reading each line:
    h_pos = 0   # Horizontal position
    d_pos = 0   # Depth position

    # Go through each line and figure out what the submarine is doing
    for line in input_fp:
        # Each line is represented as follow: <direction> <unit>
        (direction, unit) = line.split(" ", maxsplit=2)
        if direction == "forward":
            # Horizontal position has changed, increment based on the unit provided
            h_pos += int(unit)
        elif direction == "down":
            # Depth position has changed, increment (as we're going down)
            d_pos += int(unit)
        elif direction == "up":
            # Depth position has changed, decrement (as we're going up)
            d_pos -= int(unit)

    # Get our result (multiply!)
    return d_pos * h_pos

def part2(input_fp):
    # We're interested in these three numbers as we're reading each line:
    h_pos = 0   # Horizontal position
    d_pos = 0   # Depth position
    aim = 0     # Aim (which has a funny algorithm calculation to it, from the solution :-)

    # Go through each line and figure out what the submarine is doing
    for line in input_fp:
        # Each line is represented as follow: <direction> <unit>
        (direction, unit) = line.split(" ", maxsplit=2)
        if direction == "forward":
            # Horizontal position has changed, increment based on the unit provided
            h_pos += int(unit)

            # Depth position has changed, increment based off our current aim multiplied by
            # the unit.
            d_pos += aim * int(unit)

        elif direction == "down":
            # Our aim *increases*
            aim += int(unit)
        elif direction == "up":
            # Our aim *decreases*
            aim -= int(unit)

    # Get our result (multiply!)
    return d_pos * h_pos

run(__package__, part1, part2)
