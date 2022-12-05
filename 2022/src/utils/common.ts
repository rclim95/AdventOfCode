import { BufReader } from "io/buffer.ts";
import { EOL } from "fs/eol.ts";
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
 * @param includeNewLineEnding
 *      Indicate whether the last line, i.e., an empty new line, should be included as part
 *      of the line reading. The default is `true`. If `false`, it'll be omitted.
 * @returns An iterator that'll go through each line in STDIN.
 */
export async function* readLinesFromStdin(includeNewLineEnding = true): AsyncIterableIterator<string> {
    const bufReader = new BufReader(Deno.stdin);
    while (true) {
        const line = await bufReader.readString(EOL.LF);

        // We reached the end of STDIN, so stop.
        if (line === null) {
            return;
        }

        // Do we need to return the "new line" ending, typically represented with an empty line?
        const nextByte = await bufReader.peek(1);
        if (nextByte === null && !includeNewLineEnding) {
            return;
        }

        // Otherwise, keep on returning the current line.
        yield line.trim();
    }
}
