# Advent of Code 2021

Implementations for finding the solutions to the problems for Advent of Code 2021 in... Python! :snake:

## Running It

To run the solutions in this directory, make sure that you're in `advent2021/` folder of this
repository (i.e., the folder where this `REAMDE.md` resides in :smile:). From there, run whichever
day using `pythom -m` command, like so:

```bash
# Run Day 1, Part 1 solution (using the input file in inputs/day1.txt)
$ python -m advent2021.day1 --input inputs/day1.txt --part 1

# Run Day 2, Part 2 solution (using the input file in inputs/day2.txt)
$ python -m advent2021.day2 --input inputs/day2.txt --part 2

# Run Day 3, Part 1 solution (using the input file in inputs/day1.txt, expecting the result 1071734,
# which the program will print out whether the expected result was calculated or not)
$ python -m advent2021.day3 --input inputs/day3.txt --part 1 --expected 1071734
```

For more information, run `python -m day2021.dayX -h` (`X` being one of the implemented Advent of
Code 2021 days that's in this repository :kissing:) and you'll get a pretty helpful message,
like so:
```
usage: advent2021.dayX [-h] --input INPUT [--part {1,2}] [--exgit spected EXPECTED]

options:
  -h, --help           show this help message and exit
  --input INPUT        The input file to run the solution against. The default
                       is 'input.txt'.
  --part {1,2}         Which part of the solution would you like to run? The
                       default is '1'.
  --expected EXPECTED  The expected result (used for testing to ensure the
                       solution is working properly).
```
