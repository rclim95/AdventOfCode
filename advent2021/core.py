import argparse
from dataclasses import dataclass
import os
import typing

def parse_arguments(
    module_name: str,
    script_path: str,
     part1_func: typing.Callable[[typing.TextIO], None],
     part2_func: typing.Callable[[typing.TextIO], None]):
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

    result = parser.parse_args()
    with result.input:
        if result.part == 2:
            part2_func(result.input)
        else:
            part1_func(result.input)
