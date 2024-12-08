import logging
import re
import typing
from advent2024.core import run

def part1(lines: typing.TextIO) -> int:
    # Read the entire input into memory. This is our "corrupted memory"
    memory = lines.read()

    # Use regular expression to extract the mul(...) instructions from memory and...
    running_sum = 0
    mul_regex = re.compile(r"mul\((?P<arg1>\d+),(?P<arg2>\d+)\)")
    
    for instruction in mul_regex.finditer(memory):
        # Extract the arguments from the mul(...) instruction, execute the instruction (i.e., multiply),
        # and store it in the running sum.
        (arg1, arg2) = (int(instruction.group("arg1")), int(instruction.group("arg2")))
        result = arg1 * arg2
        running_sum += result

    return running_sum

def part2(lines: typing.TextIO) -> int:
    # Read the entire input into memory. This is our "corrupted memory"
    memory = lines.read()

    # Use regular expression to extract the do(), don't(), and mul(...) instructions from memory
    multiply = True
    running_sum = 0
    mul_regex = re.compile(r"do\(\)|don't\(\)|mul\((?P<arg1>\d+),(?P<arg2>\d+)\)")
    for instruction in mul_regex.finditer(memory):
        # What instruction are we working with? do() tells us mul(...) instructions should be
        # processed, don't() tells us mul(...) shouldn't be processed, and mul(...) means we
        # should multiply (assuming we came across a do() earlier *or* we haven't come across
        # a don't() yet).
        match = instruction.group(0)
        logging.debug("Processing %s...", match)

        if match.startswith("don't"):
            logging.debug("Multiplication enabled!")
            multiply = False
            continue

        if match.startswith("do"):
            logging.debug("Multiplicatoin disabled!")
            multiply = True
            continue

        if not multiply:
            # Ignore, we're not supposed to multiply.
            continue

        # Extract the arguments from the mul(...) instruction, execute the instruction (i.e., multiply),
        # and store it in the running sum.
        (arg1, arg2) = (int(instruction.group("arg1")), int(instruction.group("arg2")))
        result = arg1 * arg2
        running_sum += result
    
    return running_sum

run(__package__, part1, part2)
