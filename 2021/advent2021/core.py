import argparse
from dataclasses import dataclass
import typing
import time
import traceback
import colorama
from colorama import Fore, Style

def run(
    module_name: str,
     part1_func: typing.Callable[[typing.TextIO], str],
     part2_func: typing.Callable[[typing.TextIO], str]):
    """Parses the arguments that were passed to the command line."""
    # Prepare ourselves to have pretty console color
    colorama.init()

    # Build the argument parser
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
            run_part = part2_func
        else:
            run_part = part1_func

        # For funsies, we'll also time out how long it took to invoke the function.
        started_at = time.time()
        try:
            result = run_part(args.input)
        except Exception as e:
            result = e
        ended_at = time.time()

        # Now print out a summary!
        duration = ended_at - started_at
        print()
        print("The results are in!")
        if isinstance(result, Exception):
            print(Fore.RED + Style.DIM + "[❌] Program Encounterd Error")
            print(Fore.LIGHTBLUE_EX + "[⏱️] Duration:\t{0:#.3f} seconds".format(duration))
            print(Fore.YELLOW + "[❌] Exception: ")
            traceback.print_exception(type(result), result, result.__traceback__)
        else:
            # Did we meet our expected result (if provided)?
            if args.expected is not None and result == args.expected:
                print(Fore.GREEN + Style.BRIGHT + "[✅] Solution Ran with Expected Result" + Style.RESET_ALL)
            elif args.expected is not None:
                print(Fore.GREEN + Style.BRIGHT + "[⚠️] Solution Ran with Unexpected Result" + Style.RESET_ALL)
            else:
                print(Fore.GREEN + Style.BRIGHT + "[✅] Solution Ran Successfully" + Style.RESET_ALL)

            print(Fore.LIGHTBLUE_EX + "[⏱️] Duration:\t{0:#.3f} seconds".format(duration))
            print(Fore.BLUE + "[🤔] Result:\t" + Style.DIM + str(result), Style.RESET_ALL)

            if args.expected is not None:
                print(Fore.LIGHTBLUE_EX + "[👀] Expected:\t" + Style.BRIGHT + str(args.expected), Style.RESET_ALL)
