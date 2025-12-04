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
                .filter { (floor( log10(it.toDouble())) + 1).toInt().mod(2) == 0 }
                // To figure out if an ID is invalid, convert the number into a String, split it at the middle, and
                // compare the first half with the second half.
                .filter {
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
        var runningSum: Long = 0

        while (input.hasNextLong()) {
            val firstId = input.nextLong()
            val lastId = input.nextLong()

            // Look for IDs between `firstId` and `lastId` that are invalid.
            val invalidIds = (firstId..lastId)
                // For Part 2, an ID is only considered invalid if it has repeated substrings, i.e., the number is
                // composed of a sequence of digits that are repeated, e.g., 123123 ("123" is repeated) or 1111 ("1"
                // is repeated).
                .filter { hasRepeatedSubstring(it) }
                .toList()

            // Add the invalid IDs to our running sum
            System.err.println("Between $firstId and $lastId, found ${invalidIds.size} invalid IDs: ${invalidIds.joinToString()}")
            runningSum += invalidIds.sum()
        }

        // The sum of all invalid IDs is what we want.
        return runningSum.toString()
    }
}

private fun hasRepeatedSubstring(n: Long): Boolean {
    // To figure out if `n` is composed of a repeated substring, convert `n` into a string, repeat the string twice,
    // and then remove the first and last digits. If we find `n` within the concatenated string, then we know the string
    // is repeated, as the "middle" part of the concatenated string should make up `n` within it (removing the first
    // and last digit prevents the "trivial" match of finding the repeated substring at the beginning or end of the
    // string).
    //
    // Given "525525":
    // 1. Repeat the string twice:          525525525525
    // 2. Remove the first and last digits:  2552552552
    // 3. Look for `n` in the updated string:  525525
    // 4. This is a repeated substring!
    //
    // But given "123":
    // 1. Repeat the string twice:           123123
    // 2. Remove the first and last digits:   2312
    // 3. Look for `n` in the updated string: (No "123")
    // 4. This is not a repeated substring.
    //
    // Clever trick for finding a repeated substring courtesy of this StackOverflow post:
    // https://stackoverflow.com/a/63730237
    val numberAsString = n.toString()
    val repeatedString = numberAsString.repeat(2)
    val substringTest = repeatedString.substring(1, repeatedString.length - 1)

    return substringTest.contains(numberAsString)
}