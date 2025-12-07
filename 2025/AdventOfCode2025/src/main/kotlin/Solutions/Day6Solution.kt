package solutions

import DaySolution
import java.util.Scanner

/**
 * Solves Day 6 of Advent of Code 2025.
 */
object Day6Solution : DaySolution {
    override val day: Int
        get() = 6

    override fun runPart1(input: Scanner): String {
        var runningSum = 0L

        // The input should be a column of numbers (space-separated) on each line. The very last line will contain
        // the operation that we want to perform on all those numbers.
        //
        // Create a Mutable<MutableList<Long>>, where the outer List represents a column, and the inner MutableList of
        // `long` represents the numbers in that column.
        val columns = mutableListOf<MutableList<Long>>()
        while (input.hasNextLine()) {
            val currentLine = input.nextLine()
            if (currentLine.matches("^[\\d\\s]+$".toRegex())) {
                // We are reading space-separated numbers.
                val numbers = currentLine.split(" ")
                    .map{ s -> s.trim() }
                    .filter { s -> !s.isEmpty() }
                    .map { s -> s.toLong() }
                if (columns.isEmpty()) {
                    // First time we're initializing these columns.
                    columns.addAll(numbers.map { n -> mutableListOf(n) })
                }
                else {
                    // We initialized these columns before. Iterate through the list of columns we initialized already
                    // and add the respective number that was read from the input into it.
                    columns.forEachIndexed { columnIndex, column -> column.add(numbers[columnIndex]) }
                }
            }
            else {
                // We are reading space-separated operations (with "+" meaning add all numbers in the column above and
                // "*" meaning to multiply all numbers in the column above).
                val operations = currentLine.split(" ")
                    .map { s -> s.trim() }
                    .filter { s -> !s.isEmpty() }
                for ((columnIndex, operator) in operations.withIndex()) {
                    val numbers = columns[columnIndex]
                    val result = when (operator) {
                        "+" -> numbers.reduce { acc, n -> n + acc }
                        "*" -> numbers.reduce { acc, n -> n * acc }
                        else -> error("Unexpected operator: $operator")
                    }

                    // For funsies, print out the numbers that were added or multiplied altogether.
                    System.err.println(columns[columnIndex].joinToString(separator = operator, postfix = " = $result"))

                    // We need the running sum of the results
                    runningSum += result
                }
            }
        }

        return runningSum.toString()
    }

    override fun runPart2(input: Scanner): String {
        TODO()
    }
}