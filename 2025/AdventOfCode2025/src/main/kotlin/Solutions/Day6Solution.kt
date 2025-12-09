package solutions

import DaySolution
import java.util.Scanner
import java.util.Stack
import kotlin.math.log10

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
        // The input should be a column of numbers on each line. The very last line will contain the operation that we
        // want to perform on all those numbers.
        //
        // Part 2 has a twist though: cephalopods apparently read numbers from right-to-left, column-by-column. So if
        // you see something like:
        //
        // 123 456
        //   1 92
        // +   *
        //
        // This is really:
        // - Column 1: 31 + 2  + 1  = 34
        // - Column 2: 6  * 52 * 49 = 15288
        //
        // Note that spaces do matter here (e.g., the position of 1 vs. 92 on the second row).
        //
        // Because of this, read all lines that are in our input file and index each character in each string to figure
        // out the number to add or multiply. And to take care of spacing issues of each math problem ("how do I know if
        // this is a new math problem column?"), use the last row containing the operators to help us (the operators are
        // aligned to the "start" of each math problem column).
        val lines = buildList() {
            while (input.hasNextLine()) {
                add(input.nextLine())
            }
        }

        var runningSum = 0L
        var currentOperator: Char? = null
        val numbersToAggregate = mutableListOf<Long>()
        val longestLine = lines.maxOf { l -> l.length } // Get around silly trimming issues. :)
        val operationLine = lines.last()
        for (i in 0..<longestLine) {
            if (i < operationLine.length && (operationLine[i] == '*' || operationLine[i] == '+')) {
                if (currentOperator != null) {
                    // We have come across an operation. Aggregate the numbers in our list and apply the operation requested.
                    val answer = when (currentOperator) {
                        '*' -> numbersToAggregate.reduce { acc, n -> acc * n }
                        '+' -> numbersToAggregate.reduce { acc, n -> acc + n }
                        else -> error("Unsupported operator: $currentOperator")
                    }

                    System.err.print(numbersToAggregate.joinToString(separator = " $currentOperator ", postfix = " = "))
                    System.err.println(answer)
                    numbersToAggregate.clear()
                    runningSum += answer
                }

                // Now read in the current operator.
                currentOperator = operationLine[i]
            }

            // Read in the current number from top to bottom. To make this easy, read it as a series of string concatenation.
            var currentNumber = ""
            for (line in lines.take(lines.size - 1)) {
                if (i < line.length) {
                    currentNumber += line[i]
                }
            }
            currentNumber = currentNumber.trim()

            // Make sure currentNumber isn't empty (this can happen if we're about to approach the next math problem column).
            if (!currentNumber.isEmpty()) {
                numbersToAggregate.add(currentNumber.toLong())
            }
        }

        // Don't forget to aggregate the last column!
        val answer = when (currentOperator) {
            '*' -> numbersToAggregate.reduce { acc, n -> acc * n }
            '+' -> numbersToAggregate.reduce { acc, n -> acc + n }
            else -> error("Unsupported operator: $currentOperator")
        }

        System.err.print(numbersToAggregate.joinToString(separator = " $currentOperator ", postfix = " = "))
        System.err.println(answer)
        runningSum += answer

        return runningSum.toString()
    }
}