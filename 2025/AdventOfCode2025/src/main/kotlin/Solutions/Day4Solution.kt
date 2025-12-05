package solutions

import DaySolution
import java.util.Scanner

/**
 * Solves Day 4 of Advent of Code 2025.
 */
object Day4Solution : DaySolution {
    /**
     * Gets the Advent of Code day that this solution implements.
     */
    override val day: Int
        get() = 4

    override fun runPart1(input: Scanner): String {
        // Read the entire input into a list, as we'll need to be able to access it in memory to do some
        // adjacency checks.
        val rows = readMutableMap(input)

        // Each "@" represents a roll of paper. "." represents empty space. The forklift can only access "@" if
        // there are less than four rolls of paper adjacent to it. If the toilet paper is near the map's boundary such
        // that trying to access outside the boundary would lead to an "IndexOutOfRangeException", then assume it's
        // an empty space. With that...
        val accessibleRolls = mutableListOf<Position>()
        for ((y, cols) in rows.withIndex()) {
            for ((x, cell) in cols.withIndex()) {
                if (cell != '@') {
                    // Don't care, move on.
                    continue
                }

                // Determine the 8 positions that should be checked and check them to see if we're blocked
                // by other paper rolls.
                val positionsToCheck = listOf(
                    Position(x - 1, y - 1), // Northwest
                    Position(x, y - 1),     // North
                    Position(x + 1, y - 1), // Northeast
                    Position(x - 1, y),     // West
                    Position(x + 1, y),     // East
                    Position(x - 1, y + 1), // Southwest
                    Position(x, y + 1),     // South
                    Position(x + 1, y + 1)  // Southeast
                )
                val paperRollTiles = positionsToCheck.count{ p -> p.isPaperRoll(rows) }
                if (paperRollTiles < 4) {
                    accessibleRolls.add(Position(x, y))
                }
            }
        }

        // Print out map, but replacing all rolls that were deemed accessible with an asterisk.
        for (position in accessibleRolls) {
            rows[position.y][position.x] = '*'
        }
        for (cols in rows) {
            System.err.println(cols.joinToString(separator = ""))
        }

        return accessibleRolls.size.toString()
    }

    override fun runPart2(input: Scanner): String {
        TODO()
    }
}

private fun readMutableMap(input: Scanner): MutableList<MutableList<Char>> {
    val columns = mutableListOf<MutableList<Char>>()
    while (input.hasNextLine()) {
        columns.add(input.nextLine().toMutableList())
    }

    return columns
}

private data class Position(val x: Int, val y: Int) {
    fun isPaperRoll(map: List<List<Char>>): Boolean {
        // If we're going to be out-of-bounds, then we're definitely not a paper roll.
        if (y < 0 || y >= map.size) {
            return false
        }

        if (x < 0 || x >= map[y].size) {
            return false
        }

        return map[y][x] == '@'
    }
}