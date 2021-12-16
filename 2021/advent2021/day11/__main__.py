import sys
import typing
from advent2021.core import run

def run_part1(file: typing.TextIO) -> int:
    MAX_STEPS = 100

    # Read the energy level of the octopuses in our 10 x 10 grid. Like other problems,
    # the inner list contains a list of integer (each integer representing the energy level of the
    # octopus) and the outer list is a list of lists (each list representing a row of octopuses).
    octopuses = []
    for row in file:
        octopuses.append([int(energy) for energy in row if energy.isnumeric()])

    # Now that we've read our initial energy level, let's go through the "steps".
    total_flashes = 0
    for step in range(MAX_STEPS):
        # Increment the energy level of each octopus by 1 and determine if the octopus will
        # "flash". An octopus will flash if its energy level is greater than 9.
        #
        # Note that adjacent octopuses (including diagonals)  will have their energy level increased
        # if this octopus flash. *And* because of that, it's possible that those adjacent octopuses
        # will flash.... Therefore, we should queue up all of the octopuses we're expecting to flash
        # so we handle for "adjacent" flashes too. :)
        flash_queue = []
        for row in range(len(octopuses)):
            for col in range(len(octopuses[row])):
                octopuses[row][col] += 1
                if octopuses[row][col] > 9:
                    # This octopus is going to flash, queue it up. :)
                    flash_queue.append((row, col))

        # Now go through our queue and start flashing (i.e., increase energy level of adjacent
        # octopuses and handle them [in case they flash as well])
        while len(flash_queue) > 0:
            (row, col) = flash_queue.pop(0)

            # This octopus's energy level is greater than 9. Go ahead and charge up all
            # adjacent octopuses (note that some of these cardinal directions can actually be None
            # instead of an energy level, in case the octopus exists near the edge of the grid).
            #
            # Once charged, check its energy level--if it's a candidate for being charged up, queue
            # it up as well. :)
            octo_nw = __charge_octopus_at(octopuses, row - 1, col - 1)
            octo_n = __charge_octopus_at(octopuses, row - 1, col)
            octo_ne = __charge_octopus_at(octopuses, row - 1, col + 1)
            octo_w = __charge_octopus_at(octopuses, row, col - 1)
            octo_e = __charge_octopus_at(octopuses, row, col + 1)
            octo_sw = __charge_octopus_at(octopuses, row + 1, col - 1)
            octo_s = __charge_octopus_at(octopuses, row + 1, col)
            octo_se = __charge_octopus_at(octopuses, row + 1, col + 1)

            if octo_nw is not None and octo_nw > 9:
                flash_queue.append((row - 1, col - 1))
            if octo_n is not None and octo_n > 9:
                flash_queue.append((row - 1, col))
            if octo_ne is not None and octo_ne > 9:
                flash_queue.append((row - 1, col + 1))
            if octo_w is not None and octo_w > 9:
                flash_queue.append((row, col - 1))
            if octo_e is not None and octo_e > 9:
                flash_queue.append((row, col + 1))
            if octo_sw is not None and octo_sw > 9:
                flash_queue.append((row + 1, col - 1))
            if octo_s is not None and octo_s > 9:
                flash_queue.append((row + 1, col))
            if octo_se is not None and octo_se > 9:
                flash_queue.append((row + 1, col + 1))

        # Now that we're done "flashing", reset all octopuses whose energy level is greater than 9
        # to 0 as they've used up their energy. :)
        #
        # Note that we're interested in the number of flashes that occurred, so keep a running
        # total of that.
        for row in range(len(octopuses)):
            for col in range(len(octopuses[row])):
                this_octo = octopuses[row][col]
                if this_octo > 9:
                    total_flashes += 1
                    octopuses[row][col] = 0

    for row in range(len(octopuses)):
        for col in range(len(octopuses[row])):
            this_octo = octopuses[row][col]
            print(this_octo, end="")
        print()
    return total_flashes

