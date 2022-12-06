import { readLinesFromStdin, runSolutions } from "./utils/common.ts";

/**
 * Represents a stack (of crates).
 */
class Stack {
    private _crates: string[];

    /**
     * Initializes the stack some labeled crates.
     * @param initialCrates The crates.
     */
    constructor(initialCrates: string[]) {
        this._crates = initialCrates;
    }

    /**
     * Gets the number of crates in the stack.
     */
    public get length() : number {
        return this._crates.length;
    }
    

    /**
     * Peeks at the top-most crate of the stack.
     * @returns The crate at the top of the stack or `null` if the stack is empty.
     */
    public peek(): string | null {
        return this._crates[this._crates.length - 1];
    }

    /**
     * Places a crate to the top of the stack.
     * @param crate The crate or crates (as an array of `string`) to push.
     */
    public push(crate: string | string[]) {
        if (typeof crate === "string") {
            this._crates.push(crate);
        }
        else {
            this._crates.push(...crate);
        }
    }

    /**
     * Pops one or more crates off the stack and return it.
     * @param total The number of crates to pop off. The default is 1.
     * @returns 
     *      The crate popped off or `null` if there were no crate on the stack. If `total` is
     *      greater than one, than an array of `string` matching `total` will be returned.
     */
    public pop(total = 1): string | string[] | null {
        if (total > 1) {
            return this._crates.splice(this._crates.length - total, total);
        }
        else {
            return this._crates.pop() ?? null;
        }
    }
}

async function readCrateStacks(stdin: AsyncIterableIterator<string>) {
    const stacks: Array<Array<string>> = [];
    while (true) {
        const it = await stdin.next();
        if (it.done || it.value === null) {
            break;
        }

        // If we don't see any brackets, that indicates we're done processing our crates. :)
        const line = it.value;
        if (line.indexOf("[") < 0) {
            break;
        }

        // Have we figured out how many stacks we're reading? If not, let's calculate that by
        // reading the line's length.
        if (stacks.length === 0) {
            // For the crate ASCII art, it's represented as "[X]", where X is the crate's label (letter).
            // When we look at the full picture, e.g.:
            //
            // [X]     [X]
            // [Y] [A] [B]
            //  1   2   3
            //
            // We can see that each crate takes up three characters and the stack of crates are
            // "space-delimited". However, if a crate doesn't exist in that row, it's simply a bunch
            // of spaces.
            //
            // To figure out how many stacks we're expecting, we can look at the length of the line,
            // add one ("pretend" we're adding a space to the last stack) and divide it by four.
            //
            // So in our example, "[X]     [X]" has a length of 11 characters. Add one and that makes
            // it 12, so divide that by 4, and we get 3--which is what we expect, as we have three
            // stacks. :)
            const totalStacks = Math.floor((line.length + 1) / 4);
            for (let i = 0; i < totalStacks; i++) {
                stacks.push([]);
            }
        }

        // Read the crates in this line in groups of four. :)
        for (let i = 0; i < line.length - 1; i += 4) {
            // Crate letters are always the second character in these four-length substrings we read.
            const crateLetter = line[i + 1];
            if (crateLetter !== " ") {
                // This is a crate, so figure out where it belongs.
                stacks[Math.floor(i / 4)].push(crateLetter);
            }
        }
    }

    return stacks.map(s => new Stack(s.reverse()));
}

async function solutionOne() {
    // Parse our crate stacks to get the initial position.
    const stdin = readLinesFromStdin();
    const stacks = await readCrateStacks(stdin);
    console.log(stacks);
    
    // Now that we've done that, let's figure out the rearrangement procedure. :)
    while (true) {
        const it = await stdin.next();
        if (it.done || it.value === null) {
            break;
        }

        // We're expecting the rearrangement procedure to match this regualr expression:
        const line = it.value;
        const result = line.match(/move (?<count>\d+) from (?<srcStack>\d+) to (?<dstStack>\d+)/);
        if (result === null) {
            // Not a valid rearrangement text--skip.
            continue;
        }

        const count = parseInt(result.groups?.count ?? "-1");
        const sourceStack = parseInt(result.groups?.srcStack ?? "-1");
        const destinationStack = parseInt(result.groups?.dstStack ?? "-1");

        // Make sure all of the following are valid. Otherwise, skip!
        if (count === -1 || sourceStack === -1 || destinationStack === -1) {
            continue;
        }

        // NOTE: The CrateMover 9000 only allows moving items one by one, so we need to iterate
        // through each move.
        for (let i = 0; i < count; i++) {
            // From the soruce stack, pop off the top crates and move them to the destination stack.
            // Note that we need to subtract one, since sourceStack and destinationStack are 1-based.
            const crateToMove = stacks[sourceStack - 1].pop();
            if (crateToMove !== null) {
                stacks[destinationStack - 1].push(crateToMove);
            }
        }
    }

    // Now peek at the top of the stack and return the crates up top.
    return stacks.map(s => s.peek()).join("");
}

async function solutionTwo() {
// Parse our crate stacks to get the initial position.
const stdin = readLinesFromStdin(true, false);
const stacks = await readCrateStacks(stdin);
console.log(stacks);

// Now that we've done that, let's figure out the rearrangement procedure. :)
while (true) {
    const it = await stdin.next();
    if (it.done || it.value === null) {
        break;
    }

    // We're expecting the rearrangement procedure to match this regualr expression:
    const line = it.value;
    const result = line.match(/move (?<count>\d+) from (?<srcStack>\d+) to (?<dstStack>\d+)/);
    if (result === null) {
        // Not a valid rearrangement text--skip.
        continue;
    }

    const count = parseInt(result.groups?.count ?? "-1");
    const sourceStack = parseInt(result.groups?.srcStack ?? "-1");
    const destinationStack = parseInt(result.groups?.dstStack ?? "-1");

    // Make sure all of the following are valid. Otherwise, skip!
    if (count === -1 || sourceStack === -1 || destinationStack === -1) {
        continue;
    }

    // From the soruce stack, pop off the top crates (of the speciifed count, as the CrateMover 9001
    // actually supports moving multiple crates at once and not one-by-one like Solution 1), and 
    // move them to the destination stack.
    //
    // Note that we need to subtract one, since sourceStack and destinationStack are 1-based.
    const cratesToMove = stacks[sourceStack - 1].pop(count);
    if (cratesToMove !== null) {
        stacks[destinationStack - 1].push(cratesToMove);
    }
}

// Now peek at the top of the stack and return the crates up top.
return stacks.map(s => s.peek()).join("");
}

runSolutions(solutionOne, solutionTwo);
