from dataclasses import dataclass, field
from enum import Enum
import re
import sys
from typing import List, Tuple, Dict
import typing
from advent2021.core import run

class NodeType(Enum):
    START = 0
    END = 1
    BIG = 2
    SMALL = 3

    @classmethod
    def from_name(cls, name: str) -> "NodeType":
        # If the name is "start", then that means this is the start node
        if name == "start":
            return NodeType.START

        # If the name is "end", then that means this is the end node
        elif name == "end":
            return NodeType.END

        # If the name is in lowercase, that means this is a small cave
        elif re.match("[a-z]+", name):
            return NodeType.SMALL

        # If the name is in uppercase, then that means this is a big cave
        elif re.match("[A-Z]+", name):
            return NodeType.BIG

        # We don't know how to classify this node
        raise NotImplementedError(f"Unable to classify node {name}")

@dataclass
class Node:
    name: str
    node_type: NodeType
    neighbors: typing.List["Node"] = field(default_factory=list)

def run_part1(file: typing.TextIO) -> int:
    # Read in our graph and print out the adjacency
    nodes = __build_graph(file)
    print("Neighbors:")
    __print_graph_adjacency(nodes)

    # Traverse through all possible path, going through small caves only once (but big caves
    # multiple times), collecting the available paths based off those criteria. Using a DFS recursive
    # approach here. :)
    start_node = nodes["start"]
    paths: List[List[Node]] = []
    def traverse(start: Node, path: List[Node]):
        path.append(start)

        print((" " * len(path)), start.name)

        # If we've reached the end, save this path to our list and stop
        if start.node_type == NodeType.END:
            paths.append(path[:])
            return

        # Traverse our neighbors, but only if they _don't_ match the following criteria:
        for neighbor in start.neighbors:
            # Is this the starting node? We don't want to visit it again.
            if neighbor.node_type == NodeType.START:
                continue

            # Is this a small cave? We only want to visit it once.
            if neighbor.node_type == NodeType.SMALL and neighbor in path:
                continue

            # Otherwise, traverse.
            traverse(neighbor, path[:])

    traverse(start_node, [])

    print("Paths:", file=sys.stderr)
    for path in paths:
        print("->".join([node.name for node in path]), file=sys.stderr)
    return len(paths)

def run_part2(file: typing.TextIO) -> int:
    # Read in our graph and print out the adjacency
    nodes = __build_graph(file)
    print("Neighbors:")
    __print_graph_adjacency(nodes)

    # Traverse through all possible path, going through a single small cave twice, big caves multiple
    # times, and all other small cave once (different from Part 1, which only allowed us to go to
    # all small caves once), collecting the available paths based off those criteria. Using a DFS
    # recursive approach here. :)
    start_node = nodes["start"]
    paths: List[List[Node]] = []
    def traverse(start: Node, path: List[Node], can_twice: bool = True):
        path.append(start)

        print((" " * len(path)), start.name)

        # If we've reached the end, save this path to our list and stop
        if start.node_type == NodeType.END:
            paths.append(path[:])
            return

        # Traverse our neighbors, but only if they _don't_ match the following criteria:
        for neighbor in start.neighbors:
            # Is this the starting node? We don't want to visit it again.
            if neighbor.node_type == NodeType.START:
                continue

            # Is this a small cave? We can only visit a single small cave *twice*; after that, we
            # can only visit remaining small caves once.
            if neighbor.node_type == NodeType.SMALL:
                visit_count = __count_visits(neighbor, path)
                if can_twice and visit_count < 3:
                    traverse(neighbor, path[:], can_twice=False)
                elif not can_twice and visit_count == 1:
                    continue

            # Otherwise, traverse.
            traverse(neighbor, path[:], can_twice)

    traverse(start_node, [], can_twice=True)

    print("Paths:", file=sys.stderr)
    for path in paths:
        print("->".join([node.name for node in path]), file=sys.stderr)
    return len(paths)

def __build_graph(file: typing.TextIO) -> Dict[str, Node]:
    nodes = {}  # type: Dict[str, Node]
    for line in file:
        # Each line represents a single path: an edge, follow by a dash, follow by another edge. Use
        # this information to build the graph. :)
        (start, end) = line.strip().split("-")

        # Does the start node exist? If not, create it.
        if start not in nodes:
            node_type = NodeType.from_name(start)
            start_node = Node(start, node_type)
            nodes[start] = start_node

        # How about the end node?
        if end not in nodes:
            node_type = NodeType.from_name(end)
            end_node = Node(end, node_type)
            nodes[end] = end_node

        # Now that we've gotten (or created) our nodes, add them as neighbors (if we haven't
        # already). :)
        start_node = nodes[start]
        end_node = nodes[end]

        if end_node not in start_node.neighbors:
            start_node.neighbors.append(end_node)

        if start_node not in end_node.neighbors:
            end_node.neighbors.append(start_node)

    return nodes

def __print_graph_adjacency(nodes: Dict[str, Node]) -> None:
    for node in nodes.values():
        print("    Node", node.name, end=" -> ", file=sys.stderr)
        print(",".join([neighbor.name for neighbor in node.neighbors]), file=sys.stderr)

def __count_visits(node: Node, current_path: List[Node]) -> int:
    count = 0
    for current_node in current_path:
        if current_node.name == node.name:
            count += 1
    return count

run(__package__, run_part1, run_part2)
