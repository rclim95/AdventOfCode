package solutions

import DaySolution
import java.util.Scanner
import kotlin.math.abs

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
        var password: Int = 0
        var dialPosition: Int = 50;

        // Each line should correspond to whether we're turning this 100-tick knob to the left ("L")
        // or right ("R").
        while (input.hasNext()) {
            val currentRotation: String = input.next()
            var rotationTicks: Int = currentRotation.substring(1).toInt()

            // For Part 2, it seems that the password method for this problem is based on the number of times
            // the dial has reached zero while rotating--not just when it stopped at zero.
            //
            // We'll need to take that into account. Figure out if we'll be "over" 100 or "under" 0 and implement our
            // own for of "modulo 100" to handle this.
            while (rotationTicks > 0) {
                if (currentRotation.startsWith("L")) {
                    if (dialPosition - rotationTicks < 0) {
                        // We're going to be wrapping around from 0 to 100, so adjust our dialPosition to go to
                        // 100 and update our rotationTicks with the remaining rotations we need to do leftward.
                        //
                        // Note that we don't want to increment our password if dialPosition is 0 (the previous
                        // rotation step should've counted it).
                        if (dialPosition != 0) {
                            password++
                            rotationTicks -= dialPosition
                            System.err.println("During $currentRotation, dialPosition rotated to 0. ${rotationTicks} ticks remain.")
                        }

                        dialPosition = 100
                    }
                    else {
                        // No wrap around occurred, rotate left (subtract) as-is.
                        dialPosition -= rotationTicks
                        rotationTicks = 0

                        // If our dialPosition is zero after applying our rotation, make sure to increment our
                        // password!
                        if (dialPosition == 0) {
                            password++
                        }

                        System.err.println("After $currentRotation, dialPosition is now $dialPosition.")
                    }
                }
                else if (currentRotation.startsWith("R")) {
                    if (dialPosition + rotationTicks > 99) {
                        // We're going to be wrapping around from 100 to 0, so adjust our dialPosition to go to 0
                        // and update our rotationTicks with the remaining rotations we need to do rightward.
                        password++
                        rotationTicks -= 100 - dialPosition
                        dialPosition = 0

                        System.err.println("During $currentRotation, dialPosition rotated to 0. ${rotationTicks - dialPosition} ticks remain.")
                    }
                    else {
                        // No wrap around occurred, rotate right (add) as-is.
                        dialPosition += rotationTicks
                        rotationTicks = 0

                        System.err.println("After $currentRotation, dialPosition is now $dialPosition.")
                    }
                }
            }
        }

        return password.toString()
    }
}