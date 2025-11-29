package solutions

import DaySolution
import java.util.Scanner

/**
 * Solves Day 0 of Advent of Code 2025.
 *
 * Note that this is a sample and doesn't correspond to an actual Advent of Code day.
 */
object Day0Solution : DaySolution {
    /**
     * Gets the Advent of Code day that this solution implements.
     */
    override val day: Int
        get() = 0

    /**
     * Runs the solution for Part 1 for this day.
     * @param input The standard input.
     * @return The solution to Part 1, based on the provided input.
     */
    override fun runPart1(input: Scanner): String {
        // This is some example code to sum some numbers.
        var count = 0;
        while (input.hasNextInt()) {
            count += input.nextInt()
        }

        return count.toString()
    }

    /**
     * Runs the solution for Part 2 for this day.
     * @param input The standard input.
     * @return The solution to Part 2, based on the provided input.
     */
    override fun runPart2(input: Scanner): String {
        // This is some example code to count the number of even numbers.
        var count = 0;
        while (input.hasNextInt()) {
            val numberEntered = input.nextInt()
            if (numberEntered % 2 == 0) {
                count++
            }
        }

        return count.toString()
    }
}