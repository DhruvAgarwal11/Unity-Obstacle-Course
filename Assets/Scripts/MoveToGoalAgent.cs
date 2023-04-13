using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;
using System;

public class MoveToGoalAgent : Agent
{
    [SerializeField] private Transform targetTransform;
    float moveSpeed = 2f;
    Vector3 lastMovement;
    double lastVal;

    public override void OnEpisodeBegin()
    {
        float x = 24.0F;
        float y = 0.25F;
        float z = 6.0f;
        transform.position = new Vector3(x, y, z);
        lastVal = Math.Pow(transform.position.x + 22, 2) + Math.Pow(transform.position.z -2, 2);
        Debug.Log(transform.position);
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        // Debug.Log(transform.position);
        // Debug.Log(targetTransform.position);
        sensor.AddObservation(transform.position);
        sensor.AddObservation(targetTransform.position);
    }

    public override void OnActionReceived(ActionBuffers actions)
    {
        float moveX = actions.ContinuousActions[0];
        float moveZ = actions.ContinuousActions[1];
        lastMovement = new Vector3(moveX, 0, moveZ);
        transform.position += lastMovement * Time.deltaTime * moveSpeed;
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

    // public override void Heuristic(in ActionBuffers actionsOut) {
    //     ActionSegment<float> continuousActions = actionsOut.ContinuousActions;
    //     Debug.Log(continuousActions);
    //     continuousActions[0] = Input.GetAxisRaw("Horizontal");
    //     continuousActions[1] = Input.GetAxisRaw("Vertical");
    // }

    private void OnTriggerEnter(Collider other) {
        Debug.Log("here");
        if (other.TryGetComponent<Goal>(out Goal goal)) {
            AddReward(100000f);
            EndEpisode();
        }
        // else {
        //     // Debug.Log(-1);
        //     AddReward(-1f);
            
        //     transform.position -= lastMovement;
        //     // EndEpisode();
        // }
        // EndEpisode();
    }

    public void OnTriggerStay (Collider other) {
        //move the object back
        AddReward(-100000f);
        // AddReward(-3f);
        EndEpisode();
        transform.position -= lastMovement * Time.deltaTime * moveSpeed;
    }
}
