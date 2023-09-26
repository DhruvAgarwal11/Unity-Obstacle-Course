using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using static System.Math;
using System;

public static class ReadWriteMap 
{
    public static MapCoordinates GameReadMap(string filename) 
    {
        string filePath = "./" + filename + ".txt";
        int numCylinders = 0;
        ArrayList optimalPath = new ArrayList();
        Vector3[] cylinders = new Vector3[0];
        try
        {
            using (StreamReader sr = new StreamReader(filePath))
            {
                string line;
                int i = 0;
                while ((line = sr.ReadLine()) != null)
                {
                    if (i == 0) 
                    {
                        numCylinders = int.Parse(line);
                        cylinders = new Vector3[numCylinders];
                    } 
                    else if (numCylinders <= i) 
                    {
                        float x = float.Parse(line);
                        float y = float.Parse(sr.ReadLine());
                        float z = float.Parse(sr.ReadLine());
                        cylinders[i] = new Vector3(x, y, z);
                    } 
                    else 
                    {
                        float x = float.Parse(line);
                        float y = float.Parse(sr.ReadLine());
                        float z = float.Parse(sr.ReadLine());
                        optimalPath.Add(new Vector3(x, y, z));
                    }
                    i += 1;
                }
            }
        }
        catch (Exception e)
        {
            Debug.Log("An error occurred: " + e.Message);
        }
        return new MapCoordinates(numCylinders, optimalPath, cylinders);
        
    }

    public static void GameWriteMap(string filename, MapCoordinates mapC) 
    {
        int numCylinders = mapC.getNumCylinders();
        ArrayList optimalPath = mapC.getOptimalPath();
        Vector3[] cylinders = mapC.getCylinderLocs();
        string filePath = "./" + filename + ".txt";
        try
        {
            using (StreamWriter sw = new StreamWriter(filePath))
            {
                sw.WriteLine(numCylinders);
                foreach (Vector3 cylinder in cylinders) 
                {
                    sw.WriteLine(cylinder.x);
                    sw.WriteLine(cylinder.y);
                    sw.WriteLine(cylinder.z);
                }
                for (int i = 0; i < optimalPath.Count; i++) 
                {
                    sw.WriteLine(((Vector3)optimalPath[i]).x);
                    sw.WriteLine(((Vector3)optimalPath[i]).y);
                    sw.WriteLine(((Vector3)optimalPath[i]).z);
                }
            }
            Debug.Log("Data written to the file.");
        }
        catch (Exception e)
        {
            Debug.Log("An error occurred: " + e.Message);
        }
    }
}