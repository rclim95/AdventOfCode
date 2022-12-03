import { readLinesFromStdin, runSolutions } from "./utils/common.ts";

function findCommonItem(compartmentOne: string, compartmentTwo: string) {
    // Convert the first compartment to a "lookup" table.
    const compartmentOneLookup = new Set(compartmentOne);
    for (const item of compartmentTwo) {
        if (compartmentOneLookup.has(item)) {
            // Found the common item!
            return item;
        }
    }

    // This shouldn't happen as it wasn't mentioned in the original problem statement... :sweal:
    return "";
}

function calculatePriority(item: string) {
    // Priority is calculated as follow:
    // Letters [a...z] have priorities [1...26], respectively.
    // Letters [A...Z] have priorities [27...52], respectively.
    //
    // Use char codes to make this easy to work with. :)
    if (item.match(/[A-Z]/) !== null) {
        // Upper-case letter.
        return 27 + (item.charCodeAt(0) - 'A'.charCodeAt(0));
    }
    else {
        // Lower-case letter.
        return 1 + (item.charCodeAt(0) - 'a'.charCodeAt(0));
    }
}

async function solutionOne() {
    let runningPriority = 0;
    for await (const line of readLinesFromStdin()) {
        if (line === "") {
            continue;
        }

        // Each line can be split in half to get the items that makes up each compartment of the
        // rucksack.
        const midway = line.length / 2;
        const firstCompartment = line.substring(0, midway);
        const secondCompartment = line.substring(midway);

        // Find the common item and figure out its priority score.
        const commonItem = findCommonItem(firstCompartment, secondCompartment);
        runningPriority += calculatePriority(commonItem);
    }

    return runningPriority;
}

async function solutionTwo() {
    let runningPriority = 0;
    const currentGroup = [];

    for await (const line of readLinesFromStdin()) {
        // NOTE: This time, we're not skipping the empty line, as we need that to *signify* to us
        // that we should start processing the last group (compared to Solution 1). :)

        // We're now working in groups of three, so keep appending until we got three elves in our
        // current group. :)
        if (currentGroup.length !== 3) {
            currentGroup.push(line);
            continue;
        }

        // Once we're here, let's process their items and find their missing badges (i.e., the
        // item that appears THRICE in all three lines). In other words, an intersection!
        let badgeItem = "";
        const elfOneItems = new Set(currentGroup[0]);
        const elfTwoItems = new Set(currentGroup[1]);
        const elfThreeItems = new Set(currentGroup[2]);
        for (const item of elfOneItems) {
            if (elfTwoItems.has(item) && elfThreeItems.has(item)) {
                // Found the badge item!
                badgeItem = item;
                break;
            }
        }
        console.assert(badgeItem !== "", "badgeItem === ''");

        runningPriority += calculatePriority(badgeItem);

        // Don't forget to clear the previous group's items and add the current line
        // before moving on.
        currentGroup.length = 0;
        currentGroup.push(line);
    }

    return runningPriority;
}

runSolutions(solutionOne, solutionTwo);