def run_part2(file: typing.TextIO) -> int:
    # Read the energy level of the octopuses in our 10 x 10 grid. Like other problems,
    # the inner list contains a list of integer (each integer representing the energy level of the
    # octopus) and the outer list is a list of lists (each list representing a row of octopuses).
    octopuses = []
    for row in file:
        octopuses.append([int(energy) for energy in row if energy.isnumeric()])
    total_octopuses = len(octopuses) * len(octopuses[0])

    # Now that we've read our initial energy level, let's go through the "steps".
    step = 1
    while True:
        # Increment the energy level of each octopus by 1 and determine if the octopus will
        # "flash". An octopus will flash if its energy level is greater than 9.
        #
        # Note that adjacent octopuses (including diagonals)  will have their energy level increased
        # if this octopus flash. *And* because of that, it's possible that those adjacent octopuses
        # will flash.... Therefore, we should queue up all of the octopuses we're expecting to flash
        # so we handle for "adjacent" flashes too. :)
        flash_queue = []
        for row in range(len(octopuses)):
            for col in range(len(octopuses[row])):
                octopuses[row][col] += 1
                if octopuses[row][col] > 9:
                    # This octopus is going to flash, queue it up. :)
                    flash_queue.append((row, col))

        # Now go through our queue and start flashing (i.e., increase energy level of adjacent
        # octopuses and handle them [in case they flash as well])
        while len(flash_queue) > 0:
            (row, col) = flash_queue.pop(0)

            # This octopus's energy level is greater than 9. Go ahead and charge up all
            # adjacent octopuses (note that some of these cardinal directions can actually be None
            # instead of an energy level, in case the octopus exists near the edge of the grid).
            #
            # Once charged, check its energy level--if it's a candidate for being charged up, queue
            # it up as well. :)
            octo_nw = __charge_octopus_at(octopuses, row - 1, col - 1)
            octo_n = __charge_octopus_at(octopuses, row - 1, col)
            octo_ne = __charge_octopus_at(octopuses, row - 1, col + 1)
            octo_w = __charge_octopus_at(octopuses, row, col - 1)
            octo_e = __charge_octopus_at(octopuses, row, col + 1)
            octo_sw = __charge_octopus_at(octopuses, row + 1, col - 1)
            octo_s = __charge_octopus_at(octopuses, row + 1, col)
            octo_se = __charge_octopus_at(octopuses, row + 1, col + 1)

            if octo_nw is not None and octo_nw > 9:
                flash_queue.append((row - 1, col - 1))
            if octo_n is not None and octo_n > 9:
                flash_queue.append((row - 1, col))
            if octo_ne is not None and octo_ne > 9:
                flash_queue.append((row - 1, col + 1))
            if octo_w is not None and octo_w > 9:
                flash_queue.append((row, col - 1))
            if octo_e is not None and octo_e > 9:
                flash_queue.append((row, col + 1))
            if octo_sw is not None and octo_sw > 9:
                flash_queue.append((row + 1, col - 1))
            if octo_s is not None and octo_s > 9:
                flash_queue.append((row + 1, col))
            if octo_se is not None and octo_se > 9:
                flash_queue.append((row + 1, col + 1))

        # Now that we're done "flashing", reset all octopuses whose energy level is greater than 9
        # to 0 as they've used up their energy. :)
        #
        # Note that we're interested when the flashes are *synchronized*. To do that, keep a track
        # of the number of time we're flashing for this step. If the number of flashes equal the
        # number of octopuses we've read in, then that's when we know we just encountered a
        # synchronized flash. :)
        total_flashes = 0
        for row in range(len(octopuses)):
            for col in range(len(octopuses[row])):
                this_octo = octopuses[row][col]
                if this_octo > 9:
                    octopuses[row][col] = 0
                    total_flashes += 1
        if total_flashes == total_octopuses:
            # Found it! Stop now. :)
            break
        else:
            # Not it, increment our step. :)
            step += 1

    for row in range(len(octopuses)):
        for col in range(len(octopuses[row])):
            this_octo = octopuses[row][col]
            print(this_octo, end="", file=sys.stderr)
        print(file=sys.stderr)

    return step

def __charge_octopus_at(grid, row, col):
    # If the row and column exceeds the dimension of the grid, return None
    if row < 0 or row >= len(grid):
        return None
    if col < 0 or col >= len(grid[row]):
        return None

    # The octopus exists at the specified row and column. Ensure it isn't already "charged" up,
    # i.e., greater than 9. Otherwise, no need to consider it. :)
    if grid[row][col] > 9:
        return None

    # Go ahead and "charge" it up (i.e., increase its energy level) and return its new energy level.
    grid[row][col] += 1
    return grid[row][col]

run(__package__, run_part1, run_part2)
