import { readLines } from "io/buffer.ts";
import { parse } from "flags/mod.ts";

type SolutionFuncAsync = () => Promise<string> | Promise<number>;

/**
 * Determines whether to run `solutionOne` or `solutionTwo` depending on the arguments passed
 * to the command line.
 * @param solutionOne The function to invoke representing the first solution.
 * @param solutionTwo The function to invoke representing the second solution.
 */
export async function runSolutions(solutionOne: SolutionFuncAsync, solutionTwo: SolutionFuncAsync) {
    const parsedArgs = parse(Deno.args);
    switch (parsedArgs.solution) {
        case 1:
            // Run the first solution
            console.log("Solution 1:");
            console.log(await solutionOne());
            break;

        case 2:
            // Run the second solution.
            console.log("Solution 2:");
            console.log(await solutionTwo());
            break;

        default:
            console.error("ERROR: --solution argument has not been provided or the value provided");
            console.error("       is not supported.");
    }
}

/**
 * Returns an asychronous iterator that will read each line from `STDIN`.
 * @returns An iterator that'll go through each line in STDIN.
 */
export async function* readLinesFromStdin(): AsyncIterableIterator<string> {
    let lastLineRead = "";
    for await (const line of readLines(Deno.stdin)) {
        lastLineRead = line;
        yield line;
    }

    // If the last line read is not an empty string, then assume that the newline at the end of the
    // file got eaten up.
    if (lastLineRead !== "") {
        yield "";
    }
}
