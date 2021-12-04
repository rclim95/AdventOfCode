import itertools
import os
from advent2021.core import run

SCRIPT_DIR = os.path.dirname(__file__)
INPUT_FILE = "input.txt"

def part1(input_fp):
    # Read the input file
    zeroes_count = []
    ones_count = []

    # Go through each line (which should represent a binary number)
    for line in input_fp:
        # Split our line by its individual bit
        bits = list(line.strip())

        # Are our count empty? If so, use the first line to figure out how many bits we're
        # working with. :)
        if len(zeroes_count) == 0 and len(ones_count) == 0:
            zeroes_count.extend(itertools.repeat(0, len(bits)))
            ones_count.extend(itertools.repeat(0, len(bits)))

        # Go through each bit and increment the proper count
        for (i, bit) in enumerate(bits):
            if int(bit) == 0:
                zeroes_count[i] += 1
            elif int(bit) == 1:
                ones_count[i] += 1

    # Number crunching time!
    #
    # Figure out the gamma rate by seeing which is the most common bit for each "bit position",
    # (e.g., if 1 appeared more than 0 for the first bit, we put a "1"; if 0 appeared more than 1,
    # then we put a "0", next; so far, our binary number would then be "10" in Base2, or 2 in Base10).
    #
    # For the epsilon rate, the above is still the same, except we flip it.
    gamma_rate = 0
    epsilon_rate = 0
    for (lshift_count, (zero_count, one_count)) in enumerate(zip(zeroes_count, ones_count)):
        if zero_count > one_count:
            # More zeroes were found than one.
            gamma_rate |= 0 << (len(bits) - lshift_count - 1)
            epsilon_rate |= 1 << (len(bits) - lshift_count - 1)
        else:
            # More ones were found than zeroes.
            gamma_rate |= 1 << (len(bits) - lshift_count - 1)
            epsilon_rate |= 0 << (len(bits) - lshift_count - 1)

    # Now get our result--the power consumption (multiplication of gamma and epsilon)
    return gamma_rate * epsilon_rate


def part2(input_fp):
    numbers = []

    # We're going to need to do several pasts with the file, so let's load all the binary
    # numbers into memory (have a list of tuple of bits).
    #
    # While we're add it, figure out the number of zeroes and ones for each bit position. This
    # will help us determine oxygen generator rating and the CO2 scrubber rating (as these ratings
    # are dependent on the "frequency" of 0's and 1's in each position)
    for line in input_fp:
        bits_list = list(line.strip())
        numbers.append(tuple(int(bit) for bit in bits_list))

    # How many bits are we working with? We're going to need this information later. :)
    bit_count = len(numbers[0])

    # First, figure out the oxygen generator rating. This is accomplished by:
    # 1. Look at the first bit of each binary number and determine which appears more--0's or 1's.
    # 2. Filter out binary numbers whose first bit matches the most common number (if 0's and 1's
    #    appear the same, err to the side of 1) and set that as the new list.
    # 3. With this new list, now look at the second bit and determine which appears more--0's or 1's.
    # 4. Filter out binary numbers whose second bit matches the most common number (again, if 0's
    #    and 1's appear the same, err to the side of 1) and set that as th enew list.
    # 5. Keep doing this until you've gotten exactly one number.
    filtered_numbers = list(numbers)
    oxygen_generator_rating_binary = None
    for bit_pos in range(bit_count):
        # Determine what's more frequent--0's or 1's.
        zero_count = len([number for number in filtered_numbers if number[bit_pos] == 0])
        one_count = len([number for number in filtered_numbers if number[bit_pos] == 1])
        most_common = None
        if zero_count > one_count:
            most_common = 0
        else:
            most_common = 1

        # Now build the filtered list
        filtered_numbers = [number for number in filtered_numbers if number[bit_pos] == most_common]
        if len(filtered_numbers) == 1:
            # We can stop now!
            oxygen_generator_rating_binary = filtered_numbers[0]
            break

    # Next, figure out the carbon dioxide (CO2) scrubber rating. This is the same as above, except
    # we're looking at the _least_ common number occurring at the bit position (if they happen to
    # be the same, favor 0's).
    filtered_numbers = list(numbers)
    co2_scrubber_rating = None
    for bit_pos in range(bit_count):
        # Determine what's more frequent--0's or 1's.
        zero_count = len([number for number in filtered_numbers if number[bit_pos] == 0])
        one_count = len([number for number in filtered_numbers if number[bit_pos] == 1])
        least_common = None
        if zero_count > one_count:
            least_common = 1
        else:
            least_common = 0

        # Now build the filtered list
        filtered_numbers = [number for number in filtered_numbers if number[bit_pos] == least_common]
        if len(filtered_numbers) == 1:
            # We can stop now!
            co2_scrubber_rating = filtered_numbers[0]
            break

    # Finally, figure out the life support rating of the submarine by multiplying those two numbers.
    return __bin2dec(co2_scrubber_rating) * __bin2dec(oxygen_generator_rating_binary)

def __bin2dec(bits):
    number = 0
    for (bit_pos, bit) in enumerate(bits):
        number += 2 ** (len(bits) - bit_pos - 1) * bit
    return number

run(__package__, part1, part2)
