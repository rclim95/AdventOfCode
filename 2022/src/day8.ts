import { readLinesFromStdin, runSolutions } from "./utils/common.ts";

/**
 * Checks to see whether a tree at the specified `row` and `col` is visible or not. A tree is
 * considered visible if the tree isn't blocked by a tree that has a higher height than it, starting
 * from the left / right and top / bottom edges of the grid.
 * @param grid The tree grid.
 * @param row The row of the tree to check.
 * @param col The column of the tree to check.
 */
function isTreeVisible(grid: Array<Array<number>>, row: number, col: number) {
    const thisTreeHeight = grid[row][col];

    // If any attempt to access the specified row or column would lead to an out of bound, then
    // we must be near the edge, so we know the tree is visible. :)
    if (row - 1 < 0 || col - 1 < 0) {
        return true;
    }
    if (row + 1 >= grid.length || col + 1 >= grid[row].length) {
        return true;
    }

    // Figure out what is the tallest tree surrounding our current tree on the left, right, top,
    // and bottom. If our tree's height is less than these maxes, that means the tree is visible.
    const maxTreeHeightLeft = Math.max(...grid[row].slice(0, col));
    const maxTreeHeightRight = Math.max(...grid[row].slice(col + 1, grid[row].length));
    const maxTreeHeightTop = Math.max(...grid.slice(0, row).map(row => row[col]));
    const maxTreeHeightBottom = Math.max(...grid.slice(row + 1, grid.length).map(row => row[col]));

    return thisTreeHeight > maxTreeHeightTop ||
        thisTreeHeight > maxTreeHeightBottom ||
        thisTreeHeight > maxTreeHeightLeft ||
        thisTreeHeight > maxTreeHeightRight;
}

/**
 * Calculates the scenic score of a tree, i.e., the product of how many trees are visible from
 * the top, left, right, and bottom of the current tree before it encounters a tree taller
 * than it.
 * @param grid The grid of tree and their heights. 
 * @param row The row of the tree whose scenic score should be calculated.
 * @param col The colum nof the tree whose scenic score should be calculated.
 */
function calculateScenicScore(grid: Array<Array<number>>, row: number, col: number) {
    // Bail early if we're at the edge, as the scenic score for an edge is 0. :)
    if (row - 1 < 0 || col - 1 < 0) {
        return 0;
    }
    if (row + 1 >= grid.length || col + 1 >= grid[row].length) {
        return 0;
    }

    const thisTreeHeight = grid[row][col];
    let totalTreeVisibleTop = 0;
    for (let rowIndex = row - 1; rowIndex >= 0; rowIndex--) {
        totalTreeVisibleTop++;
        if (grid[rowIndex][col] >= thisTreeHeight) {
            break;
        }
    }

    let totalTreeVisibleBottom = 0;
    for (let rowIndex = row + 1; rowIndex < grid.length; rowIndex++) {
        totalTreeVisibleBottom++;
        if (grid[rowIndex][col] >= thisTreeHeight) {
            break;
        }
    }

    let totalTreeVisibleLeft = 0;
    for (let colIndex = col - 1; colIndex >= 0; colIndex--) {
        totalTreeVisibleLeft++;
        if (grid[row][colIndex] >= thisTreeHeight) {
            break;
        }
    }

    let totalTreeVisibleRight = 0;
    for (let colIndex = col + 1; colIndex < grid[row].length; colIndex++) {
        totalTreeVisibleRight++;
        if (grid[row][colIndex] >= thisTreeHeight) {
            break;
        }
    }

    return totalTreeVisibleTop * totalTreeVisibleLeft * totalTreeVisibleRight * totalTreeVisibleBottom;
}

/**
 * Reads the tree grid from standard input and returns it.
 * @returns An array of array of numbers indicating the tree height. The outer array represents the
 *      rows, and the inner array represents the inner columns, containing the tree heights.
 */
async function readTreeGrid(): Promise<Array<Array<number>>> {
    const rows = [];
    for await (const line of readLinesFromStdin(false)) {
        rows.push(line.split("").map(t => parseInt(t)));
    }
    return rows;
}

/**
 * Count the number of trees that are visible, including those in the edge, as well as interior.
 * @returns The number of visible trees outside the grid.
 */
async function countOfVisibleTrees() {
    const treeGrid = await readTreeGrid();
    let countOfVisibleTrees = 0;
    for (let row = 0; row < treeGrid.length; row++) {
        for (let col = 0; col < treeGrid[row].length; col++) {
            if (isTreeVisible(treeGrid, row, col)) {
                countOfVisibleTrees++;
            }
        }
    }

    return countOfVisibleTrees;
}

async function maxScenicScore() {
    const treeGrid = await readTreeGrid();
    let maxScenicScore = 0;
    for (let row = 0; row < treeGrid.length; row++) {
        for (let col = 0; col < treeGrid[row].length; col++) {
            const scenicScore = calculateScenicScore(treeGrid, row, col);
            if (scenicScore > maxScenicScore) {
                maxScenicScore = scenicScore;
            }
        }
    }

    return maxScenicScore;
}

runSolutions(countOfVisibleTrees, maxScenicScore);
