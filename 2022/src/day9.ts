import { readLinesFromStdin, runSolutions } from "./utils/common.ts";

/**
 * Defines the direction that the head of the rope is moving.
 */
enum Direction {
    Horizontal,
    Vertical
}

/**
 * Represents a rope whose tails is closely attached to its head (i.e., the tail and head are at most
 * a space apart or the head is on top of its tail).
 */
class Rope {
    private _knotPositions: Array<[number, number]>;

    /**
     * Constructor.
     * @param initialPosition The initial position of the rope.
     * @param numberOfKnots The number of knots, including the head and tail. A minimum of two
     *  knots are required.
     */
    constructor(initialPosition: [number, number], numberOfKnots = 2) {
        if (numberOfKnots < 2) {
            throw new RangeError("The number of knots must be 2 or greater.");
        }

        // Initialize the stack of knots to the initial position.
        this._knotPositions = [];
        for (let i = 0; i < numberOfKnots; i++) {
            this._knotPositions.push([initialPosition[0], initialPosition[1]]);
        }
    }

    /**
     * Gets the current position of the head of the rope.
     */
    public get headCurrentPosition() {
        return this._knotPositions[0];
    }

    /**
     * Gets the current position of the tail of the rope.
     */
    public get tailCurrentPosition() {
        return this._knotPositions[this._knotPositions.length - 1];
    }

    /**
     * Moves the head of the rope up.
     * @param moveCount The number of units to move up.
     * @returns An array of point containing the positions the tail was at while the head moved.
     */
    public moveUp(moveCount: number): Array<[number, number]> {
        return this.doMove(Direction.Vertical, moveCount * -1);
    }

    /**
     * Moves the head of the rope down.
     * @param moveCount The number of units to move down.
     * @returns An array of point containing the positions the tail was at while the head moved.
     */
    public moveDown(moveCount: number): Array<[number, number]> {
        return this.doMove(Direction.Vertical, moveCount);
    }

    /**
     * Moves the head of the rope left.
     * @param moveCount The number of units to move left.
     * @returns An array of point containing the positions the tail was at while the head moved.
     */
    public moveRight(moveCount: number): Array<[number, number]> {
        return this.doMove(Direction.Horizontal, moveCount);
    }

    /**
     * Moves the head of the rope right.
     * @param moveCount The number of units to move right.
     * @returns An array of point containing the positions the tail was at while the head moved.
     */
    public moveLeft(moveCount: number): Array<[number, number]> {
        return this.doMove(Direction.Horizontal, moveCount * -1);
    }

    private doMove(direction: Direction, offset: number): Array<[number, number]> {
        // NB: Set doesn't work with array, because array comparison are done by reference. Therefore,
        // we need to store our tail positions as strings instead.
        const tailPositions = new Array<[number, number]>();

        // Figure out where the head is supposed to go...
        const positionIndex = direction == Direction.Horizontal ? 0 : 1;
        const stepDirection = offset < 0 ? -1 : 1;
        const start = this.headCurrentPosition[positionIndex];
        const end = start + offset + stepDirection;
        for (let i = start; i !== end; i += stepDirection) {
            // ...and move the head there. :)
            this.headCurrentPosition[positionIndex] = i;

            // Now go through the remaining knots and make sure that the knot that it's adjacent
            // to is nearby, i.e., within 1 unit diagonally or adjacent.
            for (let j = 0; j < this._knotPositions.length - 1; j++) {
                const [horizDiff, vertDiff] = this.distanceBetweenKnots(j, j + 1);

                if (Math.abs(horizDiff) < 2 && Math.abs(vertDiff) < 2) {
                    // They're pretty close to each other, so no need to do anything.
                    continue;
                }
                
                // Do we need to move the adjacent knot diagonally?
                if (Math.abs(horizDiff) > 1 && Math.abs(vertDiff) !== 0 ||
                    Math.abs(vertDiff) > 1 && Math.abs(horizDiff) !== 0) {
                    this._knotPositions[j + 1][0] += Math.sign(horizDiff);
                    this._knotPositions[j + 1][1] += Math.sign(vertDiff);
                }
                else {
                    // Follow along in which ever direction the knot should go with respect
                    // to its adjacent knot.
                    if (horizDiff === 0) {
                        this._knotPositions[j + 1][1] += Math.sign(vertDiff);
                    }
                    else if (vertDiff === 0) {
                        this._knotPositions[j + 1][0] += Math.sign(horizDiff);
                    }
                }
            }

            // Add the tail position, if it changed.
            const lastIndex = tailPositions.length - 1;
            if (tailPositions.length === 0 ||
                tailPositions[lastIndex][0] !== this.tailCurrentPosition[0] ||
                tailPositions[lastIndex][1] !== this.tailCurrentPosition[1]) {
                tailPositions.push([this.tailCurrentPosition[0], this.tailCurrentPosition[1]]);
            }
        }

        return tailPositions;
    }

    private distanceBetweenKnots(knotIndex: number, otherKnotIndex: number): [number, number] {
        return [
            this._knotPositions[knotIndex][0] - this._knotPositions[otherKnotIndex][0],
            this._knotPositions[knotIndex][1] - this._knotPositions[otherKnotIndex][1]
        ];
    }
}


/**
 * Counts the number of unique positions the `T` (tail) of the rope have visited at least once.
 * @param numberOfKnots The number of knots, including the head and tail. Minimum is `2`.
 */
async function countUniquePositionsOfTail(numberOfKnots: number) {
    // NB: Because arrays are compared by reference, we can't use a Set<[number, number]> here,
    // because equality comparison to determine whether a list is "unique" will be reference-based,
    // i.e., exists in the same "memory location," than value-based, i.e., checking to see if the
    // list contains the same values. Therefore, use strings to make usre we're doing value-based
    // comparisons. :)
    const tailPositions = new Set<string>();

    // Initialize our current position to (0, 0) and add it to the tail's current position.
    const currentPosition: [number, number] = [0, 0];
    tailPositions.add(currentPosition.join(","));

    // Now read our movement script.
    const rope = new Rope(currentPosition, numberOfKnots);
    for await (const line of readLinesFromStdin(false)) {
        // The line is composed of two tokens, delimited by a space. The first token represents
        // the direction ("U", "D", "L", "R") and the second token represents the number of moves
        // in that direction.
        const [direction, moveCount] = line.split(" ", 2);
        const moveCountAsInt = parseInt(moveCount);
        switch (direction) {
            case "U":
                // We're going up.
                rope.moveUp(moveCountAsInt).forEach(p => tailPositions.add(p.join(",")));
                break;
            
            case "L":
                // We're going left.
                rope.moveLeft(moveCountAsInt).forEach(p => tailPositions.add(p.join(",")));
                break;
            
            case "D":
                // We're going down.
                rope.moveDown(moveCountAsInt).forEach(p => tailPositions.add(p.join(",")));
                break;
            
            case "R":
                // We're going right.
                rope.moveRight(moveCountAsInt).forEach(p => tailPositions.add(p.join(",")));
                break;
        }
    }

    return tailPositions.size;
}

runSolutions(
    async () => await countUniquePositionsOfTail(2),
    async () => await countUniquePositionsOfTail(10)
);
