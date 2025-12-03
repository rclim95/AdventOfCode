import java.io.InputStream
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
     * Creates a [[Scanner]] that should be passed to [[runPart1]] and [[runPart2]].
     *
     * This method can be implemented so that a customized [[Scanner]] (e.g., one that uses a different kind of
     * delimiter to separate tokens) will be used when running the solutions to Part 1 and Part 2.
     *
     * The default implementation will return a [[Scanner]] with the default delimiter.
     */
    fun createScanner(input: InputStream): Scanner {
        return Scanner(input)
    }

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