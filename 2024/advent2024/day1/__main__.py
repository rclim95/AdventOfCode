from collections import Counter
import typing
from advent2024.core import run

def part1(lines):
    # Read in the two list from the provided input file.
    (list_one, list_two) = _read_lists(lines)

    # Sort it in ascending order (smallest to biggest)
    list_one.sort()
    list_two.sort()

    # Zip up our two list, which will give us the pair of numbers we want to see how far apart they
    # are so we can add it to the running sum.
    running_sum = 0
    for (number_one, number_two) in zip(list_one, list_two):
        difference = abs(number_one - number_two)
        running_sum += difference

    # The sum of all the differences we added will be the answer. :)
    return running_sum

def part2(lines):
    # Read in the two list from the provided input file.
    (list_one, list_two) = _read_lists(lines)

    # list_one contains the number we want to check and see *how many times* it appears in list_two
    # so that we can calculate its similarity score, i.e., the number we're checking in list_one 
    # multiplied by the number of time it appears in list_two.
    #
    # Let's preemptively count the number of occurrence of each number in list_two so we can have
    # a lookup table with the help of the fancy Counter class:
    # https://docs.python.org/3/library/collections.html#collections.Counter
    list_two_counter = Counter(list_two)

    # Go through the numbers we want to look up, calculate its similarity score, and store it in a
    # running sum.
    running_sum = 0
    for number in list_one:
        running_sum += number * list_two_counter[number]
    
    # The sum of the calculated similarity score we added will be the answer. :)
    return running_sum

def _read_lists(lines: typing.TextIO) -> typing.Tuple[typing.List[int], typing.List[int]]:
    # Read in the two lists that we're expecting. The input file should contain two numbers 
    # separated by a space. The first number belongs to the first list and the second number belongs
    # to the second list.
    list_one = []
    list_two = []
    for line in lines:
        (number_one, number_two) = line.split(maxsplit=2)
        list_one.append(int(number_one))
        list_two.append(int(number_two))
    
    return (list_one, list_two)

run(__package__, part1, part2)
