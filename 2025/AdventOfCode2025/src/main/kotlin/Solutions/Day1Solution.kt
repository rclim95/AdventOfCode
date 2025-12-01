package solutions

import DaySolution
import java.util.Scanner

/**
 * Solves Day 1 of Advent of Code 2025.
 */
object Day1Solution : DaySolution {
    /**
     * Gets the Advent of Code day that this solution implements.
     */
    override val day: Int
        get() = 1

    /**
     * Runs the solution for Part 1 for this day.
     * @param input The standard input.
     * @return The solution to Part 1, based on the provided input.
     */
    override fun runPart1(input: Scanner): String {
        var password: Int = 0
        var dialPosition: Int = 50;

        // Each line should correspond to whether we're turning this 100-tick knob to the left ("L")
        // or right ("R").
        while (input.hasNext()) {
            val currentRotation: String = input.next()
            val rotationTicks: Int = currentRotation.substring(1).toInt()
            dialPosition = when {
                currentRotation.startsWith("L") -> (dialPosition - rotationTicks).mod(100)
                currentRotation.startsWith("R") -> (dialPosition + rotationTicks).mod(100)
                else -> error("Undefined rotation prefix for input \"$currentRotation\"")
            };

            if (dialPosition == 0) {
                // The password is the number of times the dial was at 0.
                password++
            }

            System.err.println("After $currentRotation, dialPosition is now $dialPosition")
        }

        return password.toString()
    }

    /**
     * Runs the solution for Part 2 for this day.
     * @param input The standard input.
     * @return The solution to Part 2, based on the provided input.
     */
    override fun runPart2(input: Scanner): String {
        TODO("Not yet implemented.")
    }
}