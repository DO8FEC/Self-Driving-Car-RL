using System.Collections;
using System.Collections.Generic;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;
using UnityEngine;

public class MyAgent : Agent
{
    // The speed of the agent car
    public float speed = 20.0f;

    // This is a reference to the target (the parking space)
    public Transform TargetTransform;

    // It's called at the start of a new episode.
    public override void OnEpisodeBegin()
    {
        // Reset the position of the agent car
        transform.localPosition = new Vector3(18.5f, 1, -36);

        // Reset the position of the target parking space
        TargetTransform.localPosition = new Vector3(3, 2, 3);
    }

    // Executes the actions chosen by the agent.
    public override void OnActionReceived(ActionBuffers actions)
    {
        var actionTaken = actions.ContinuousActions;

        float actionSpeed = actionTaken[0];
        float actionSteering = actionTaken[1];

        transform.Translate(Vector3.forward * speed * actionSpeed * Time.fixedDeltaTime);
        transform.rotation = Quaternion.Euler(new Vector3(0, actionSteering * 180, 0));

        // Penalize for inefficient time usage
        AddReward(-0.01f);
    }

    // Useful for testing and debugging the agent's behavior.
    public override void Heuristic(in ActionBuffers actionsOut)
    {
        var actions = actionsOut.ContinuousActions;

        var horizontal = Input.GetAxisRaw("Horizontal");
        var vertical = Input.GetAxisRaw("Vertical");

        actions[0] = 0;
        actions[1] = 0;

        if (horizontal == -1) {
            actions[1] = -0.5f;
        } else if (horizontal == +1) {
            actions[1] = 0.5f;
        }
        if (vertical == -1) {
            actions[0] = -1;
        } else if (vertical == +1) {
            actions[0] = +1;
        }
    }

    // It can detect the collision and gives rewards accordingly
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.tag == "Wall")
        {
            // Penalize the agent since it hits the wall
            AddReward(-1);
            EndEpisode();
        }
        else if (collision.collider.tag == "ParkingTarget")
        {
            // Add the reward since the agent could find the target parking space
            AddReward(+1);
            EndEpisode();
        }
        else if (collision.collider.tag == "ParkedCar")
        {
            // Penalize the agent since it hits the other parked car
            AddReward(-0.1f);
        }
    }
}
