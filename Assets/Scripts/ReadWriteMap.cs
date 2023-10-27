using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using static System.Math;
using System;
using Newtonsoft.Json;

public static class ReadWriteMap 
{
    public static MapCoordinates GameReadMap(string filename) 
    {
        string filePath = Path.Combine(Application.persistentDataPath, filename + ".json");
        int numCylinders = 0;
        ArrayList optimalPath = new ArrayList();
        Vector3[] cylinders = new Vector3[0];

        try
        {
            string jsonString = File.ReadAllText(filePath);
            var jsonData = JsonConvert.DeserializeObject<JsonData>(jsonString);

            numCylinders = jsonData.NumCylinders;
            cylinders = new Vector3[numCylinders];

            for (int i = 0; i < numCylinders; i++)
            {
                cylinders[i] = new Vector3(jsonData.Cylinders[i].X, jsonData.Cylinders[i].Y, jsonData.Cylinders[i].Z);
            }

            foreach (var path in jsonData.OptimalPath)
            {
                optimalPath.Add(new Vector3(path.X, path.Y, path.Z));
            }
        }
        catch (Exception e)
        {
            Debug.Log("An error occurred: " + e.Message);
        }
        return new MapCoordinates(numCylinders, optimalPath, cylinders);
    }

    [Serializable]
    public class JsonData
    {
        public int NumCylinders;
        public VectorData[] Cylinders;
        public VectorData[] OptimalPath;
    }

    [Serializable]
    public class VectorData
    {
        public float X;
        public float Y;
        public float Z;
    }

    
    public static void GameWriteMap(string filename, MapCoordinates mapC) 
    {
        int numCylinders = mapC.getNumCylinders();
        ArrayList optimalPath = mapC.getOptimalPath();
        Vector3[] cylinders = mapC.getCylinderLocs();
        string filePath = Path.Combine(Application.persistentDataPath, filename + ".json");
        (new FileInfo(filePath)).Directory.Create();

        // Create a data structure for JSON serialization
        var convertedCylinders = Array.ConvertAll(cylinders, cylinder => new VectorData 
        {
            X = cylinder.x,
            Y = cylinder.y,
            Z = cylinder.z
        });
        var jsonData = new 
        {
            NumCylinders = numCylinders,
            Cylinders = Array.ConvertAll(cylinders, cylinder => new 
            {
                X = cylinder.x,
                Y = cylinder.y,
                Z = cylinder.z
            }),
            OptimalPath = Array.ConvertAll(optimalPath.ToArray(), path => new 
            {
                X = ((Vector3)path).x,
                Y = ((Vector3)path).y,
                Z = ((Vector3)path).z
            })
        };

        // Convert the data structure to a JSON string
        string jsonString = JsonConvert.SerializeObject(jsonData);
        try
        {
            File.WriteAllText(filePath, jsonString);
            Debug.Log(filePath);
            Debug.Log("Data written to the file.");
        }
        catch (Exception e)
        {
            Debug.Log("An error occurred: " + e.Message);
        }
    }
}