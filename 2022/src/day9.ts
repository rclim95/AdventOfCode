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
    private _headPosition: [number, number];
    private _tailPosition: [number, number];

    /**
     * Constructor.
     * @param initialPosition The initial position of the rope.
     */
    constructor(initialPosition: [number, number]) {
        this._headPosition = [initialPosition[0], initialPosition[1]];
        this._tailPosition = [initialPosition[0], initialPosition[1]];
    }

    /**
     * Gets the current position of the head of the rope.
     */
    public get headCurrentPosition() {
        return this._headPosition;
    }

    /**
     * Gets the current position of the head of the rope.
     */
    public get tailCurrentPosition() {
        return this._tailPosition;
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
        switch (direction) {
            case Direction.Horizontal: {
                // We're moving left (-1) or right (+1).
                const stepDirection = offset < 0 ? -1 : 1;
                const start = this._headPosition[0];
                const stop = this._headPosition[0] + offset + stepDirection;
                for (let i = start; i !== stop; i += stepDirection) {
                    // Move in that direction. :)
                    this._headPosition[0] = i;

                    // Now evaluate: is our tail near our head?
                    if (!this.isTailNearHead()) {
                        // It isn't! D: Help the tail get close to its head.
                        const [horizDiff, vertDiff] = this.distanceBetweenHeadAndTail();

                        // If the head and tail are not on the same row, then move the tail
                        // diagonally in the proper direction so that it's near.
                        if (vertDiff !== 0) {
                            this._tailPosition[1] += vertDiff;
                        }
                        
                        this._tailPosition[0] += horizDiff - stepDirection;
                        tailPositions.push([this._tailPosition[0], this._tailPosition[1]]);
                    }
                }
                break;
            }
            
            case Direction.Vertical: {
                // We're moving up (-1) or down (+1).
                const stepDirection = offset < 0 ? -1 : 1;
                const start = this._headPosition[1];
                const stop = this._headPosition[1] + offset + stepDirection;
                for (let i = start; i !== stop; i += stepDirection) {
                    // Move in that direction. :)
                    this._headPosition[1] = i;

                    // Now evaluate: is our tail near our head?
                    if (!this.isTailNearHead()) {
                        // It isn't! D: Help the tail get close to its head.
                        const [horizDiff, vertDiff] = this.distanceBetweenHeadAndTail();

                        // If the head and tail are not on the same column, then move the tail
                        // diagonally in the proper direction so that it's near.
                        if (horizDiff !== 0) {
                            this._tailPosition[0] += horizDiff;
                        }
                        this._tailPosition[1] += vertDiff - stepDirection;
                        tailPositions.push([this._tailPosition[0], this._tailPosition[1]]);
                    }
                    
                    
                }
                break;
            }
        }

        return tailPositions;
    }

    private distanceBetweenHeadAndTail(): [number, number] {
        return [
            this._headPosition[0] - this._tailPosition[0],
            this._headPosition[1] - this._tailPosition[1]
        ];
    }

    private isTailNearHead(): boolean {
        // Find the difference between the X and Y coordinate of the head and tail. If we're within
        // one (or even zero, indicating overlaps), we're good! :D
        const distance = this.distanceBetweenHeadAndTail();
        console.log(distance);
        const horizDiff = Math.abs(distance[0]);
        const vertDiff = Math.abs(distance[1]);

        return horizDiff < 2 && vertDiff < 2;
    }
}


/**
 * Counts the number of unique positions the `T` (tail) of the rope have visited at least once.
 */
async function countUniquePositionsOfTail() {
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
    const rope = new Rope(currentPosition);
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

async function solutionTwo() {
    return "";
}

runSolutions(countUniquePositionsOfTail, solutionTwo);
