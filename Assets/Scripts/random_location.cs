using UnityEngine;
using UnityEngine.UI;
using static System.Math;
using System.Collections;
using System.Collections.Generic;

using static Score;
using static ReadWriteMap;

public class random_location : MonoBehaviour 
{
    MapCoordinates mapCoordinates;
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
    public int numTrials;
    int curTrialNum;
    Vector3 movement;
    bool gameOver;
    int numFramesBeforeNextTrial;
    Vector3 finalSpot = new Vector3(-22.0f, 1.0f, 0.0f);
    ArrayList finalList;
    int curPointGoingTo = 0;
    
    // Start is called before the first frame update
    void Start() 
    {
        Application.targetFrameRate = 50;
        numFramesBeforeScoreDecrease = Application.targetFrameRate;

        numTrials = 10;

        //number of obstacles
        minNumCylinders = 60;
        maxNumCylinders = 60;

        //find the cube in the scene
        cube_Rigidbody = GetComponent<Rigidbody>();
        
        //start the first trial
        curTrialNum = 1;
        StartNewTrial();
    }

    void StartNewTrial() 
    {
        gameOver = false;
        numFramesBeforeNextTrial = 200;
        Score.scoreStart(10);

        Score.displayGameOver(GameOverText, "");

        //Reset cube location
        x = 24.0F;
        y = 0.25F;
        z = 0.0F;
        pos = new Vector3(x, y, z);
        transform.position = pos;

        //Time between previous frame and current frame
        timePrev = 0;

        //Destroy cylinders 
        if (curTrialNum != 1) 
        {
            foreach (GameObject curCylinder in cylinders) 
            {
                if (curCylinder != null) 
                {
                    Destroy(curCylinder);
                }  
            }
        }
        
        //Generate random cylinders across the board
        cylinders = new GameObject[Random.Range(minNumCylinders, maxNumCylinders)];
        for (int i = 0; i < cylinders.Length; i++) 
        {
            RandomCylinderGenerator(i);
        }
        Dictionary<Vector3, SortedDictionary<float, Vector3>> pointsToOtherPoints = cylindersNothingBetween();
        finalList = djikstra(pointsToOtherPoints);
        mapCoordinates = new MapCoordinates(cylinders.Length, finalList, cylinders);
        // ReadWriteMap.GameWriteMap("out", mapCoordinates)
        foreach(var a in finalList) 
        {
            Debug.Log(a);
        }
    }

    void OnTriggerEnter(Collider other) 
    {
        if (other.gameObject.name == "Player")
        {
            Destroy(other.gameObject);
        }
    }

    void OnTriggerStay(Collider other) 
    {
        //move the object back
        transform.Translate(-movement * Time.deltaTime * speed);
        numFramesBeforeScoreDecrease--;
        if (numFramesBeforeScoreDecrease == 0) 
        {
            Score.decreaseScore(GameOverText);
            numFramesBeforeScoreDecrease = Application.targetFrameRate;
        }
    }

    // Update is called once per frame
    void Update() {
        if (curPointGoingTo < finalList.Count) {
            if (transform.position != (Vector3) finalList[curPointGoingTo]) {
                transform.position = Vector3.MoveTowards(transform.position, (Vector3) finalList[curPointGoingTo], Time.deltaTime*speed);
            } else {
                curPointGoingTo++;
            }
        }

        displayScore(LivesRemainingText);
        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");
        timePrev = Time.deltaTime;
        if (!gameOver) 
        {
            if (checkGameEnd(transform.position.x, transform.position.z)) 
            {
                Score.displayGameOver(GameOverText, "You reached the target! Congrats!");
                gameOver = true;
                return;
            }
            else if (Score.getScore() == 0) 
            {
                gameOver = true;
                return;
            }
            MoveObject(x, z, timePrev);
        }
        else 
        {
            numFramesBeforeNextTrial--;
            if (numFramesBeforeNextTrial == 0) 
            {
                if (curTrialNum < numTrials) 
                {
                    curTrialNum++;
                    StartNewTrial();
                }
                else 
                {
                    Score.displayGameOver(GameOverText, "Congrats, you finished all trials!");
                    Debug.Log("Congrats, you finished all trials!");
                    Application.Quit();
                    Debug.Break(); //remove in production
                }
            }
        }
    }

    void MoveObject(float x, float z, float time = 1) 
    {
        movement = new Vector3(x, 0, z);
        movement = Vector3.ClampMagnitude(movement, 1);
        transform.Translate(movement * speed * time);
    }

    bool checkGameEnd(float x, float z) 
    {
        return x <= -20 && z >= -0.4 && z <= 4.1;
    }
    
}
