from dataclasses import dataclass
import re
import sys
import typing
from advent2021.core import run

@dataclass
class Line:
    x1: int
    y1: int
    x2: int
    y2: int

    def __post_init__(self):
        # Figure out if we need to do some switcheroos based off the difference between x1 and x2
        # and y1 and y2.
        x_diff = self.x2 - self.x1
        y_diff = self.y2 - self.y1

        print(x_diff, y_diff, self.is_horizontal, self.is_vertical, self.is_diagonal)

        # The switcheroo will be dependent on what kind of line we're working with.
        if self.is_horizontal:
            # If the x value is negative, flip it!
            if x_diff < 0:
                (self.x1, self.x2) = (self.x2, self.x1)
        elif self.is_vertical:
            # If the y value is negative. flip it!
            if y_diff < 0:
                (self.y1, self.y2) = (self.y2, self.y1)
        else:
            # For diagonals, we want to normalize it the two directions: from upper-left to bottom-right (\),
            # or bottom-left to upper-right (/).
            if (x_diff < 0 and y_diff < 0) or (x_diff < 0 and y_diff > 0):
                (self.x1, self.y1, self.x2, self.y2) = (self.x2, self.y2, self.x1, self.y1)

    @property
    def is_horizontal(self):
        # A line is considered horizontal if y values are the same
        return self.y1 == self.y2

    @property
    def is_vertical(self):
        # A line is considered vertical if x values are the same
        return self.x1 == self.x2

    @property
    def is_diagonal(self):
        # A line is 45° diagonal if the slope of (x1, y1) and (x2, y2) are the same
        return abs(self.x1 - self.x2) == abs(self.y1 - self.y2)

    def points(self):
        # To figure out what are the points that makes up the line, we'll need to figure out
        # whether it's running horizontally, vertically, or diagonally.
        if self.is_horizontal:
            for x in range(self.x1, self.x2 + 1):
                yield (x, self.y1)
        elif self.is_vertical:
            for y in range(self.y1, self.y2 + 1):
                yield (self.x1, y)
        else:
            # Determine what kind of diagonal we're doing: the "back-slash" (\) or "forward-slash" (/)
            x_diff = self.x2 - self.x1
            y_diff = self.y2 - self.y1
            if x_diff > 0 and y_diff > 0:
                # We're doing the back-slash method.
                for i in range(x_diff + 1):
                    yield (self.x1 + i, self.y1 + i)
            else:
                # We're doing the forward-slash method
                for i in range(x_diff + 1):
                    yield (self.x1 + i, self.y1 - i)


def run_part1(file: typing.TextIO) -> int:
    # Only consider lines going horizontal or vertically (no diagonals)
    lines = [line for line in __parse_lines(file) if line.is_horizontal or line.is_vertical]

    # To figure out point where there are overlaps, go through each line and figure out all the
    # points that makes up the line. From there, store these points into a dictionary and increment
    # the number of time we come across it.
    points = {}
    for line in lines:
        # print(line, file=sys.stderr)
        for point in line.points():
            # print("", point, file=sys.stderr)
            if point not in points:
                points[point] = 1
            else:
                points[point] += 1

    # Return the number of keys whose count is greater than 1
    criteria = [point for (point, count) in points.items() if count > 1]
    return len(criteria)

def run_part2(file: typing.TextIO) -> int:
    # Now we're considering everything! So no filtering necessary. :)
    lines = [line for line in __parse_lines(file)]

    # To figure out point where there are overlaps, go through each line and figure out all the
    # points that makes up the line. From there, store these points into a dictionary and increment
    # the number of time we come across it.
    points = {}
    for line in lines:
        # print(line, "h=" + str(line.is_horizontal), "v=" + str(line.is_vertical), "d=" + str(line.is_diagonal), file=sys.stderr)
        for point in line.points():
            # print("\t", point, file=sys.stderr)
            if point not in points:
                points[point] = 1
            else:
                points[point] += 1

    # Return the number of keys whose count is greater than 1
    criteria = [point for (point, count) in points.items() if count > 1]
    return len(criteria)

def __parse_lines(file: typing.TextIO) -> typing.List[Line]:
    lines = []
    for line in file:
        # Parse each line (in the file) as an actual Line object. Format is as follow:
        # x1,y1 -> x2,y2
        match = re.match(r"(?P<x1>\d+),(?P<y1>\d+) -> (?P<x2>\d+),(?P<y2>\d+)", line) # type: re.Match
        lines.append(Line(
            x1=int(match.group("x1")),
            y1=int(match.group("y1")),
            x2=int(match.group("x2")),
            y2=int(match.group("y2"))
        ))
    return lines

run(__package__, run_part1, run_part2)
