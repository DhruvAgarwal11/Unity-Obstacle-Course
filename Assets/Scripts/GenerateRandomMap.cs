using System;
using UnityEngine;
using UnityEngine.UI;
using static System.Math;
using System.Collections;
using System.Collections.Generic;
using Random=System.Random;
// using static OptimalPath;
using static ReadWriteMap;

public static class GenerateRandomMap{

    /**
    Overall function to call for generating the random map
    */
    public static MapCoordinates generateRandomMap(int minNumCylinders, int maxNumCylinders, string filename) {
        Random random = new Random();
        Vector3[] cylinderLocs = new Vector3[(int)(randomlyGeneratedFloat(random, minNumCylinders, maxNumCylinders))];
        for (int i = 0; i < cylinderLocs.Length; i++) {
            RandomCylinderGenerator(cylinderLocs, i);
        }
        ArrayList finalList = OptimalPathNew.GetShortestPath(cylinderLocs);
        Debug.Log(finalList.Count);
        MapCoordinates mpc = new MapCoordinates(cylinderLocs.Length, finalList, cylinderLocs);
        ReadWriteMap.GameWriteMap(filename, mpc);
        ReadWriteMap.GameReadMap(filename);
        return mpc;
    }

    public static MapCoordinates getMap(string filename) {
        Random random = new Random();
        MapCoordinates mpc = ReadWriteMap.GameReadMap(filename);
        return mpc;
    }

    /**
    Determines whether the cylinder is located on top of 
    */
    static void RandomCylinderGenerator(Vector3[] cylinderLocs, int idx) {
        float x;
        float y;
        float z;
        bool overlapping;
        int j = 0;
        do {
            Random random = new Random();
            x = randomlyGeneratedFloat(random, -23.0, 23.0);
            y = 1.0f;
            z = randomlyGeneratedFloat(random, -23.0, 23.0);
            overlapping = false;
            for (int i = 0; i < idx; i++) {
                if (cylinderLocs[i] != null) {
                    float distance = Vector3.Distance(new Vector3(x, y, z), cylinderLocs[i]);
                    if (distance <= 4.2) {
                        overlapping = true;
                        break;
                    }
                }  
            }
            j++;
        } while ((x <= -18.0 && z >= -2.0 && z <= 5.0) || (x > 18.0F && z >= -6.0F && z <= 6.0) || overlapping);
        cylinderLocs[idx] = new Vector3(x, y, z);
    }


    static float randomlyGeneratedFloat(Random random, double start, double end) {
        double randomNumber = random.NextDouble() * (end - start) + start;
        return (float)(randomNumber);
    }

}
