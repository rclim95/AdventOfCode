import logging
import re
import typing
from advent2024.core import run

TARGET_WORD = "XMAS"
TARGET_WORD_LEN = len(TARGET_WORD)

def part1(lines: typing.TextIO) -> int:
    # Read the entire word search into a 2 × 2 array
    word_search = []
    for line in lines:
        word_search.append([c for c in line if not c.isspace()])
    
    logging.debug("word_search = %s", word_search)

    # Now search for "XMAS" horizontally, vertically, and diagonally -- both backwards and forwards.
    count = 0
    for (r, row) in enumerate(word_search):
        for (c, letter) in enumerate(row):
            if letter == 'X':
                # This might be the start of "XMAS"! Look around and see if it is.
                count += count_xmas_horizontally(word_search, r, c)

    return count

def part2(lines: typing.TextIO) -> int:
    raise NotImplementedError()

def count_xmas_horizontally(word_search: typing.List[typing.List[str]], row: int, column: int) -> int:
    """Count the number of `XMAS` that was found horizontally at `(row, col)`, both forwards and backwards."""
    current_row = word_search[row]

    # Can we go backward horizontally? If so, look for XMAS (in reverse).
    count = 0
    if column >= 3:
        word = "".join(reversed(current_row[column - TARGET_WORD_LEN:column + 1]))
        logging.debug("Trying reversed %s at %d, %d", word, row, column)
        if word == TARGET_WORD:
            count += 1
    
    # Can we go forward horizontally? If so, look for "XMAS".
    if column <= len(current_row) - TARGET_WORD_LEN:
        word = "".join(current_row[column : TARGET_WORD_LEN])
        logging.debug("Trying %s at %d, %d", word, row, column)
        if word == TARGET_WORD:
            count += 1
    
    return count

run(__package__, part1, part2)
