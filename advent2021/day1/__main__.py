import os
import sys

SCRIPT_DIR = os.path.dirname(__file__)
INPUT_FILE = "input.txt"

def part1():
    # Read the input file
    with open(os.path.join(SCRIPT_DIR, INPUT_FILE), "r") as input_fp:
        previous_measurement = None
        increase_count = 0
        for line in input_fp:
            current_measurement = int(line)

            # Has the current line increase (compared to the last measurement)?
            # NOTE: We also check to see if previous_measurement is None (indicates that this
            # is the first measurement we're looking at, so there's no previous to compare to)
            if previous_measurement is not None and previous_measurement < current_measurement:
                # Increased! Count up.
                increase_count += 1

            # Set the current one as our previous for the next scan. :)
            previous_measurement = current_measurement

        # Print our result :)
        print(increase_count)

def part2():
    # Read the input file
    with open(os.path.join(SCRIPT_DIR, INPUT_FILE), "r") as input_fp:
        previous_measurement = None
        increase_count = 0
        window = []

        # Now we're reading in groups of three (a three-measurement window) that is all summed
        # together. :-)
        for line in input_fp:
            if len(window) < 3:
                # Append the number until we reached three (we're still building our summation window) :)
                window.append(int(line))
                continue

            # We've build our window! First, sum it.
            current_measurement = sum(window)

            # Then, compare with the previous measurement (if we can't, ignore)
            if previous_measurement is not None and previous_measurement < current_measurement:
                # It's an increase, so go ahead and increment. :)
                increase_count += 1


            # Set the current to be our previous (in preparation for the next), shift our list, and
            # insert the next line we've read (so we can sum it later)
            previous_measurement = current_measurement
            (window[0], window[1], window[2]) = (window[1], window[2], int(line))

        # Don't forget our last window!
        current_measurement = sum(window)
        if previous_measurement < current_measurement:
            increase_count += 1

        # Print our result :-)
        print(increase_count)


if __name__ == "__main__":
    if len(sys.argv) > 1:
        part2()
    else:
        part1()