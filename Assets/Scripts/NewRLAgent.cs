using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using static Score;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;
using System;

public class NewRLAgent : Agent
{
    Rigidbody cube_Rigidbody;
    public Text LivesRemainingText;
    public Text GameOverText;
    public float speed = 3.5f;
    float x;
    float y;
    float z;
    Vector3 pos;
    float timePrev;
    float numFramesBeforeScoreDecrease;
    int minNumCylinders;
    int maxNumCylinders;
    public GameObject cylinder;
    GameObject[] cylinders;
    float[] xVals;
    float[] yVals;
    float[] zVals;
    // public int numTrials;
    // int curTrialNum;
    Vector3 movement;
    // bool gameOver;
    int numFramesBeforeNextTrial;
    double lastVal;
    [SerializeField] private Transform targetTransform;
    
    // Start is called before the first frame update
    public override void OnEpisodeBegin()
    {
        numFramesBeforeScoreDecrease = 2000;

        //number of obstacles
        minNumCylinders = 60;
        maxNumCylinders = 70;

        //find the cube in the scene
        cube_Rigidbody = GetComponent<Rigidbody>();

        // gameOver = false;
        Score.scoreStart(10);

        Score.displayGameOver(GameOverText, "");

        //Reset cube location
        x = 24.0F;
        y = 0.25F;
        z = UnityEngine.Random.Range(-4, 4);
        pos = new Vector3(x, y, z);
        transform.position = pos;

        //Time between previous frame and current frame
        timePrev = 0;    

        if (cylinders != null) {
            foreach (GameObject curCylinder in cylinders) {
                if (curCylinder != null) {
                    Destroy(curCylinder);
                }  
            }
        }
        
        //Generate random cylinders across the board
        cylinders = new GameObject[60];
        xVals = new float[cylinders.Length];
        yVals = new float[cylinders.Length];
        zVals = new float[cylinders.Length];
        for (int i = 0; i < cylinders.Length; i++) {
            RandomCylinderGenerator(i);
        }

        transform.position = new Vector3(x, y, z);
        lastVal = Math.Pow(transform.position.x + 22, 2) + Math.Pow(transform.position.z -2, 2);
        Debug.Log(transform.position);
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        sensor.AddObservation(transform.position);
        sensor.AddObservation(targetTransform.position);
        for (int i = 0; i < cylinders.Length; i++) {
            sensor.AddObservation(cylinders[i].transform.position);
        }
    }

    public override void OnActionReceived(ActionBuffers actions)
    {
        float moveX = actions.ContinuousActions[0];
        float moveZ = actions.ContinuousActions[1];
        movement = new Vector3(moveX, 0, moveZ);
        transform.position += movement * Time.deltaTime * speed;
        double newVal = Math.Pow(transform.position.x + 22, 2) + Math.Pow(transform.position.z -2, 2);
        if (newVal < lastVal) {
            AddReward(+50);
        }
        else {
            AddReward(+25);
        }
        lastVal = newVal;
        // Debug.Log(transform.position);
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.name == "Player")
        {
            Destroy(other.gameObject);
        }
    }

    void OnTriggerStay(Collider other) {
        //move the object back
        transform.position -= movement * Time.deltaTime * speed;
        numFramesBeforeScoreDecrease--;
        AddReward(-20f);
        if (numFramesBeforeScoreDecrease == 0) {
            EndEpisode();
            Score.decreaseScore(GameOverText);
            numFramesBeforeScoreDecrease = Application.targetFrameRate;
        }
    }

    // Update is called once per frame
    void Update()
    {
        displayScore(LivesRemainingText);
        timePrev = Time.deltaTime;
        if (checkGameEnd(transform.position.x, transform.position.z)) {
            Score.displayGameOver(GameOverText, "You reached the target! Congrats!");
            AddReward(10000);
            EndEpisode();
        }
        else if (Score.getScore() == 0) {
            // gameOver = true;
            AddReward(-500);
            EndEpisode();
            return;
        }
    }

    void RandomCylinderGenerator(int idx) 
    {
        float x = UnityEngine.Random.Range(-23.0F, 23.0F);
        float y = 1.0f;
        float z = UnityEngine.Random.Range(-23.0f, 23.0f);
        while ((x <= -18.0 && z >= -2.0 && z <= 5.0) || (x > 22.0F && z >= -6.0F && z <= 6.0)) {
            x = UnityEngine.Random.Range(-23.0F, 23.0F);
            y = 1.0f;
            z = UnityEngine.Random.Range(-23.0f, 23.0f);
        }
        cylinders[idx] = Instantiate(cylinder, new Vector3(x, y, z), Quaternion.identity);
        xVals[idx] = x;
        yVals[idx] = y;
        zVals[idx] = z;
    }

    bool checkGameEnd(float x, float z) {
        return x <= -20 && z >= -0.4 && z <= 4.1;
    }
    
}
