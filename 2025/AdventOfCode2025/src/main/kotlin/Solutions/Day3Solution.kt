package solutions

import DaySolution
import java.util.Scanner
import kotlin.math.max
import kotlin.math.pow

/**
 * Solves Day 3 of Advent of Code 2025.
 */
object Day3Solution : DaySolution {
    override val day: Int
        get() = 3

    override fun runPart1(input: Scanner): String {
        var totalOutputJoltage = 0

        // Each line represents a battery bank, and each digit within the "power level" of the battery. We need to
        // turn on exactly two batteries. The "joltage" is the concatenation of the digits of the two batteries that
        // were turned on *in order* (i.e., batteries can't be rearranged).
        while (input.hasNextLine()) {
            val currentBank = input.nextLine()

            // Find the two batteries that we need to turn on to get the highest joltage. Convert currentBank into
            // a list of Battery (which includes the digits it holds and the index it was found) and sort them by the
            // digit it holds in descending order, with the battery with the highest digit being first.
            val sortedBatteries = currentBank
                .withIndex()
                .map { (i, digit) -> Battery(digit.digitToInt(), i) }
                .sortedByDescending { it -> it.digit }
                .toList()

            val largestBattery = sortedBatteries[0]

            // Now that we know the largest battery, we need to determine whether this battery should be first or
            // second. To do that, partition the banks into two sides: batteries to the "left" of `largestBattery`
            // and batteries to the "right" of `largestBatteries`. Then look for the "largest" batteries for each side.
            //
            // Take care of obvious cases: if `largestBattery`'s index is 0 or the last index, then `largestBattery`
            // has to be first or second digit, respectively. Look at the second largest battery in our `sortedBatteries`
            // list to figure out our total joltage.
            if (largestBattery.index == 0 || largestBattery.index == sortedBatteries.size - 1) {
                val secondLargestBattery = sortedBatteries[1]
                val joltage = if (largestBattery.index == 0)
                    (largestBattery.digit * 10) + secondLargestBattery.digit else
                    (secondLargestBattery.digit * 10) + largestBattery.digit

                totalOutputJoltage += joltage
                System.err.println("$currentBank -> Joltage: $joltage")
            }
            else {
                val largestLeft = sortedBatteries
                    .filter{ battery -> battery.index < largestBattery.index }
                    .maxBy { battery -> battery.digit }
                val largestRight = sortedBatteries
                    .filter { battery -> battery.index > largestBattery.index }
                    .maxBy { battery -> battery.digit }

                // Left or right--who will give us the most power?!
                val joltageLeft = (largestLeft.digit * 10) + largestBattery.digit
                val joltageRight = (largestBattery.digit * 10) + largestRight.digit
                val joltage = max(joltageLeft, joltageRight)

                totalOutputJoltage += joltage
                System.err.println("$currentBank -> Joltage: $joltage")
            }
        }

        return totalOutputJoltage.toString()
    }

    override fun runPart2(input: Scanner): String {
        var totalOutputJoltage = 0L

        // Each line represents a battery bank, and each digit within the "power level" of the battery. Unlike the
        // previous part, we need to turn on exactly *twelve* batteries. The "joltage" is the concatenation of the digits
        // of the twelve batteries that were turned on *in order*.
        while (input.hasNextLine()) {
            val currentBank = input.nextLine()

            // Convert currentBank into a list of `Battery` holding the digit its hold and the index it was found at.
            // Whereas in Part 1 we sorted by the digit the battery holds, this time we *don't* want to do that and
            // leave it as-is, i.e., sorted by the index it was found out.
            val batteries = currentBank
                .withIndex()
                .map { (i, digit) -> Battery(digit.digitToInt(), i) }
                .toList()

            // To figure out the twelve batteries that we need to turn on for Maximum Power™️, let's consider the
            // wonderful properties of numbers and how they work.
            //
            // If we were to look at a number from left to right, we know that the digits on the left carries more
            // significance than the right. So if we want to make big numbers, we want to grab the highest digits we
            // can get from the left. Courtesy of this visualization for clueing me in on this property:
            // https://www.reddit.com/r/adventofcode/comments/1pdh1fy/2025_day_3_part_2_visualization/
            //
            // So with that in mind, let the largest joltage represent the last twelve batteries
            // in the bank (for now).
            val largestJolt = batteries
                .subList(batteries.size - 12, batteries.size)
                .toMutableList()

            // Now, let's re-evaluate the twelve batteries and see if we can make larger numbers by look for a larger
            // digit to the left of the current battery we're evaluating.
            var startIndex = 0
            for (n in 0..<12) {
                // If we do find a larger digit to the left, then we'll let that larger battery be our new digit. Note
                // that we'll shrink our search window as we only focus on batteries that are to the right of the
                // batteries that were moved and the remaining batteries on the left that needs to be evaluated for a
                // "new max".
                val largestBattery = batteries.subList(startIndex, largestJolt[n].index + 1).maxBy { b -> b.digit }
                if (largestBattery == largestJolt[n]) {
                    // We are the largest battery, so this digit position (at `n`) can't be moved anymore. This also
                    // means any remaining digits (i.e., digits whose place are after `n`) can't be moved either. We
                    // can stop moving now.
                    break
                }

                largestJolt[n] = largestBattery
                startIndex = largestBattery.index + 1
            }

            // We should now have our largest jolt.
            val largestJoltAsLong = largestJolt
                .withIndex()
                .sumOf { (i, battery) -> (battery.digit * 10.0.pow(11 - i)).toLong() }
            totalOutputJoltage += largestJoltAsLong
            System.err.println("$currentBank -> Joltage: $largestJoltAsLong")
        }

        return totalOutputJoltage.toString()
    }
}

/**
 * Encapsulates a battery in a bank.
 * @param digit The digit that this battery have.
 * @param index The index that this battery was found in the current bank being processed.
 */
private data class Battery(val digit: Int, val index: Int);