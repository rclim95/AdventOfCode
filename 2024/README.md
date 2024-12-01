# Advent of Code 2024

Implementations for finding the solutions to the problems for Advent of Code 2024 in... Python (again)! :snake:

## Running It

To run the solutions in this directory, make sure that you're in `advent2024/` folder of this
repository (i.e., the folder where this `REAMDE.md` resides in :smile:). From there, run whichever
day using `pythom -m` command, like so:

```bash
# Run Day 1, Part 1 solution (using STDIN)
$ python -m advent2024.day1 - --part 1 < inputs/day1.txt

# Run Day 1, Part 1 solution (using the input file in inputs/day1.txt)
$ python -m advent2024.day1 inputs/day1.txt --part 1

# Run Day 2, Part 2 solution (using the input file in inputs/day2.txt)
$ python -m advent2024.day2 inputs/day2.txt --part 2

# Run Day 3, Part 1 solution (using the input file in inputs/day1.txt, expecting the result 1071734,
# which the program will print out whether the expected result was calculated or not)
$ python -m advent2024.day3 inputs/day3.txt --part 1 --expected 1071734
```

For more information, run `python -m advent2024.dayX -h` (`X` being one of the implemented Advent of
Code 2024 days that's in this repository :kissing:) and you'll get a pretty helpful message,
like so:
```
usage: advent2024.dayX [-h] [--part {1,2}] [--expected EXPECTED] infile

positional arguments:
  infile               The input file to run the solution against.

options:
  -h, --help           show this help message and exit
  --part {1,2}         Which part of the solution would you like to run? The default is '1'.
  --expected EXPECTED  The expected result (used for testing to ensure the solution is working properly).
```
