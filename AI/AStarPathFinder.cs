using System.Collections.Generic;
using UnityEngine;
using System;

namespace TurnBasedStrategy
{
    /// <summary>
    /// Implements the A* pathfinding algorithm for grid-based movement.
    /// Provides methods to find the shortest path between two grid positions, considering walkability and grid bounds.
    /// </summary>
    public class AStarPathfinder: Singleton<AStarPathfinder>
    {
        /// <summary>
        /// Represents a node in the pathfinding graph, storing position, parent, and cost values.
        /// </summary>
        public class Node
        {
            public GridPosition position; // Position of the node on the grid
            public Node parent; // Parent node in the path
            public int gCost; // Cost from start node
            public int hCost; // Heuristic cost to goal
            public int FCost => gCost + hCost; // Total cost

            public Node(GridPosition pos, Node parent, int gCost, int hCost)
            {
                this.position = pos;
                this.parent = parent;
                this.gCost = gCost;
                this.hCost = hCost;
            }
        }

        /// <summary>
        /// Finds the shortest path from start to goal using the A* algorithm.
        /// </summary>
        /// <param name="start">Starting grid position</param>
        /// <param name="goal">Goal grid position</param>
        /// <param name="isWalkable">Function to determine if a grid position is walkable</param>
        /// <param name="gridWidth">Width of the grid</param>
        /// <param name="gridHeight">Height of the grid</param>
        /// <returns>List of grid positions representing the path, or null if no path found</returns>
        public static List<GridPosition> FindPath(GridPosition start, GridPosition goal, System.Func<GridPosition, bool> isWalkable, int gridWidth, int gridHeight)
        {
            var openSet = new List<Node>(); // Nodes to be evaluated
            var closedSet = new HashSet<GridPosition>(); // Evaluated nodes

            Node startNode = new Node(start, null, 0, GetHeuristic(start, goal));
            openSet.Add(startNode);

            while (openSet.Count > 0)
            {
                // Find node with lowest F cost
                Node current = openSet[0];
                for (int i = 1; i < openSet.Count; i++)
                {
                    if (openSet[i].FCost < current.FCost ||
                        (openSet[i].FCost == current.FCost && openSet[i].hCost < current.hCost))
                    {
                        current = openSet[i];
                    }
                }

                // If goal reached, reconstruct and return path
                if (current.position.Equals(goal))
                {
                    return ReconstructPath(current);
                }

                openSet.Remove(current);
                closedSet.Add(current.position);

                // Evaluate neighbors
                foreach (var neighbor in GetNeighbors(current.position, gridWidth, gridHeight))
                {
                    if (!isWalkable(neighbor) || closedSet.Contains(neighbor))
                        continue;

                    int tentativeGCost = current.gCost + 1;
                    Node neighborNode = openSet.Find(n => n.position.Equals(neighbor));
                    if (neighborNode == null)
                    {
                        neighborNode = new Node(neighbor, current, tentativeGCost, GetHeuristic(neighbor, goal));
                        openSet.Add(neighborNode);
                    }
                    else if (tentativeGCost < neighborNode.gCost)
                    {
                        neighborNode.gCost = tentativeGCost;
                        neighborNode.parent = current;
                    }
                }
            }

            // No path found
            return null;
        }

        /// <summary>
        /// Reconstructs the path from the goal node to the start node.
        /// </summary>
        private static List<GridPosition> ReconstructPath(Node node)
        {
            var path = new List<GridPosition>();
            while (node != null)
            {
                path.Add(node.position);
                node = node.parent;
            }
            path.Reverse();
            return path;
        }

        /// <summary>
        /// Calculates the Manhattan distance heuristic between two grid positions.
        /// </summary>
        private static int GetHeuristic(GridPosition a, GridPosition b)
        {
            return Mathf.Abs(a.x - b.x) + Mathf.Abs(a.z - b.z);
        }

        /// <summary>
        /// Returns the neighboring grid positions (up, down, left, right) within grid bounds.
        /// </summary>
        private static IEnumerable<GridPosition> GetNeighbors(GridPosition pos, int gridWidth, int gridHeight)
        {
            var directions = new[]
            {
                new GridPosition(1, 0),
                new GridPosition(-1, 0),
                new GridPosition(0, 1),
                new GridPosition(0, -1)
            };

            foreach (var dir in directions)
            {
                GridPosition neighbor = new GridPosition(pos.x + dir.x, pos.z + dir.z);
                if (neighbor.x >= 0 && neighbor.x < gridWidth && neighbor.z >= 0 && neighbor.z < gridHeight)
                {
                    yield return neighbor;
                }
            }
        }
    }
}