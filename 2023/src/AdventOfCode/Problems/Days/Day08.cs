// <copyright file="Day8.cs" company="rclim95">
// Copyright (c) rclim95
//
// Licensed under the MIT License. For more info, see the LICENSE.md at the root of this repo.
// </copyright>

using System.Text.RegularExpressions;
using Path = (string Left, string Right);

namespace AdventOfCode.Problems.Days.Day08;

/// <summary>
/// Implements the solution for Day 8 of Advent of Code.
/// </summary>
internal class Day08 : IPuzzle
{
    /// <inheritdoc />
    public static int Day => 8;

    /// <inheritdoc />
    public static string GetPart1Answer(TextReader reader)
    {
        // The first line should contain the direction we are supposed to travel (and cycle through
        // if we reached the end).
        string directions = reader.ReadLine() ?? throw new ArgumentException(nameof(reader));

        // The remaining lines should contain all the nodes that describe the network we need to
        // travel through, starting from "AAA" to "ZZZ".
        IDictionary<string, (string Left, string Right)> nodes = GetNodes(reader);
        string currentNode = "AAA";
        int step = 0;

        // Starting from "AAA", count how many steps are needed to reach "ZZZ", following the
        // directions we're provided.
        while (currentNode != "ZZZ")
        {
            Path path = nodes[currentNode];
            char currentDirection = directions[step % directions.Length];
            switch (currentDirection)
            {
                case 'L':
                    currentNode = path.Left;
                    break;
                case 'R':
                    currentNode = path.Right;
                    break;
            }

            step++;
        }

        return step.ToString();
    }

    /// <inheritdoc />
    public static string GetPart2Answer(TextReader reader)
    {
        // The first line should contain the direction we are supposed to travel (and cycle through
        // if we reached the end).
        string directions = reader.ReadLine() ?? throw new ArgumentException(nameof(reader));

        // The remaining lines should contain all the nodes that describe the network we need to
        // travel through.
        IDictionary<string, (string Left, string Right)> nodes = GetNodes(reader);

        // For Part 2, we need to split ourselves like ghost and start at _all nodes_ that ends with
        // A... like a ghost. We need to keep traversing until _all nodes_ end with Z. First step:
        // collect all nodes that ends with A.
        List<string> nodesToTravel = nodes.Keys.Where(k => k.EndsWith("A")).ToList();

        // There's two approach to this: we can either brute force this by going through each node
        // to travel and follow the direction _until_ we reach all nodes that travel to ZZZ (but
        // that will take too long, given the number of directions and steps involved).
        //
        // So instead: if we were to print out at what steps each start node reach the end node,
        // we can see that it takes the same number of steps to reach the end node again, if we
        // were to continue following the directions we're given after reaching the end node (i.e.,
        // going through the start node to the end node and to the end node again is cyclic in nature).
        //
        // With that, figure out how many steps it takes to each the Z node for each node we're
        // travelling individually.
        List<(string StartNode, long StepsToZ)> nodesAndSteps = [];
        foreach (string node in nodesToTravel)
        {
            int step = 0;
            string currentNode = node;
            while (!currentNode.EndsWith("Z"))
            {
                char direction = directions[step % directions.Length];
                Path path = nodes[currentNode];
                switch (direction)
                {
                    case 'L':
                        currentNode = path.Left;
                        break;

                    case 'R':
                        currentNode = path.Right;
                        break;
                }

                step++;
            }

            // Record the number of steps it took.
            nodesAndSteps.Add((node, step));
        }

        // Now that we know the number of steps to reach the end node from the start node (for each
        // starting node), we need to figure out *the* number (our answer) where all these start
        // nodes to end nodes cycle will match at the same time.
        //
        // To do that, go through each starting node, select the node that has the highest step,
        // and then jump to the highest step for that node (and for the node that takes a lesser time
        // to cycle, use modulo to figure out what current step [out of the total step needed to reach
        // end node] it would be on). When all of them reach mod 0, then we know we found the currentStep
        // where all these starting nodes are happily at their end.
        long largestStep = nodesAndSteps.Max(n => n.StepsToZ);
        long currentStep = largestStep;
        while (!nodesAndSteps.TrueForAll(n => currentStep % n.StepsToZ == 0))
        {
            currentStep += largestStep;
        }

        return currentStep.ToString();
    }

    private static IDictionary<string, Path> GetNodes(TextReader reader)
    {
        Dictionary<string, Path> nodes = new();

        while (true)
        {
            string? currentLine = reader.ReadLine();
            if (currentLine == null)
            {
                break;
            }
            else if (currentLine == string.Empty)
            {
                continue;
            }

            // The current line should be in the form of this regular expression, i.e.:
            // NNN = (LLL, RRR)
            // - NNN is the name of the node.
            // - LLL is the name of the node when going left from this node.
            // - RRR is the name of the node when going right from this node.
            //
            // The key in our dictionary will be NNN and the value will be a tuple containing
            // the left and right node.
            Match match = Regex.Match(currentLine, @"(?'name'\w{3}) = \((?'left'\w{3}), (?'right'\w{3})\)");
            string name = match.Groups["name"].Value;
            string left = match.Groups["left"].Value;
            string right = match.Groups["right"].Value;

            nodes.Add(name, (left, right));
        }

        return nodes;
    }
}
