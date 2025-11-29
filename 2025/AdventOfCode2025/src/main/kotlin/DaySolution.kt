import java.util.Scanner

/**
 * Provides a common interface for an Advent of Code puzzle for a specific day.
 */
interface DaySolution {
    /**
     * Gets the Advent of Code day that this solution implements.
     */
    val day: Int

    /**
     * Runs the solution for Part 1 for this day.
     * @param input The standard input.
     * @return The solution to Part 1, based on the provided input.
     */
    fun runPart1(input: Scanner): String

    /**
     * Runs the solution for Part 2 for this day.
     * @param input The standard input.
     * @return The solution to Part 2, based on the provided input.
     */
    fun runPart2(input: Scanner): String
}