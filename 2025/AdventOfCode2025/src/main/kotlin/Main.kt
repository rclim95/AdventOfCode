import solutions.Day0Solution

import com.github.ajalt.clikt.core.CliktCommand
import com.github.ajalt.clikt.core.main
import com.github.ajalt.clikt.parameters.options.help
import com.github.ajalt.clikt.parameters.options.option
import com.github.ajalt.clikt.parameters.options.prompt
import com.github.ajalt.clikt.parameters.types.choice
import com.github.ajalt.clikt.parameters.types.inputStream
import com.github.ajalt.clikt.parameters.types.int
import solutions.Day1Solution
import java.io.InputStream
import java.util.Scanner

class RunCommand : CliktCommand() {
    val input: InputStream by option()
        .inputStream()
        .prompt("Where is your input file located", promptSuffix = "? ")
    val day: Int by option().int()
        .prompt("Which day would you like to run", promptSuffix = "? ")
        .help("The day that should be run.")
    val part: Int by option()
        .choice("1" to 1, "2" to 2)
        .prompt("Which part would you like to run", promptSuffix = "? ")

    companion object {
        private fun solutions(): List<DaySolution> = listOf(
            Day0Solution,
            Day1Solution
        )
    }

    override fun run() {
        // Find the solution the user wants to run
        val solution = solutions()
            .firstOrNull { it.day == day }
        require(solution != null)

        // Create a scanner for the provided answer
        val scanner = solution.createScanner(input)
        val answer = when (part) {
            1 -> solution.runPart1(scanner)
            2 -> solution.runPart2(scanner)
            else -> error("Undefined part: $part")
        }

        println("Answer: $answer")
    }
}

fun main(args: Array<String>) = RunCommand().main(args)