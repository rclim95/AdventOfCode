from dataclasses import dataclass
import math
import typing
from advent2021.core import run

def run_part1(file: typing.TextIO) -> int:
    count = 0

    # Go through each line in our input file
    for line in file:
        # We want to look at the output values (i.e., the stuff after the pipe '|' delimiter),
        # so let's focus on that.
        (_, output) = line.strip().split("|")
        values = output.strip().split(" ")

        # All right, so Part 1 is asking us to identify the output values that _would_ correspond
        # to 1, 4, 7, 8.
        #
        # In our theoretical world where the wires haven't gone... haywired, using the
        # seven-segment display in the problem, these easy numbers would correspond to:
        # 1. C, F                   (2 characters)
        # 2. A, C, F                (3 characters)
        # 4. B, C, D, F             (4 characters)
        # 8. A, B, C, D, E, F, G    (7 characters)
        #
        # So these "easy" numbers are "easy" because the character lengths are unique across
        # these numbers (i.e., no "overlaps" in characters). Therefore, count the number of
        # values whose length are 2, 3, 4, or 8 characters. :)
        for value in values:
            if len(value) in (2, 3, 4, 7):
                count += 1

    # Return the number of characters that matched the above criteria :)
    return count

def run_part2(file: typing.TextIO) -> int:
    # Go through each line in the file...
    running_sum = 0
    for line in file:
        # Now we need to figure out the mapping of the signal line (stuff before the pipe) to
        # deduce the actual output (stuff after the pipe)
        (signals, outputs) = line.strip().split("|")
        signals = signals.split(" ")
        outputs = outputs.strip().split(" ")

        # We can figure out an "initial" signal pattern maps to which number based off the above
        # deduction, so go ahead and do that (note: index is the number, value stored at the index
        # is the pattern)
        number_to_pattern = [
            None,                                                           # 0
            __first_or_default(signals, lambda signal: len(signal) == 2),   # 1
            None,                                                           # 2
            None,                                                           # 3
            __first_or_default(signals, lambda signal: len(signal) == 4),   # 4
            None,                                                           # 5
            None,                                                           # 6
            __first_or_default(signals, lambda signal: len(signal) == 3),   # 7
            __first_or_default(signals, lambda signal: len(signal) == 7),   # 8
            None                                                            # 9
        ]

        # Start deducing!
        #
        # NOTE: When the comments below are saying "X" (where X is [A, B, C, D, E, F, G]), we're
        #       assuming "X" is located as follow (from the original example for 8):
        #        aaaa
        #       b    c
        #       b    c
        #        dddd
        #       e    f
        #       e    f
        #        gggg

        # - For the "A" segment (original example), get the set difference between the signal
        #   represent 7 and the signal representing 1. The outlier (the one that exists in 7 but not
        #   1 must be the "A" segment).
        a_signal = set(number_to_pattern[7]).difference(number_to_pattern[1])

        # - Based off the above deductions, we can figure out which is 9 by looking for the signal
        #   pattern that contains the "a_signal" and the set of character that represents "4". We
        #   should only have a single difference -- that'll be the "g" segment.
        four_union_a = set(number_to_pattern[4]).union(a_signal)
        number_to_pattern[9] = __first_or_default(
            signals,
            lambda s: len(s) == 6 and len(set(s).difference(four_union_a)) == 1
        )
        g_signal = set(number_to_pattern[9]).difference(four_union_a)

        # - Now that we have figured out who's 9, we can figure out which character is the "E" segment
        #   by taking the difference between the signal pattern representing 8 and the signal pattern
        #   representing 9. What exists in 8 but not will give us the "E" segment mapping.
        e_signal = set(number_to_pattern[8]).difference(number_to_pattern[9])

        # - As 0 is built off of the pattern representing 7 (which we have) and the "E" and "G"
        #   segments (which we've deduced earlier), we can figure out who is 0 by taking a union
        #   of {number_to_patter[7], g_signal, e_signal} and iterate through the signals that are 6
        #   characters in length and taking the set difference of said signal and the union we did
        #   earlier yields 1 (which will also give us the "B" segment once we deduce who's 0).
        seven_union_g_union_e = set(number_to_pattern[7]).union(g_signal, e_signal)
        number_to_pattern[0] = __first_or_default(
            signals,
            lambda s: len(s) == 6 and len(set(s).difference(seven_union_g_union_e)) == 1
        )
        b_signal = set(number_to_pattern[0]).difference(seven_union_g_union_e)

        # - To deduce the "D" segment, get the set difference between the signal representing
        #   4 and the signal representing 1. We will have two outliers: the characters that
        #   exists in 4 but not 7, which are the "B" and "D" segments. Now that we know what is the
        #   "B" segment is, we can easily figure out which one is the "D" segment by taking
        #   a set difference of the "BD" signals with the B segment--whatever remains has to be D.
        bd_signals = set(number_to_pattern[4]).difference(number_to_pattern[1])
        d_signal = bd_signals.difference(b_signal)

        # - To figure out which pattern represents 2, take the union of
        #   {a_signal, d_signal, e_signal, g_signal} and filter out all patterns whose length
        #   is 5 and the set difference of the signal and the union is 1. Note that the result of
        #   set difference will also give us the letter that represents the "C" signal.
        a_union_d_union_e_union_g = a_signal.union(d_signal, e_signal, g_signal)
        number_to_pattern[2] = __first_or_default(
            signals,
            lambda s: len(s) == 5 and len(set(s).difference(a_union_d_union_e_union_g)) == 1
        )
        c_signal = set(number_to_pattern[2]).difference(a_union_d_union_e_union_g)

        # - To deduce the F segment, take the set difference between the signal pattern representing
        #   7 and the set {a_signal, c_signal} (which we've deduced earlier).
        f_signal = set(number_to_pattern[7]).difference(a_signal | c_signal)

        # - To figure out which pattern represents 3, figure out which set _exactly_ matches
        #   the union of {a_signal, c_signal, d_signal, f_signal, g_signal}
        number_to_pattern[3] = __first_or_default(
            signals,
            lambda s: set(s) == (a_signal | c_signal | d_signal | g_signal | f_signal)
        )

        # - At this point, we've figured out which letter maps to the "correct" segment. We
        #   can use that to figure out 5 and 6.
        number_to_pattern[5] = __first_or_default(
            signals,
            lambda s: set(s) == (a_signal | b_signal | d_signal | f_signal | g_signal)
        )
        number_to_pattern[6] = __first_or_default(
            signals,
            lambda s: set(s) == (a_signal | b_signal | d_signal | e_signal | f_signal | g_signal)
        )

        # Now that we got our mapping, let's figure out what is being outputted.
        output_number = 0
        for (i, output) in enumerate(outputs):
            # What's our decimal position we're working with? This is so we know which 10 place
            # we're supposed to put the current output at.
            ten_position = len(outputs) - i - 1

            # What is this output mapped to?
            for (number, pattern) in enumerate(number_to_pattern):
                if set(pattern) == set(output):
                    # This is a match. Use the number we're on to "build" our number.
                    output_number += (10 ** ten_position) * number
                    break

        running_sum += output_number

    return running_sum

def __first_or_default(list: typing.List[str], criteria: typing.Callable[[str], bool]):
    try:
        return next(filter(criteria, list))
    except StopIteration:
        return None

run(__package__, run_part1, run_part2)
