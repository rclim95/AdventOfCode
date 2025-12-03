package solutions

import DaySolution
import java.io.InputStream
import java.util.Scanner
import kotlin.math.floor
import kotlin.math.log10

/**
 * Solves Day 2 of Advent of Code 2025.
 */
object Day2Solution : DaySolution {
    override val day: Int
        get() = 2


    override fun createScanner(input: InputStream): Scanner {
        // NB: Ensure that the Scanner created for this solution uses a dash ("-") or a comma (",") to delimit
        // tokens in addition to whitespace. This will make it easy to parse the number ranges we're expecting. :)
        return super.createScanner(input).useDelimiter("[-,\\s]")
    }

    override fun runPart1(input: Scanner): String {
        var runningSum: Long = 0

        while (input.hasNextLong()) {
            val firstId = input.nextLong()
            val lastId = input.nextLong()

            // Look for IDs between `firstId` and `lastId` that are invalid.
            val invalidIds = (firstId..lastId)
                // Only consider IDs that have an even number of digits. To count the number of digits, take the log10(...)
                // of the ID, floor it, and then add 1.
                .filter { it -> (floor( log10(it.toDouble())) + 1).toInt().mod(2) == 0 }
                // To figure out if an ID is invalid, convert the number into a String, split it at the middle, and
                // compare the first half with the second half.
                .filter {
                    it ->
                        val idAsString = it.toString()
                        val middleIndex = idAsString.length / 2
                        idAsString.take(middleIndex) == idAsString.substring(middleIndex)
                }
                .toList()

            // Add the invalid IDs to our running sum
            System.err.println("Between $firstId and $lastId, found ${invalidIds.size} invalid IDs: ${invalidIds.joinToString()}")
            runningSum += invalidIds.sum()
        }

        // The sum of all invalid IDs is what we want.
        return runningSum.toString()
    }

    override fun runPart2(input: Scanner): String {
        TODO("Not yet implemented.")
    }
}