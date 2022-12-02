import { readLines } from "io/buffer.ts";

async function solutionOne() {
    let maxCalories = 0;
    let runningCaloriesSum = 0;
    for await (const line of readLines(Deno.stdin)) {
        if (line.length === 0) {
            // Is this our newest max? If so, store it, because we're interested in the calories
            // carried by the elf that's carrying the most calories here. :)
            if (runningCaloriesSum > maxCalories) {
                maxCalories = runningCaloriesSum;
            }

            // An empty line indicates that we're about to read in the calories for the next
            // elves--clear our running sum.
            runningCaloriesSum = 0;
        }
        else {
            // Add to our running total.
            runningCaloriesSum += parseInt(line);
        }
    }

    return maxCalories;
}

async function solutionTwo() {
    const topThreeCalories = [0, 0, 0];
    let runningCaloriesSum = 0;
    for await (const line of readLines(Deno.stdin, )) {
        console.log("Read: \"" + line + "\"");
        if (line.length === 0) {
            // Find the smallest element in the topThreeCalories list, and place the new max there,
            // if one exists.
            if (topThreeCalories[0] < runningCaloriesSum) {
                topThreeCalories[2] = topThreeCalories[1];
                topThreeCalories[1] = topThreeCalories[0];
                topThreeCalories[0] = runningCaloriesSum;
            }
            else if (topThreeCalories[1] < runningCaloriesSum) {
                topThreeCalories[2] = topThreeCalories[1];
                topThreeCalories[1] = runningCaloriesSum;
            }
            else if (topThreeCalories[2] < runningCaloriesSum) {
                topThreeCalories[2] = runningCaloriesSum;
            }

            // An empty line indicates that we're about to read in the calories for the next
            // elves--clear our running sum.
            runningCaloriesSum = 0;
        }
        else {
            // Add to our running total.
            runningCaloriesSum += parseInt(line);
        }
    }

    // Don't forget the last element!
    if (topThreeCalories[0] < runningCaloriesSum) {
        topThreeCalories[2] = topThreeCalories[1];
        topThreeCalories[1] = topThreeCalories[0];
        topThreeCalories[0] = runningCaloriesSum;
    }
    else if (topThreeCalories[1] < runningCaloriesSum) {
        topThreeCalories[2] = topThreeCalories[1];
        topThreeCalories[1] = runningCaloriesSum;
    }
    else if (topThreeCalories[2] < runningCaloriesSum) {
        topThreeCalories[2] = runningCaloriesSum;
    }

    return topThreeCalories.reduce((previous, current) => previous + current, 0);
}


console.log("Solution 1:", await solutionOne());
console.log("Solution 2:", await solutionTwo());
