using UnityEngine;
using UnityEngine.UI;
using static System.Math;
using System.Collections;
using System.Collections.Generic;

using static Score;
using static ReadWriteMap;
using static GenerateRandomMap;

public class random_location : MonoBehaviour 
{
    public Text LivesRemainingText;
    public GameObject cylinder;
    public Text GameOverText;
    public string ExperimentType; // define different types of trials that we want to have people do
    public int minNumCylinders;
    public int maxNumCylinders;
    public int numTrials;
    float speed = 3.5f;
    MapCoordinates mapCoordinates;
    Rigidbody cube_Rigidbody;
    float x;
    float y;
    float z;
    Vector3 pos;
    float timePrev;
    float numFramesBeforeScoreDecrease;
    GameObject[] cylinders;
    int curTrialNum;
    Vector3 movement;
    bool gameOver;
    int numFramesBeforeNextTrial;
    Vector3 finalSpot = new Vector3(-22.0f, 1.0f, 0.0f);
    ArrayList finalList;
    int curPointGoingTo;
    bool startTrial;
    
    // Start is called before the first frame update
    void Start() 
    {
        Application.targetFrameRate = 50;
        numFramesBeforeScoreDecrease = Application.targetFrameRate;
        //find the cube in the scene
        cube_Rigidbody = GetComponent<Rigidbody>();
        //start the first trial
        curTrialNum = 1;
        startTrial = false;
        MainRunner();
    }

    void MainRunner() 
    {
        if (curTrialNum <= numTrials)
        {
            //startTrial = false;
            StartCoroutine(StartNewTrial());
            //startTrial = true;
        }
    }

    IEnumerator StartNewTrial() 
    {
        // gameOver = false;
        // numFramesBeforeNextTrial = 200;
        // Score.scoreStart(10);
        // Score.displayGameOver(GameOverText, "");
        // //Reset cube location
        // x = 24.0F;
        // y = 0.25F;
        // z = 0.0F;
        // pos = new Vector3(x, y, z);
        // transform.position = pos;

        // //Time between previous frame and current frame
        // timePrev = 0;

        // //Destroy cylinders 
        // if (curTrialNum != 1) 
        // {
        //     foreach (GameObject curCylinder in cylinders) 
        //     {
        //         if (curCylinder != null) 
        //         {
        //             Destroy(curCylinder);
        //         }  
        //     }
        // }
        
        //Generate random cylinders across the board
        while (curTrialNum < numTrials) {
            string filename = "./trial/" + (curTrialNum + 945);
            //For Writing to a file
            mapCoordinates = GenerateRandomMap.generateRandomMap(minNumCylinders, maxNumCylinders, filename);
            curTrialNum += 1;
        }
        
        yield return null;

        //For Reading from the file
        // mapCoordinates = GenerateRandomMap.getMap("/Users/dhruv/Library/Application Support/DefaultCompany/Obstacle Course/trial/1");

        // finalList = mapCoordinates.getOptimalPath();
        // cylinders = new GameObject[mapCoordinates.getNumCylinders()];
        // Debug.Log(cylinders.Length);
        // Vector3[] cylinderLocs = mapCoordinates.getCylinderLocs();
        // for (int i = 0; i < cylinderLocs.Length; i++) {
        //     cylinders[i] = Instantiate(cylinder, cylinderLocs[i], Quaternion.identity);
        // }
        // curPointGoingTo = 0;
        // ReadWriteMap.GameWriteMap("out", mapCoordinates);
        // yield return new WaitForSeconds(0.25F);
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
        if (startTrial) {
            // Debug.Log(curPointGoingTo);
            // Debug.Log(finalList);
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
            } else 
            {
                numFramesBeforeNextTrial--;
                if (numFramesBeforeNextTrial == 0) 
                {
                    if (curTrialNum < numTrials) 
                    {
                        curTrialNum++;
                        MainRunner();
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
