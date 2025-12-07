package solutions

import DaySolution

import java.util.Scanner
import kotlin.math.max
import kotlin.math.min

/**
 * Solves Day 5 of Advent of Code 2025.
 */
object Day5Solution : DaySolution {
    override val day: Int
        get() = 5

    override fun runPart1(input: Scanner): String {
        // Read in the database. The input should be composed of three parts:
        // 1. The list of fresh ingredient IDs (as an inclusive range).
        // 2. A blank line (indicating this is the start of ingredient IDs to check).
        // 3. The list of ingredient IDs to check.
        val freshRanges = mutableListOf<Range>()
        while (input.hasNext("(\\d+)-(\\d+)")) {
            val tokenizedLine = input.nextLine().split("-")
            val currentRange = Range(tokenizedLine[0].toLong(), tokenizedLine[1].toLong())

            // Check to see if this range overlaps with another existing range we have. If it does, "union" it with
            // that range so it's already included.
            var foundOverlap = false
            for (i in 0..<freshRanges.size) {
                if (freshRanges[i].overlaps(currentRange)) {
                    freshRanges[i] = freshRanges[i].union(currentRange)
                    foundOverlap = true
                    break
                }
            }
            if (!foundOverlap) {
                freshRanges.add(currentRange)
            }
        }

        // This should be a blank line.
        val blank = input.nextLine();
        require(blank == "")

        var totalFreshIngredients = 0
        while (input.hasNextLong()) {
            // Read in the ingredient ID and see if it is fresh by cross-checking it with our list of fresh ranges.
            val ingredientId = input.nextLong()
            val associatedRange = freshRanges.firstOrNull { r -> r.contains(ingredientId) }
            if (associatedRange != null) {
                System.err.println("Ingredient $ingredientId is fresh because it exists in Freshness Range $associatedRange")
                totalFreshIngredients++
            }
        }

        return totalFreshIngredients.toString()
    }

    override fun runPart2(input: Scanner): String {
        // Read in the database. The input should be composed of three parts:
        // 1. The list of fresh ingredient IDs (as an inclusive range).
        // 2. A blank line (indicating this is the start of ingredient IDs to check).
        // 3. The list of ingredient IDs to check.
        //
        // For Part 2, we only care about the ingredient range though, as we want to count the number of ingredient IDs
        // that are considered fresh (i.e., exists in the range).
        val freshRanges = mutableListOf<Range>()
        while (input.hasNext("(\\d+)-(\\d+)")) {
            val tokenizedLine = input.nextLine().split("-")
            val currentRange = Range(tokenizedLine[0].toLong(), tokenizedLine[1].toLong())

            // Check to see if this range overlaps with another existing range we have. If it does, "union" it with
            // that range so it's already included.
            var foundOverlap = false
            for (i in 0..<freshRanges.size) {
                if (freshRanges[i].overlaps(currentRange)) {
                    freshRanges[i] = freshRanges[i].union(currentRange)
                    foundOverlap = true
                    break
                }
            }
            if (!foundOverlap) {
                freshRanges.add(currentRange)
            }
        }

        // Something that didn't get accounted for Part 1 that needs to be fixed for Part 2: Even though we do our
        // best to check for overlaps and union, it's possible we still have other ranges that could be merged. Sort
        // our ranges by the starting number in ascending order, start from the end, and work our way to the start as
        // we look for overlaps between the current range and the range previous of it. If it overlaps, union the
        // current range with the previous range and remove the current range from our list so we don't double-count it.
        freshRanges.sortWith { r1, r2 -> r1.start.compareTo(r2.start) }
        var i = freshRanges.size - 1
        while (i > 0) {
            val currentRange = freshRanges[i]
            val previousRange = freshRanges[i - 1]
            if (currentRange.overlaps(previousRange)) {
                freshRanges.removeAt(i)
                freshRanges[i - 1] = previousRange.union(currentRange)
            }

            i--
        }

        // For debugging, loop through all the range we found.
        for (range in freshRanges) {
            System.err.println("From ${range.start} -> ${range.end}, there are ${range.count()} fresh ingredients")
        }

        // We are looking for the number of ingredient IDs that are fresh.
        val totalFreshIngredients = freshRanges.sumOf { r -> r.count() }
        return totalFreshIngredients.toString()
    }
}

/**
 * Represents an inclusive range.
 * @param start The start of the range.
 * @param end The end of the range.
 */
private data class Range(val start: Long, val end: Long) {
    /**
     * Gets the number of values that are between [[start]] and [[end]], inclusive.
     * @return The number of values.
     */
    fun count(): Long {
        return this.end - this.start + 1
    }

    /**
     * Checks to see if this range contains the specified [[value]].
     * @param value The value to check.
     * @return `true` if this range includes the [[value]], else `false`.
     */
    fun contains(value: Long): Boolean {
        return value >= this.start && value <= this.end
    }

    /**
     * Checks to see if this range overlaps with another range.
     * @param other The other range to check to see if it overlaps.
     * @return `true` if it overlaps, else `false`.
     */
    fun overlaps(other: Range): Boolean {
        return this.start <= other.end && other.start <= this.end
    }

    /**
     * Unions the range with the other range to create a new [[Range]].
     * @param other The other range to union.
     * @return The new [[Range]]. Note that if this range does not overlap with [[other]] then `null` will be returned.
     * @exception IllegalArgumentException `other` does not overlap with this range.
     */
    fun union(other: Range): Range {
        require(this.overlaps(other))

        val start = min(this.start, other.start);
        val end = max(this.end, other.end)

        return Range(start, end)
    }
}