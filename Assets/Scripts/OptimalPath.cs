// using System;
using UnityEngine;
using static System.Math;
using System.Collections;
using System.Collections.Generic;
using Random=System.Random;

public static class OptimalPath {

    public static Vector3 startPoint = new Vector3(24.0F, 0.25F, 0F);
    public static Vector3 finalSpot = new Vector3(-22.0f, 1.0f, 0.0f);

    public static ArrayList djikstra(Dictionary<Vector3, SortedDictionary<float, Vector3>> pToP) {
        Vector3 curPoint = startPoint;
        Dictionary<Vector3, Vector3> mappingsBack = new Dictionary<Vector3, Vector3>();
        float curDistance = 0;
        HashSet<Vector3> alrVisited = new HashSet<Vector3>();
        PriorityQueue<Vector3, float> inLine = new PriorityQueue<Vector3, float>();
        Dictionary<Vector3, float> pointToDistance = new Dictionary<Vector3, float>();

        while (Vector3.Distance(curPoint, finalSpot) != 0.0f) {
            if (pToP.ContainsKey(curPoint) && !alrVisited.Contains(curPoint)) {
                alrVisited.Add(curPoint);
                foreach (KeyValuePair<float, Vector3> entry in pToP[curPoint]) {
                    float key = entry.Key;
                    Vector3 value = entry.Value;
                    if (!alrVisited.Contains(value)) {
                        inLine.Enqueue(value, curDistance + key);
                        if (!pointToDistance.ContainsKey(value) || curDistance + key < pointToDistance[value]) {
                            mappingsBack[value] = curPoint;
                            pointToDistance[value] = curDistance + key;
                        }
                    }
                }
            }
            curPoint = inLine.Dequeue();
            curDistance = pointToDistance[curPoint];
        }
        ArrayList ret = new ArrayList();
        while (Vector3.Distance(curPoint, startPoint) != 0) {
            ret.Insert(0, curPoint);
            curPoint = mappingsBack[curPoint];
        }
        ret.Insert(0, curPoint);
        return ret;
    }

    /**
    Creates a graph with the edges between vertices that the user can go along
    */
    public static void addToGraphNodes(Vector3[] cylinderLocs, Dictionary<Vector3, SortedDictionary<float, Vector3>> allGraphNodes, Vector3 p1, Vector3 p2) {
        if (cylinderNotInWay(cylinderLocs, p1, p2, new Vector3[0])) {
            if (!allGraphNodes.ContainsKey(p1)) {
                allGraphNodes.Add(p1, new SortedDictionary<float, Vector3>());
            }
            allGraphNodes[p1].Add(Vector3.Distance(p1, p2), p2);
        }
    }

    /**
    Finds 
    */
    public static bool cylinderNotInWay(Vector3[] cylinderLocs, Vector3 position1, Vector3 position2, Vector3[] cylinderLocsNotIncluded) {
        float slope = (position2.z - position1.z)/(position2.x - position1.x);
        float perpSlope = -1/slope;
        float intercept = position1.z - slope * position1.x;

        float maxX = Max(position1.x, position2.x);
        float minX = Min(position1.x, position2.x);

        for (int k = 0; k < cylinderLocs.Length; k++) {
            bool continueIt = false;
            for (int i = 0; i < cylinderLocsNotIncluded.Length; i++) {
                if (Vector3.Distance(cylinderLocs[k], cylinderLocsNotIncluded[i]) == 0) continueIt = true;
            }
            if (continueIt) continue;

            float curX = cylinderLocs[k].x;
            float curY = cylinderLocs[k].z;

            float perpIntercept = curY - perpSlope * curX;

            float interceptX = (perpIntercept - intercept)/(slope - perpSlope);
            float interceptY = slope * interceptX + intercept;
            if (interceptX >= minX && interceptX <= maxX && Pow(interceptX - curX, 2) + Pow(interceptY - curY, 2) < Pow(2.5, 2)) {
                return false;
            }
        }
        return true;
    }

    public static Dictionary<Vector3, SortedDictionary<float, Vector3>> cylindersNothingBetween(Vector3[] cylinderLocs) {
        var nothingBetweenPairs = new ArrayList();
        for (int i = 0; i < cylinderLocs.Length; i++) {
            for (int j = i + 1; j < cylinderLocs.Length; j++) {
                bool addThis = cylinderNotInWay(cylinderLocs, cylinderLocs[i], cylinderLocs[j], new Vector3[]{cylinderLocs[i], cylinderLocs[j]});
                if (addThis) {
                    nothingBetweenPairs.Add(new Vector3[2]{cylinderLocs[i], cylinderLocs[j]});
                }
            }
        }
        Dictionary<Vector3, SortedDictionary<float, Vector3>> allGraphNodes = new Dictionary<Vector3, SortedDictionary<float, Vector3>>();
        for (int i = 0; i < nothingBetweenPairs.Count; i++) {
            for (int j = i + 1; j < nothingBetweenPairs.Count; j++) {
                Vector3[] first = (Vector3[]) nothingBetweenPairs[i];
                Vector3[] second = (Vector3[]) nothingBetweenPairs[j];
                Vector3 p1 = AverageVectors(first[0], first[1]);
                Vector3 p2 = AverageVectors(second[0], second[1]);
                addToGraphNodes(cylinderLocs, allGraphNodes, p1, p2);
                addToGraphNodes(cylinderLocs, allGraphNodes, p2, p1);
            }
        }
        //for the startingPoint
        for (int i = 0; i < nothingBetweenPairs.Count; i++) {
            Vector3[] first = (Vector3[]) nothingBetweenPairs[i];
            Vector3 p1 = AverageVectors(first[0], first[1]);
            addToGraphNodes(cylinderLocs, allGraphNodes, startPoint, p1);
        }
        //for the endingPoint
        for (int i = 0; i < nothingBetweenPairs.Count; i++) {
            Vector3[] first = (Vector3[]) nothingBetweenPairs[i];
            Vector3 p1 = AverageVectors(first[0], first[1]);
            addToGraphNodes(cylinderLocs, allGraphNodes, p1, finalSpot);
        }
        //for the starting to end points
        addToGraphNodes(cylinderLocs, allGraphNodes, startPoint, finalSpot);
        return allGraphNodes; 
    }

    public static Vector3 AverageVectors(Vector3 v1, Vector3 v2) {
        float x = (v1.x + v2.x) / 2f;
        float y = (v1.y + v2.y) / 2f;
        float z = (v1.z + v2.z) / 2f;
        return new Vector3(x, y, z);
    }
}


// void drawLines(ArrayList finalList) {
//      for (int i = 0; i < nothingBetweenPairs.Count; i++) {
//         int j = i + 1;
//         GameObject[] x = (GameObject[]) nothingBetweenPairs[i];
//         while (j < nothingBetweenPairs.Count) {
//             GameObject[] y = (GameObject[]) nothingBetweenPairs[j];
//             if (!GameObject.ReferenceEquals(x[0], y[0])) {
//                 break;
//             }
//             j++;
//         }
//         j--;
//         LineRenderer lr = x[0].GetComponent<LineRenderer>();
//         lr.positionCount = j - i + 1;
//         Vector3[] positions = new Vector3[lr.positionCount];
//         for (int z = i; z <= j; z++) {
//             GameObject[] y = (GameObject[]) nothingBetweenPairs[z];
//             positions[z - i] = y[1].transform.position;
//         }
//         Debug.Log("setting positions for " + i + " for position count of " + lr.positionCount);
//         lr.SetPositions(positions);
//         i = j;
//     }

//     return nothingBetweenPairs;
// }