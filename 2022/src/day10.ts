import { readLinesFromStdin, runSolutions } from "./utils/common.ts";

const ScreenCharWidth = 40;

/**
 * Represents a CPU for a cathode ray tube screen, as defined in Day 10 - Cathode-Ray Tube for
 * Advent of Code. :)
 */
class Cpu {
    private _x = 1;
    private _cycle = 0;

    /**
     * Executes the `addx` instruction, which takes two cycle to complete.
     * @param operand The number to add to the `X` register.
     */
    public * addx(operand: number) {
        yield this._doCycle();
        yield this._doCycle();

        // KEYWORD: During... so after this last cycle, the X register should be modified. :)
        this._x += operand;
    }

    /**
     * Executes the `noop` instruction, which takes one cycle to complete.
     */
    public * noop() {
        yield this._doCycle();
    }

    private _doCycle(): { cycle: number, x: number } {
        return {
            cycle: ++this._cycle,
            x: this._x
        }
    }
}

/**
 * Finds the sum of the signal strengths for the specified cycles.
 * @param cyclesToRead The cycle to record the record the signal strength. The signal strength is the 
 *      the current cycle multiplied by the value in the `X` register.
 */
async function sumOfSignalStrengths(cyclesToRead: number[]): Promise<number> {
    const readings: number[] = [];
    const cpu = new Cpu();

    for await (const instruction of readLinesFromStdin(false)) {
        let instructionToExecute = null;

        // Figure out which instruction we're executing.
        if (instruction.startsWith("addx")) {
            // It's the `addx` instruction, which takes in an argument.
            const [_, operandAsString] = instruction.split(" ", 2);
            instructionToExecute = cpu.addx.bind(cpu, parseInt(operandAsString));
        }
        else {
            // Assume it's the `noop` instruction.
            instructionToExecute = cpu.noop.bind(cpu);
        }

        for (const { cycle, x } of instructionToExecute()) {
            console.log(instruction, cycle, x);
            if (cyclesToRead.includes(cycle)) {
                // This is a cycle of interest! Figure out its reading.
                readings.push(cycle * x);
            }
        }
    }

    return readings.reduce((p, c) => p + c, 0);
}

/**
 * Renders what the CRT is drawing.
 */
async function drawScreen() {
    const screen: Array<Array<string>> = [];
    let currentLine: Array<string> | null = null;
    const cpu = new Cpu();

    for await (const instruction of readLinesFromStdin(false)) {
        let instructionToExecute = null;

        // Figure out which instruction we're executing.
        if (instruction.startsWith("addx")) {
            // It's the `addx` instruction, which takes in an argument.
            const [_, operandAsString] = instruction.split(" ", 2);
            instructionToExecute = cpu.addx.bind(cpu, parseInt(operandAsString));
        }
        else {
            // Assume it's the `noop` instruction.
            instructionToExecute = cpu.noop.bind(cpu);
        }

        for (const { cycle, x } of instructionToExecute()) {
            if (cycle % ScreenCharWidth === 1) {
                // We're rendering a new line for the CRT--render it. :)
                currentLine = new Array(ScreenCharWidth);
                screen.push(currentLine);
            }

            // This shouldn't happen, but to make TypeScript happy...
            if (currentLine === null) {
                throw new Error("currentLine is null. This shouldn't happen!");
            }

            // Figure out the start and end index of our sprite. The sprite is 3 characters long,
            // and the X register indicates the middle position of the sprite. The current pixel will
            // be "lit" depending on whether the current column is within the same horizointal position
            // of where our sprite is active. :)
            const currentHorizontal = (cycle % ScreenCharWidth) - 1;
            const withinSprite = currentHorizontal >= x - 1 && currentHorizontal <= x + 1;
            currentLine[(cycle % ScreenCharWidth) - 1] = withinSprite ? "#" : ".";
        }
    }

    return screen.map(p => p.join("")).join("\n");
}

runSolutions(
    async () => await sumOfSignalStrengths([20, 60, 100, 140, 180, 220]),
    async () => await drawScreen()
);
