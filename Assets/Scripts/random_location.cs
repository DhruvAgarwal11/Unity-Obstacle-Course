using UnityEngine;
using UnityEngine.UI;
using static System.Math;
using System.Collections;
using System.Collections.Generic;
// using PriorityQueue;

using static Score;

public class random_location : MonoBehaviour {
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
    void Start() {
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

    void StartNewTrial() {
        gameOver = false;
        numFramesBeforeNextTrial = 200;
        Score.scoreStart(10);

        Score.displayGameOver(GameOverText, "");

        //Reset cube location
        x = 24.0F;
        y = 0.25F;
        z = Random.Range(-4, 4);
        pos = new Vector3(x, y, z);
        transform.position = pos;

        //Time between previous frame and current frame
        timePrev = 0;

        //Destroy cylinders 
        if (curTrialNum != 1) {
            foreach (GameObject curCylinder in cylinders) {
                if (curCylinder != null) {
                    Destroy(curCylinder);
                }  
            }
        }
        
        //Generate random cylinders across the board
        cylinders = new GameObject[Random.Range(minNumCylinders, maxNumCylinders)];
        for (int i = 0; i < cylinders.Length; i++) {
            RandomCylinderGenerator(i);
        }
        Dictionary<Vector3, SortedDictionary<float, Vector3>> pointsToOtherPoints = cylindersNothingBetween();
        finalList = djikstra(pointsToOtherPoints);
        foreach(var a in finalList) {
            Debug.Log(a);
        }
    }

    ArrayList djikstra(Dictionary<Vector3, SortedDictionary<float, Vector3>> pToP) {
        Vector3 curPoint = transform.position;
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
        while (curPoint != transform.position) {
            ret.Insert(0, curPoint);
            curPoint = mappingsBack[curPoint];
        }
        ret.Insert(0, curPoint);
        return ret;
    }

    void OnTriggerEnter(Collider other) {
        if (other.gameObject.name == "Player")
        {
            Destroy(other.gameObject);
        }
    }

    void OnTriggerStay(Collider other) {
        //move the object back
        transform.Translate(-movement * Time.deltaTime * speed);
        numFramesBeforeScoreDecrease--;
        if (numFramesBeforeScoreDecrease == 0) {
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
        if (!gameOver) {
            if (checkGameEnd(transform.position.x, transform.position.z)) {
                Score.displayGameOver(GameOverText, "You reached the target! Congrats!");
                gameOver = true;
                return;
            }
            else if (Score.getScore() == 0) {
                gameOver = true;
                return;
            }
            MoveObject(x, z, timePrev);
        }
        else {
            numFramesBeforeNextTrial--;
            if (numFramesBeforeNextTrial == 0) {
                if (curTrialNum < numTrials) {
                    curTrialNum++;
                    StartNewTrial();
                }
                else {
                    Score.displayGameOver(GameOverText, "Congrats, you finished all trials!");
                    Debug.Log("Congrats, you finished all trials!");
                    Application.Quit();
                    Debug.Break(); //remove in production
                }
            }
        }
    }

    void drawLines(ArrayList finalList) {
         // for (int i = 0; i < nothingBetweenPairs.Count; i++) {
        //     int j = i + 1;
        //     GameObject[] x = (GameObject[]) nothingBetweenPairs[i];
        //     while (j < nothingBetweenPairs.Count) {
        //         GameObject[] y = (GameObject[]) nothingBetweenPairs[j];
        //         if (!GameObject.ReferenceEquals(x[0], y[0])) {
        //             break;
        //         }
        //         j++;
        //     }
        //     j--;
        //     LineRenderer lr = x[0].GetComponent<LineRenderer>();
        //     lr.positionCount = j - i + 1;
        //     Vector3[] positions = new Vector3[lr.positionCount];
        //     for (int z = i; z <= j; z++) {
        //         GameObject[] y = (GameObject[]) nothingBetweenPairs[z];
        //         positions[z - i] = y[1].transform.position;
        //     }
        //     Debug.Log("setting positions for " + i + " for position count of " + lr.positionCount);
        //     lr.SetPositions(positions);
        //     i = j;
        // }

        // return nothingBetweenPairs;
    }


    void MoveObject(float x, float z, float time = 1) 
    {
        movement = new Vector3(x, 0, z);
        movement = Vector3.ClampMagnitude(movement, 1);
        transform.Translate(movement * speed * time);
    }

    Vector3 AverageVectors(Vector3 v1, Vector3 v2) {
        float x = (v1.x + v2.x) / 2f;
        float y = (v1.y + v2.y) / 2f;
        float z = (v1.z + v2.z) / 2f;
        return new Vector3(x, y, z);
    }

    Dictionary<Vector3, SortedDictionary<float, Vector3>> cylindersNothingBetween() {
        var nothingBetweenPairs = new ArrayList();
        for (int i = 0; i < cylinders.Length; i++) {
            for (int j = i + 1; j < cylinders.Length; j++) {
                bool addThis = cylinderNotInWay(cylinders[i].transform.position, cylinders[j].transform.position, new GameObject[]{cylinders[i], cylinders[j]});
                if (addThis) {
                    nothingBetweenPairs.Add(new GameObject[2]{cylinders[i], cylinders[j]});
                    // Debug.Log(cylinders[i].transform.position.x + " " + cylinders[i].transform.position.z + " " + cylinders[j].transform.position.x + " " + cylinders[j].transform.position.z);
                }
            }
        }

        Dictionary<Vector3, SortedDictionary<float, Vector3>> allGraphNodes = new Dictionary<Vector3, SortedDictionary<float, Vector3>>();
        Debug.Log(nothingBetweenPairs.Count);
        for (int i = 0; i < nothingBetweenPairs.Count; i++) {
            for (int j = i + 1; j < nothingBetweenPairs.Count; j++) {
                GameObject[] first = (GameObject[]) nothingBetweenPairs[i];
                GameObject[] second = (GameObject[]) nothingBetweenPairs[j];
                Vector3 p1 = AverageVectors(first[0].transform.position, first[1].transform.position);
                Vector3 p2 = AverageVectors(second[0].transform.position, second[1].transform.position);
                addToGraphNodes(allGraphNodes, p1, p2);
                addToGraphNodes(allGraphNodes, p2, p1);
            }
        }

        //for the startingPoint
        Vector3 startPoint = transform.position;
        for (int i = 0; i < nothingBetweenPairs.Count; i++) {
            GameObject[] first = (GameObject[]) nothingBetweenPairs[i];
            Vector3 p1 = AverageVectors(first[0].transform.position, first[1].transform.position);
            addToGraphNodes(allGraphNodes, startPoint, p1);
        }


        //for the endingPoint
        for (int i = 0; i < nothingBetweenPairs.Count; i++) {
            GameObject[] first = (GameObject[]) nothingBetweenPairs[i];
            Vector3 p1 = AverageVectors(first[0].transform.position, first[1].transform.position);
            addToGraphNodes(allGraphNodes, p1, finalSpot);
        }
        
        //for the starting to end points
        addToGraphNodes(allGraphNodes, startPoint, finalSpot);

        return allGraphNodes;
        //CODE TO DRAW LINES BETWEEN THE CYLINDER PAIRS
        // for (int i = 0; i < nothingBetweenPairs.Count; i++) {
        //     int j = i + 1;
        //     GameObject[] x = (GameObject[]) nothingBetweenPairs[i];
        //     while (j < nothingBetweenPairs.Count) {
        //         GameObject[] y = (GameObject[]) nothingBetweenPairs[j];
        //         if (!GameObject.ReferenceEquals(x[0], y[0])) {
        //             break;
        //         }
        //         j++;
        //     }
        //     j--;
        //     LineRenderer lr = x[0].GetComponent<LineRenderer>();
        //     lr.positionCount = j - i + 1;
        //     Vector3[] positions = new Vector3[lr.positionCount];
        //     for (int z = i; z <= j; z++) {
        //         GameObject[] y = (GameObject[]) nothingBetweenPairs[z];
        //         positions[z - i] = y[1].transform.position;
        //     }
        //     Debug.Log("setting positions for " + i + " for position count of " + lr.positionCount);
        //     lr.SetPositions(positions);
        //     i = j;
        // }

        // return nothingBetweenPairs;
    }

    void addToGraphNodes(Dictionary<Vector3, SortedDictionary<float, Vector3>> allGraphNodes, Vector3 p1, Vector3 p2) {
        if (cylinderNotInWay(p1, p2, new GameObject[0])) {
            if (!allGraphNodes.ContainsKey(p1)) {
                allGraphNodes.Add(p1, new SortedDictionary<float, Vector3>());
            }
            allGraphNodes[p1].Add(Vector3.Distance(p1, p2), p2);
        }
    }

    bool cylinderNotInWay(Vector3 position1, Vector3 position2, GameObject[] cylindersNotIncluded) {
        float slope = (position2.z - position1.z)/(position2.x - position1.x);
        float perpSlope = -1/slope;
        float intercept = position1.z - slope * position1.x;

        float maxX = Max(position1.x, position2.x);
        float minX = Min(position1.x, position2.x);

        for (int k = 0; k < cylinders.Length; k++) {
            bool continueIt = false;
            for (int i = 0; i < cylindersNotIncluded.Length; i++) {
                if (GameObject.ReferenceEquals(cylinders[k], cylindersNotIncluded[i])) continueIt = true;
            }
            if (continueIt) continue;

            float curX = cylinders[k].transform.position.x;
            float curY = cylinders[k].transform.position.z;

            float perpIntercept = curY - perpSlope * curX;

            float interceptX = (perpIntercept - intercept)/(slope - perpSlope);
            float interceptY = slope * interceptX + intercept;
            if (interceptX >= minX && interceptX <= maxX && Pow(interceptX - curX, 2) + Pow(interceptY - curY, 2) < Pow(2.5, 2)) {
                return false;
            }
        }
        return true;
    }

    void RandomCylinderGenerator(int idx) {
        float x;
        float y;
        float z;
        bool overlapping;
        do {
            x = UnityEngine.Random.Range(-23.0F, 23.0F);
            y = 1.0f;
            z = UnityEngine.Random.Range(-23.0f, 23.0f);
            overlapping = false;
            for (int i = 0; i < idx; i++) {
                if (cylinders[i] != null) {
                    float distance = Vector3.Distance(transform.position - transform.localPosition + new Vector3(x, y, z), cylinders[i].transform.position);
                    if (distance <= 4.2) {
                        overlapping = true;
                        break;
                    }
                }  
            }
        } while ((x <= -18.0 && z >= -2.0 && z <= 5.0) || (x > 18.0F && z >= -6.0F && z <= 6.0) || overlapping);
        cylinders[idx] = Instantiate(cylinder, transform.position - transform.localPosition + new Vector3(x, y, z), Quaternion.identity);
        
    }

    bool checkGameEnd(float x, float z) {
        return x <= -20 && z >= -0.4 && z <= 4.1;
    }
    
}
