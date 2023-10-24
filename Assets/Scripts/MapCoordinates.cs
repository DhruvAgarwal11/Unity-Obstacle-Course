using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static System.Math;
public class MapCoordinates {
    private int numCylinders;
    private ArrayList optimalPath;
    private Vector3[] cylinderLocs;

    public int getNumCylinders() {
        return numCylinders;
    }
    public ArrayList getOptimalPath() {
        return optimalPath;
    }

    public Vector3[] getCylinderLocs() {
        return cylinderLocs;
    }

    public MapCoordinates(int numCylinders, List<(int, int)> optimalPath, GameObject[] cylinders) {
        this.numCylinders = numCylinders;
        ArrayList finalOptimalPath = new ArrayList();
        for (int i = 0; i < optimalPath.Count; i++)
        {
            finalOptimalPath.Add(new Vector3(optimalPath[i].Item1, 0, optimalPath[i].Item2));
        }
        this.optimalPath = finalOptimalPath;
        Vector3[] store = new Vector3[cylinders.Length];
        for (int i = 0; i < cylinders.Length; i++) {
            store[i] = cylinders[i].transform.position;
        }
        this.cylinderLocs = store;
    }
    
    public MapCoordinates(int numCylinders, ArrayList optimalPath, Vector3[] cylinderLocs) {
        this.numCylinders = numCylinders;
        this.optimalPath = optimalPath;
        this.cylinderLocs = cylinderLocs;
    }
    
}