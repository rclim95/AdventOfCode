import argparse
from dataclasses import dataclass
import typing
import time

def run(
    module_name: str,
     part1_func: typing.Callable[[typing.TextIO], str],
     part2_func: typing.Callable[[typing.TextIO], str]):
    """Parses the arguments that were passed to the command line."""
    parser = argparse.ArgumentParser(module_name)
    parser.add_argument("--input",
                       type=argparse.FileType("r"),
                       required=True,
                       help="The input file to run the solution against. The default is 'input.txt'.")
    parser.add_argument("--part",
                        type=int,
                        default=1,
                        choices=[1, 2],
                        help="Which part of the solution would you like to run? The default is '1'.")
    parser.add_argument("--expected",
                        type=int,
                        default=None,
                        help="The expected result (used for testing to ensure the solution is working properly).")

    args = parser.parse_args()
    with args.input:
        if args.part == 2:
            run_part = part1_func
        else:
            run_part = part2_func

        # For funsies, we'll also time out how long it took to invoke the function.
        started_at = time.time()
        result = run_part(args.input)
        ended_at = time.time()
        __print_summary(result, ended_at - started_at, args.expected)

def __print_summary(result, duration, expected):

