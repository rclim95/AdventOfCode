import { readLinesFromStdin, runSolutions } from "./utils/common.ts";

enum Shape {
    Rock = 0,
    Paper,
    Scissor
}

enum Player {
    Me,
    Opponent
}

const OpponentLookup: Record<string, Shape> = {
    'A': Shape.Rock,
    'B': Shape.Paper,
    'C': Shape.Scissor
};

const MyLookup: Record<string, Shape> = {
    'X': Shape.Rock,
    'Y': Shape.Paper,
    'Z': Shape.Scissor
};

const ShapeScoreLookup: Record<Shape, number> = {
    [Shape.Rock]: 1,
    [Shape.Paper]: 2,
    [Shape.Scissor]: 3
};

function determineWinner(opponentShape: Shape, myShape: Shape): Player {
    if (opponentShape == Shape.Rock && myShape == Shape.Paper ||
        opponentShape == Shape.Paper && myShape == Shape.Scissor ||
        opponentShape == Shape.Scissor && myShape == Shape.Rock) {
        // I win!
        return Player.Me;
    }
    else {
        // I lost!
        return Player.Opponent;
    }
}

function determineMyPlay(opponentShape: Shape, strategy: string): Shape {
    switch (strategy) {
        case "X":   // I need to lose.
            if (opponentShape === Shape.Rock) {
                return Shape.Scissor;
            }
            else if (opponentShape === Shape.Paper) {
                return Shape.Rock;
            }
            else if (opponentShape === Shape.Scissor) {
                return Shape.Paper;
            }
            break;

        case "Y": // I need to tie (draw).
            return opponentShape;

        case "Z": // I need to win.
            if (opponentShape === Shape.Rock) {
                return Shape.Paper;
            }
            else if (opponentShape === Shape.Paper) {
                return Shape.Scissor;
            }
            else if (opponentShape === Shape.Scissor) {
                return Shape.Rock;
            }
            break;
    }

    // This shouldn't happen!
    throw new Error("Unable to handle the following strategy: " + strategy);
}

async function solutionOne() {
    let runningScore = 0;
    for await (const currentPlay of readLinesFromStdin()) {
        if (currentPlay === "") {
            continue;
        }

        // currentPlay is composed of two characters: the opponent and yourself.
        const [opponent, me] = currentPlay.split(' ', 2);
        const opponentShape = OpponentLookup[opponent.toUpperCase()];
        const myShape = MyLookup[me.toUpperCase()];

        runningScore += ShapeScoreLookup[myShape];

        if (opponentShape === myShape) {
            // It's a draw.
            runningScore += 3;
        }
        else {
            // Figure out who's victor.
            const winner = determineWinner(opponentShape, myShape);
            if (winner == Player.Me) {
                runningScore += 6;
            }
            else {
                runningScore += 0;
            }
        }
    }

    return runningScore;
}

async function solutionTwo() {
    let runningScore = 0;
    for await (const currentPlay of readLinesFromStdin()) {
        if (currentPlay === "") {
            continue;
        }

        // currentPlay is composed of two characters: the opponent and strategy.
        const [opponent, strategy] = currentPlay.split(' ', 2);
        const opponentShape = OpponentLookup[opponent.toUpperCase()];
        const myShape = determineMyPlay(opponentShape, strategy);

        // Calculate score like normal. :)
        runningScore += ShapeScoreLookup[myShape];
        switch (strategy) {
            case 'X': // I need to lose.
                runningScore += 0;
                break;

            case 'Y': // I need to tie (draw).
                runningScore += 3;
                break;

            case 'Z': // I need to win.
                runningScore += 6;
                break;
        }
    }

    return runningScore;
}

runSolutions(solutionOne, solutionTwo);
