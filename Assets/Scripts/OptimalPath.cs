using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public static class OptimalPath 
{
    public static Vector3 startPoint = new Vector3(24.0F, 0.25F, 0F);
    public static Vector3 finalSpot = new Vector3(-22.0f, 1.0f, 0.0f);

    // Dijkstra's algorithm to find shortest path
    public static ArrayList Dijkstra(Dictionary<Vector3, SortedDictionary<float, Vector3>> pToP)
    {
        Dictionary<Vector3, float> distances = new Dictionary<Vector3, float>();
        Dictionary<Vector3, Vector3> previous = new Dictionary<Vector3, Vector3>();
        List<Vector3> nodes = new List<Vector3>(pToP.Keys);

        distances[startPoint] = 0;
        

        while (nodes.Count != 0)
        {
            foreach (var node in nodes)
            {
                if (!distances.ContainsKey(node)){
                    distances[node] = float.MaxValue;
                } 
            }

            nodes.Sort((x, y) => distances[x].CompareTo(distances[y]));

            Vector3 smallest = nodes[0];
            nodes.RemoveAt(0);

            if (smallest == finalSpot)
            {
                ArrayList path = new ArrayList();
                while (previous.ContainsKey(smallest))
                {
                    path.Add(smallest);
                    smallest = previous[smallest];
                }
                path.Reverse();
                Debug.Log("here in done");
                return path;
            }

            if (!distances.ContainsKey(smallest))
                continue;

            foreach (var neighbor in pToP[smallest])
            {
                var alt = distances[smallest] + neighbor.Key;
                if (!distances.ContainsKey(neighbor.Value) || alt < distances[neighbor.Value])
                {
                    distances[neighbor.Value] = alt;
                    previous[neighbor.Value] = smallest;
                }
            }
        }
        return new ArrayList();
    }

    public static bool CylinderNotInWay(Vector3[] cylinderLocs, Vector3 position1, Vector3 position2) {
        float slope = (position2.z - position1.z)/(position2.x - position1.x);
        float perpSlope = -1/slope;
        float intercept = position1.z - slope * position1.x;

        float maxX = Mathf.Max(position1.x, position2.x);
        float minX = Mathf.Min(position1.x, position2.x);

        for (int k = 0; k < cylinderLocs.Length; k++) {
            bool continueIt = false;
            if (Vector3.Distance(cylinderLocs[k], position1) == 0 || Vector3.Distance(cylinderLocs[k], position2) == 0) continue;

            float curX = cylinderLocs[k].x;
            float curY = cylinderLocs[k].z;

            float perpIntercept = curY - perpSlope * curX;

            float interceptX = (perpIntercept - intercept)/(slope - perpSlope);
            float interceptY = slope * interceptX + intercept;
            if (interceptX >= minX && interceptX <= maxX && (interceptX - curX) * (interceptX - curX) + (interceptY - curY) * (interceptY - curY) < 2.5 * 2.5) {
                return false;
            }
        }
        return true;
    }

    public static Dictionary<Vector3, SortedDictionary<float, Vector3>> FilterPairsWithoutCylinders(Dictionary<Vector3, ArrayList> pairs, Vector3[] cylinderLocs)
    {
        List<Vector3> averagedPoints = new List<Vector3>();
        foreach (KeyValuePair<Vector3, ArrayList> pair in pairs)
        {
            foreach (var elem in pair.Value) {
                Vector3 elem1 = (Vector3) elem;
                Vector3 avgPoint1 = (pair.Key + elem1) * 0.5f;
                averagedPoints.Add(avgPoint1);
            }
        }

        Dictionary<Vector3, SortedDictionary<float, Vector3>> result = new Dictionary<Vector3, SortedDictionary<float, Vector3>>();
        for (int i = 0; i < averagedPoints.Count; i++)
        {
            for (int j = i + 1; j < averagedPoints.Count; j++)
            {
                Vector3 p1 = averagedPoints[i];
                Vector3 p2 = averagedPoints[j];

                if (CylinderNotInWay(cylinderLocs, p1, p2))
                {
                    float distance = Vector3.Distance(p1, p2);

                    if (!result.ContainsKey(p1))
                        result[p1] = new SortedDictionary<float, Vector3>();
                    result[p1][distance] = p2;

                    if (!result.ContainsKey(p2))
                        result[p2] = new SortedDictionary<float, Vector3>();
                    result[p2][distance] = p1;
                }
            }
        }

        return result;
    }


    public static Dictionary<Vector3, ArrayList> CylindersNothingBetween(Vector3[] cylinderLocs)
    {
        Dictionary<Vector3, ArrayList> graph = new Dictionary<Vector3, ArrayList>();

        for (int i = 0; i < cylinderLocs.Length; i++)
        {
            for (int j = i + 1; j < cylinderLocs.Length; j++)
            {
                Vector3 p1 = cylinderLocs[i];
                Vector3 p2 = cylinderLocs[j];
                if (CylinderNotInWay(cylinderLocs, p1, p2))
                {
                    if (!graph.ContainsKey(p1)) graph[p1] = new ArrayList();
                    graph[p1].Add(p2);
                    if (!graph.ContainsKey(p2)) graph[p2] = new ArrayList();
                    graph[p2].Add(p1);
                }
            }
        }

        // Adding logic for startPoint and finalSpot
        for (int j = 0; j < cylinderLocs.Length; j++)
        {
            Vector3 p1 = startPoint;
            Vector3 p2 = cylinderLocs[j];

            if (CylinderNotInWay(cylinderLocs, p1, p2))
            {
                if (!graph.ContainsKey(p1)) graph[p1] = new ArrayList();
                graph[p1].Add(p2);
            }

            p1 = cylinderLocs[j];
            p2 = finalSpot;

            if (CylinderNotInWay(cylinderLocs, p1, p2))
            {
                if (!graph.ContainsKey(p1)) graph[p1] = new ArrayList();
                graph[p1].Add(p2);
            }
        }

        return graph;
    }


}
