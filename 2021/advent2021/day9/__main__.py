import sys
import typing
from advent2021.core import run

def run_part1(file: typing.TextIO) -> int:
    # NOTE: The heights are being represented as a list of list of int, with the inner list
    #       representing the columns and the inner lists representing the rows.
    heights = []

    # Read all the heights from the file
    for line in file:
        heights.append([int(h) for h in line if h.isnumeric()])

    # Now go through each height and figure out which heights are the lowest (in respect to their
    # adjacent counterparts, i.e., the top, left, bottom, and right [diagonals excluded]).
    lowest_points = []
    for row in range(len(heights)):
        for col in range(len(heights[row])):
            current_height = heights[row][col]

            # NOTE: Not all directions will have an applicable adjacent direction (because that'll
            #       put us out of bound); for these inapplicable directions, assume None.
            adjacent_up = heights[row - 1][col] if row - 1 >= 0 else None
            adjacent_down = heights[row + 1][col] if row + 1 < len(heights) else None
            adjacent_left = heights[row][col - 1] if col - 1 >= 0 else None
            adjacent_right = heights[row][col + 1] if col + 1 < len(heights[row]) else None

            # Check all adjacent directions and see if it's less than the current height
            if adjacent_up is not None and current_height >= adjacent_up:
                continue
            if adjacent_down is not None and current_height >= adjacent_down:
                continue
            if adjacent_left is not None and current_height >= adjacent_left:
                continue
            if adjacent_right is not None and current_height >= adjacent_right:
                continue

            # If we've made it here, that means that the current height is _less than_ all of the
            # applicable adjacent direction.
            lowest_points.append(current_height)

    # Now we need to calculate the risk level. Per the problem, the risk level is the low point + 1.
    # Therefore, add one to the lowest points we've collected and sum them up.
    return sum(map(lambda h: h + 1, lowest_points))

def run_part2(file: typing.TextIO) -> int:
    # NOTE: The heights are being represented as a list of list of int, with the inner list
    #       representing the columns and the inner lists representing the rows.
    heights = []

    # Read all the heights from the file
    for line in file:
        heights.append([int(h) for h in line if h.isnumeric()])

    # Now go through each height and record the (row, col) coordintes of the heights that are the
    # lowest (in respect to their adjacent counterparts, i.e., the top, left, bottom, and right
    # [diagonals excluded]).
    lowest_points_coords = []
    for row in range(len(heights)):
        for col in range(len(heights[row])):
            current_height = heights[row][col]

            # NOTE: Not all directions will have an applicable adjacent direction (because that'll
            #       put us out of bound); for these inapplicable directions, assume None.
            adjacent_up = heights[row - 1][col] if row - 1 >= 0 else None
            adjacent_down = heights[row + 1][col] if row + 1 < len(heights) else None
            adjacent_left = heights[row][col - 1] if col - 1 >= 0 else None
            adjacent_right = heights[row][col + 1] if col + 1 < len(heights[row]) else None

            # Check all adjacent directions and see if it's less than the current height
            if adjacent_up is not None and current_height >= adjacent_up:
                continue
            if adjacent_down is not None and current_height >= adjacent_down:
                continue
            if adjacent_left is not None and current_height >= adjacent_left:
                continue
            if adjacent_right is not None and current_height >= adjacent_right:
                continue

            # If we've made it here, that means that the current height is _less than_ all of the
            # applicable adjacent direction.
            lowest_points_coords.append((row, col))

    # Now that we got the coordinates of the lowest points, we need to figure out the basin size
    # (w.r.t. to the lowest points) for all the lowest points. The basin "ends" once it's surrounded
    # with heights of 9's.
    basins = []
    for (row, col) in lowest_points_coords:
        basin_size = 0

        # Use a flood-fill algorithm to figure out the size of our basin.
        # https://en.wikipedia.org/wiki/Flood_fill#Moving_the_recursion_into_a_data_structure
        candidates = []
        candidates.append((row, col))
        while len(candidates) > 0:
            (current_row, current_col) = candidates.pop()
            if __is_coords_valid(heights, current_row, current_col):
                # It's inside, increase the basin size and queue up our neighbors.
                basin_size += 1
                candidates.append((current_row, current_col - 1))   # West (Left)
                candidates.append((current_row, current_col + 1))   # East (Right)
                candidates.append((current_row - 1, current_col))   # North (Up)
                candidates.append((current_row + 1, current_col))   # South (Down)

        basins.append(basin_size)

    # For fun, let's print out the heights now that they've been "marked"
    print("Basins Found:", file=sys.stderr)
    for row in heights:
        print("".join([str(r) if r is not None else "•" for r in row]), file=sys.stderr)

    # Now that we got the basin sizes, multiply the size of the three largest basins to get
    # our answer. :)
    basins.sort(reverse=True)
    return basins[0] * basins[1] * basins[2]

def __is_coords_valid(heights, row, col):
    # Ensure that the height and column doesn't exceed the dimensions of the board
    if row < 0 or row >= len(heights):
        return False
    if col < 0 or col >= len(heights[row]):
        return False

    # Now check the height at this specific (row, col). If the height is 9, it's too tall to be
    # considered part of the basin. :sweal:
    if heights[row][col] is not None and heights[row][col] < 9:
        # Mark it as None so we don't try visiting (filling) it again. :)
        heights[row][col] = None
        return True
    else:
        return False

run(__package__, run_part1, run_part2)
