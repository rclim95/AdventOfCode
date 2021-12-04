from argparse import ArgumentError
import sys
import typing
from advent2021.core import run

class BingoCard:
    ROW_COUNT = 5
    COL_COUNT = 5

    """Represents a 5 × 5 Bingo Card"""
    def __init__(self, board: typing.List[typing.List[int]]):
        # Sanity check. :)
        if len(board) != BingoCard.ROW_COUNT:
            raise ArgumentError("Invalid board. There must be {} rows.".format(BingoCard.ROW_COUNT))

        for row in board:
            if len(row) != BingoCard.COL_COUNT:
                raise ArgumentError("Invalid board. There must be {} columns for all rows.".format(BingoCard.COL_COUNT))

        self.board = board

    def mark(self, number_called):
        """Mark the board with the number called."""
        for row_index in range(BingoCard.ROW_COUNT):
            for col_index in range(BingoCard.COL_COUNT):
                if self.board[row_index][col_index] == number_called:
                    self.board[row_index][col_index] = None # Marked!
                    break

    def sum(self):
        """Gets the sum of all unmarked numbers that remained on the board"""
        running_sum = 0
        for row_index in range(BingoCard.ROW_COUNT):
            for col_index in range(BingoCard.COL_COUNT):
                number = self.board[row_index][col_index]
                if number is None:
                    continue

                running_sum += number
        return running_sum

    def is_winner(self):
        """Determines whether the board has won or not"""
        # First, check to see if we've gotten a horizontal win (i.e., one row has all numbers marked).
        for row in self.board:
            total_marked = len([True for col in row if col is None])
            if total_marked == BingoCard.COL_COUNT:
                return True

        # To check if we've gotten a vertical win (i.e., one column has all numbers marked), we'll
        # need to do some "fancy" traversals.
        for col_index in range(BingoCard.COL_COUNT):
            total_marked = 0
            for row_index in range(BingoCard.ROW_COUNT):
                if self.board[row_index][col_index] is None:
                    total_marked += 1
            if total_marked == BingoCard.ROW_COUNT:
                return True

        # Didn't win (yet) :(
        return False

    def __str__(self):
        # Print out the board.
        board_str = ""
        for row in self.board: # type: typing.List
            board_str += " ".join(["{0:02d}".format(col) if col is not None else "--" for col in row]) + "\n"
        return board_str.strip()

def run_part1(file: typing.TextIO) -> int:
    # The first line contains the number called out for the game of Bingo.
    bingo_numbers = __parse_bingo_numbers(file)

    # The next step is to build our 5 × 5 bingo cards.
    bingo_cards = __parse_bingo_cards(file)

    # Go through the numbers called out
    winning_card = None
    winning_number = 0
    for bingo_number in bingo_numbers:
        # Go through each of our board and mark it.
        for bingo_card in bingo_cards: # type: BingoCard
            bingo_card.mark(bingo_number)
            if bingo_card.is_winner():
                winning_card = bingo_card
                winning_number = bingo_number

        # If we've found a winner, stop
        if winning_card is not None:
            break

    # Now that we've gotten our winning card, figure out the score (i.e., the sum of the unmarked
    # numbers on the board multiplied by the winning number)
    print("Winning Number: ", winning_number, file=sys.stderr)
    print("Winning Board:", file=sys.stderr)
    print(winning_card, file=sys.stderr)

    return winning_card.sum() * winning_number

def run_part2(file: typing.TextIO) -> int:
    # The first line contains the number called out for the game of Bingo.
    bingo_numbers = __parse_bingo_numbers(file)

    # The next step is to build our 5 × 5 bingo cards.
    bingo_cards = __parse_bingo_cards(file)

    # Again, go through the numbers called out. However, we want to keep going until we find
    # the last bingo card that would win and calculate the score from that.
    last_winning_card = None
    last_winning_number = 0
    for bingo_number in bingo_numbers:
        # Go through each of our board and mark it.
        for (index, bingo_card) in enumerate(list(bingo_cards)): # type: BingoCard
            bingo_card.mark(bingo_number)
            if bingo_card.is_winner():
                last_winning_card = bingo_card
                last_winning_number = bingo_number

                # Since this card won, there's no need to process it anymore, so remove it.
                bingo_cards.remove(bingo_card)

    # Now that we've gotten our winning (losing) card, figure out the score (i.e., the sum of the
    # unmarked numbers on the board multiplied by the winning number)
    print("(Last) Winning Number: ", last_winning_number, file=sys.stderr)
    print("(Last) Winning Board:", file=sys.stderr)
    print(last_winning_card, file=sys.stderr)

    return last_winning_card.sum() * last_winning_number

def __parse_bingo_numbers(file: typing.TextIO) -> typing.List[int]:
    return [int(x) for x in file.readline().strip().split(",")]

def __parse_bingo_cards(file: typing.TextIO) -> typing.List[BingoCard]:
    bingo_cards = []
    current_board = []
    for line in file:
        if line.strip() == "":
            # This is an empty line...But is the current board valid, i.e., has five rows?
            if len(current_board) == 5:
                # That means this is the start of a bingo card and we can
                # create a BingoCard from the current board. :) Once we've created a BingoCard,
                # empty the list so we can start building the next board.
                bingo_cards.append(BingoCard(current_board))
                current_board = []
        else:
            # The line isn't empty, so it must contain numbers. Add it to the current board.
            current_board.append([int(num) for num in line.strip().split(" ") if num != ""])

    # Don't forget to append the last board!
    bingo_cards.append(BingoCard(current_board))
    return bingo_cards

run(__package__, run_part1, run_part2)
