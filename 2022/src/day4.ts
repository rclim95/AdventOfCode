import { readLinesFromStdin, runSolutions } from "./utils/common.ts";

async function solutionOne() {
    let runningEngulfedRanges = 0;  // "Engulfed" as in one section range fully contains the other one :)
    for await (const line of readLinesFromStdin(false)) {
        // Read our line using... regular expression!
        const lineAsRanges = line.match(/(\d+)-(\d+),(\d+)-(\d+)/);
        if (lineAsRanges === null) {
            continue;
        }

        const [rangeOneMin, rangeOneMax] = [parseInt(lineAsRanges[1]), parseInt(lineAsRanges[2])];
        const [rangeTwoMin, rangeTwoMax] = [parseInt(lineAsRanges[3]), parseInt(lineAsRanges[4])];

        // Now check to see if rangeOne and rangeTwo includes one or the other. For example, if
        // range one is [2, 5] (inclusive) and range two is [3, 4], range one would be including
        // range two. Conversely, if range one is [6, 6] and range two is [1, 10], then range two
        // would include range one.
        //
        // That is:
        // - rangeOneMin <= rangeTwoMin <= rangeTwoMax <= rangeOneMax
        // - rangeTwoMin <= rangeOneMin <= rangeOneMax <= rangeTwoMax
        if ((rangeOneMin <= rangeTwoMin && rangeTwoMax <= rangeOneMax) ||
            (rangeTwoMin <= rangeOneMin && rangeOneMax <= rangeTwoMax)) {
            runningEngulfedRanges += 1;
        }
    }

    return runningEngulfedRanges;
}

async function solutionTwo() {
    let runningEngulfedRanges = 0;  // "Engulfed" as in one section range fully contains the other one :)
    for await (const line of readLinesFromStdin(false)) {
        // Read our line using... regular expression!
        const lineAsRanges = line.match(/(\d+)-(\d+),(\d+)-(\d+)/);
        if (lineAsRanges === null) {
            continue;
        }

        const [rangeOneMin, rangeOneMax] = [parseInt(lineAsRanges[1]), parseInt(lineAsRanges[2])];
        const [rangeTwoMin, rangeTwoMax] = [parseInt(lineAsRanges[3]), parseInt(lineAsRanges[4])];

        // Now check to see if rangeOne and rangeTwo fully includes one or the other. For example, if
        // range one is [2, 5] (inclusive) and range two is [3, 4], range one would be including
        // range two. Conversely, if range one is [6, 6] and range two is [1, 10], then range two
        // would include range one.
        //
        // That is:
        // - rangeOneMin <= rangeTwoMin <= rangeTwoMax <= rangeOneMax
        // - rangeTwoMin <= rangeOneMin <= rangeOneMax <= rangeTwoMax
        if ((rangeOneMin <= rangeTwoMin && rangeTwoMax <= rangeOneMax) ||
            (rangeTwoMin <= rangeOneMin && rangeOneMax <= rangeTwoMax)) {
            runningEngulfedRanges += 1;
        }
        // We also want to check if there's even any overlap in general. That is:
        // - rangeOneMin <= (rangeTwoMin, rangeTwoMax) <= rangeOneMax
        // - rangeTwoMin <= (rangeOneMin, rangeOneMax) <= rangeTwoMax
        else if (
            ((rangeOneMin <= rangeTwoMin && rangeTwoMin <= rangeOneMax) || (rangeOneMin <= rangeTwoMax && rangeTwoMax <= rangeOneMax)) ||
            ((rangeTwoMin <= rangeOneMin && rangeOneMin <= rangeTwoMax) || (rangeTwoMin <= rangeOneMax && rangeOneMax <= rangeTwoMax))
        ) {
            runningEngulfedRanges += 1;
        }
    }

    return runningEngulfedRanges;
}

runSolutions(solutionOne, solutionTwo);
