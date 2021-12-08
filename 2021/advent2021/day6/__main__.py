from dataclasses import dataclass
import typing
from advent2021.core import run

def run_part1(file: typing.TextIO) -> int:
    MAX_DAYS = 18

    # The input is composed of a single line--the "timer" of a lanternfish before it gives birth. :)
    ages = [int(age) for age in file.readline().strip().split(",")]

    # Go through each ages (we're making a copy of the ages in case we're modifying it to add a new
    # lanternfish into the school) until we've reached day 80
    for day in range(MAX_DAYS):
        for (index, age) in enumerate(list(ages)):
            if age == 0:
                # Time to spawn a new lanternfish with an age of 8!
                ages.append(8)

                # Reset the age back to 6.
                ages[index] = 6
            else:
                # Decrement.
                ages[index] -= 1

    print(ages)

    # How many fishes did we ended up with?
    return len(ages)

def run_part2(file: typing.TextIO) -> int:
    # Same as before, but now up to 256!
    MAX_DAYS = 256

    # The input is composed of a single line--the "timer" of a lanternfish before it gives birth. :)
    initial_ages = [int(age) for age in file.readline().strip().split(",")]

    # We can't (well, we can, but we shouldn't) use Part 1's attempt where we brute-force it because
    # that would require a lot of RAM just to store the numbers as the lanternfish *expotentially*
    # grow, so let's try something different:
    #
    # Keep track of the number of fishes that are in the ages. The age of a lanternfish can fall into
    # one of these 9 buckets: [0, 1, 2, 3, 4, 5, 6, 7, 8]. Keep track of the number of fishes
    # for each of these ages. As each day past:
    # - For fishes ages [1, 8], the fish will be "decremented" from its current age and incremented
    #   to its next age.
    # - For fishes age 0, the fish will be decremented from 0 and incremented into 8.
    # - When we reach our MAX_DAYS, the sum of all days will tell us how many fishes are there.
    age_buckets = [0 for i in range(9)]

    # Populate our age bucket with the number of fishes (that falls in the specified age within the
    # age_buckets)
    for age in initial_ages:
        age_buckets[age] += 1

    # Now go through each day.
    for day in range(MAX_DAYS):
        # And go through each age bucket and move the number of fishes from its current age down
        # (e.g., if there are 5 fishes that are age 7, those 5 fishes will now move the age 6 bucket,
        # so on, so forth for the age 6 bucket -> age 5 bucket, etc)
        age_buckets_copy = age_buckets[:]
        for age in range(len(age_buckets) - 1, -1, -1):
            if age <= 7:
                age_buckets[age] = age_buckets_copy[age + 1]

                # Zero out the number of fishes on the 8th element when we're moving the fishes
                # whose timer are currently 8 to 7.
                if age == 7:
                    age_buckets[8] = 0

            # When a fish's timer is at 0, its timer will be resetted back to 6 and a new fish
            # will spawn (with a timer of 8).
            if age == 0:
                age_buckets[6] += age_buckets_copy[0]
                age_buckets[8] += age_buckets_copy[0]

    return sum(age_buckets)

run(__package__, run_part1, run_part2)
