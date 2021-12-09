from dataclasses import dataclass
import math
import typing
from advent2021.core import run

def run_part1(file: typing.TextIO) -> int:
    # Read all the positions and figure out which position we should align
    # the crabs to. :)
    best_pos = None
    positions = [int(pos) for pos in file.readline().strip().split(",")]

    # Go through each position
    for current_position in positions:
        # Find the difference beetween the current position (and all the other)
        # positions and from there, determine if this is the "best" position
        # that all the other positions should align with.
        running_sum = 0
        for other_position in positions:
            running_sum += abs(current_position - other_position)
        if best_pos is None or best_pos > running_sum:
            best_pos = running_sum

    return best_pos

def run_part2(file: typing.TextIO) -> int:
    # Read all the positions and figure out which position we should align
    # the crabs to. :)
    best_pos = None
    positions = [int(pos) for pos in file.readline().strip().split(",")]

    # Since we want to meet in the "middle" of the position (where their "fuel cost" is based off
    # 1 + 2 + ... n (where n is the number of steps taken)), the best choice for the crabs
    # to move to is the mean of all the positions.
    #
    # Note that the mean can be floor'd (round down) or ceil'd (round up). Therefore, calculate
    # the position average for floor vs. ceiling, and take the lowest of the two costs.
    position_average_floor = math.floor(sum(positions) / len(positions))
    position_average_ceil = math.ceil(sum(positions) / len(positions))

    # Go through each position
    fuel_cost_floor = 0
    fuel_cost_ceil = 0
    for current_position in positions:
        diff = abs(position_average_ceil - current_position)
        fuel_cost_ceil += (diff * (diff + 1)) // 2

        diff = abs(position_average_floor - current_position)
        fuel_cost_floor += (diff * (diff + 1)) // 2

    return min(fuel_cost_floor, fuel_cost_ceil)

run(__package__, run_part1, run_part2)
