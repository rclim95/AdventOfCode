from itertools import pairwise
import typing
from advent2024.core import run

def part1(lines: typing.TextIO):
    # Each line represents a report. Each report has a numerical level, separated by a space.
    safe_count = 0
    for report in lines:
        levels = [int(level) for level in report.split()]

        # Determine if we're increasing or not by looking at the first two levels.
        is_increasing = is_level_change_increasing(levels[0], levels[1])

        # Go through the level and determine the difference of each pair so we know whether it's 
        # safe or not. The report is considered safe if it's increasing or decreasing and the 
        # difference between two levels are between [1, 3].
        is_safe = True
        for (l1, l2) in pairwise(levels):
            # Run through the requirements of checking whether the difference is safe
            is_safe = is_level_change_safe(l1, l2, is_increasing)
            if not is_safe:
                break
        
        if is_safe:
            safe_count += 1
        
    return safe_count

def part2(lines):
    raise NotImplementedError()

def is_level_change_increasing(first_level: int, second_level: int) -> bool:
    """Determines whether the change from `first_level` to `second_level` is increasing or not.

    Returns:
        - `True` if it is increasing
        - `False` if it is decreasing
    """
    return first_level < second_level

def is_level_change_safe(from_level: int, to_level: int, is_increasing: bool) -> bool:
    """Returns true if transitioning from `from_level` to `to_level` is safe.

    The change is considered safe if:
    - If `is_increasing` is `True`, then the change from `from_level` to `to_level` increases.
    - If `is_increasing` is `False`, then the change from `from_level` to `to_level` decreases.
    - The change between `from_level` to `to_level` is between 1 to 3 (inclusive).
    """
    # Calculate the difference between from_level and to_level
    difference = to_level - from_level
    if is_increasing and difference < 0:
        # Unsafe! We're supposed to be increasing but we're actually decreasing!
        return False

    if not is_increasing and difference > 0:
        # Unsafe! We're supposed to be decreasing but we're actually increasing!
        return False

    if not is_level_change_difference_safe(from_level, to_level):
        # Unsafe! We're supposed to be increasing or decreasing between [1, 3] steps!
        return False
    
    return True

def is_level_change_difference_safe(from_level: int, to_level: int) -> bool:
    """Checks to see if the change from `from_level` to `to_level` is safe.
    
    Returns:
        - `True` if it is safe
        - `False` if it isn't.
    """
    # A safe level change occurs if the change between from_level to to_level is in steps of 1 to 3.
    # Anything outside of that change (e.g., no change, or greater than 3) is bad.
    return 1 <= abs(from_level - to_level) <= 3

run(__package__, part1, part2)
