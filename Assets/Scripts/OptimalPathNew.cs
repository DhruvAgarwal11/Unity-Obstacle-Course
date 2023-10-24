using System;
using System.Collections.Generic;
using UnityEngine;
using System.Collections;

public class Node : IComparable<Node> {
    public int X, Y;
    public double Distance;
    public Node Parent;

    public Node(int x, int y, double distance, Node parent = null) {
        X = x;
        Y = y;
        Distance = distance;
        Parent = parent;
    }

    public int CompareTo(Node other) {
        return Distance.CompareTo(other.Distance);
    }
}

public static class OptimalPathNew {
    private const int DIM = 480;
    private const float INCREMENT = 0.1f;
    private static readonly (int, int)[] MOVES = {
        (1, 0), (-1, 0), (0, 1), (0, -1),
        (1, 1), (1, -1), (-1, 1), (-1, -1)
    };
    private static readonly Vector3 StartPoint = new Vector3(24.0F, 1.0F, 0F);
    private static readonly Vector3 FinalSpot = new Vector3(-22.0f, 1.0f, 0.0f);

    public static Vector3 GridToUnityCoord(int x, int y) {
        return new Vector3((x - DIM/2) * INCREMENT, 1.0F, (y - DIM/2) * INCREMENT);
    }

    public static (int, int) UnityCoordToGrid(Vector3 pos) {
        return (Mathf.RoundToInt((pos.x / INCREMENT) + DIM/2), Mathf.RoundToInt((pos.z / INCREMENT) + DIM/2));
    }

    public static ArrayList GetShortestPath(Vector3[] cylinderLocs) {
        (int, int) start = UnityCoordToGrid(StartPoint);
        (int, int) end = UnityCoordToGrid(FinalSpot);
        Debug.Log(start);
        Debug.Log(end);
        double[,] distances = new double[DIM, DIM];
        for (int i = 0; i < DIM; i++)
        for (int j = 0; j < DIM; j++)
            distances[i, j] = double.MaxValue;

        distances[start.Item1, start.Item2] = 0;

        SortedSet<Node> queue = new SortedSet<Node> { new Node(start.Item1, start.Item2, 0) };
        bool[,] visited = new bool[DIM, DIM];
        
        while (queue.Count > 0) {
            Debug.Log(queue.Count);
            var current = queue.Min;
            queue.Remove(current);

            if (visited[current.X, current.Y]) continue;
            visited[current.X, current.Y] = true;

            Vector3 pos1 = GridToUnityCoord(current.X, current.Y);

            foreach (var move in MOVES) {
                int newX = current.X + move.Item1;
                int newY = current.Y + move.Item2;
                Vector3 pos2 = GridToUnityCoord(newX, newY);
                
                if (0 <= newX && newX < DIM && 0 <= newY && newY < DIM && !visited[newX, newY])
                {
                    // Debug.Log(pos1 + " " + pos2);
                    if (CylinderNotInWay(cylinderLocs, pos1, pos2)) {
                        double cost = (move.Item1 != 0 && move.Item2 != 0) ? Math.Sqrt(2) : 1;
                        
                        if (distances[current.X, current.Y] + cost < distances[newX, newY]) {
                            distances[newX, newY] = distances[current.X, current.Y] + cost;
                            queue.Add(new Node(newX, newY, distances[newX, newY], current));
                        }
                    }
                }
            }
        }

        ArrayList optimPath = new ArrayList();
        Node node = new Node(end.Item1, end.Item2, distances[end.Item1, end.Item2]);
        while (node != null)
        {
            optimPath.Insert(0, GridToUnityCoord(node.X, node.Y));
            node = node.Parent;
        }
        return optimPath;
    }

    public static bool CylinderNotInWay(Vector3[] cylinderLocs, Vector3 position1, Vector3 position2) {
        float slope = (position2.z - position1.z) / (position2.x - position1.x);
        float perpSlope = -1 / slope;
        float intercept = position1.z - slope * position1.x;

        float maxX = Mathf.Max(position1.x, position2.x);
        float minX = Mathf.Min(position1.x, position2.x);

        for (int k = 0; k < cylinderLocs.Length; k++) {
            float curX = cylinderLocs[k].x;
            float curY = cylinderLocs[k].z;

            float perpIntercept = curY - perpSlope * curX;

            float interceptX = (perpIntercept - intercept) / (slope - perpSlope);
            float interceptY = slope * interceptX + intercept;
            if (interceptX >= minX && interceptX <= maxX && (interceptX - curX) * (interceptX - curX) + (interceptY - curY) * (interceptY - curY) < 2.5 * 2.5) {
                return false;
            }
        }
        return true;
    }
}
