using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;

public class CubeAgent : Agent
{

    public GameObject ball;
    public bool useVecObs;
    private Rigidbody m_BallRb;
    private EnvironmentParameters m_ResetParams;
    public override void Initialize()
    {
        m_BallRb = ball.GetComponent<Rigidbody>();
        m_ResetParams = Academy.Instance.EnvironmentParameters;
        SetResetParam();
    }

    void SetResetParam()
    {
        SetBall();
    }

    void SetBall()
    {
        //random mass
        var mass = Random.Range(0.25f, 3);
        m_BallRb.mass = mass;

        //random scale
        float scale;
        if (Random.value < .6f)
        {
            scale = Random.Range(2f, 3f);
        }
        else
        {
            scale = Random.Range(.25f, 1.9f);
        }
        ball.transform.localScale = new Vector3(scale, scale, scale);
    }

    public override void OnEpisodeBegin()
    {
        gameObject.transform.rotation = new Quaternion(0, 0, 0, 0);
        gameObject.transform.Rotate(new Vector3(1, 0, 0), Random.Range(-10,10));
        gameObject.transform.Rotate(new Vector3(0, 0, 1), Random.Range(-10,10));
        m_BallRb.velocity = new Vector3(0f, 0f, 0f);
        ball.transform.position = new Vector3(Random.Range(-1.5f, 1.5f), 4f, Random.Range(-1.5f, 1.5f)) + gameObject.transform.position;
        SetResetParam();
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        if(!useVecObs) return;
        sensor.AddObservation(gameObject.transform.rotation.z);
        sensor.AddObservation(gameObject.transform.rotation.x);
        sensor.AddObservation(ball.transform.position - gameObject.transform.position);
        sensor.AddObservation(m_BallRb.velocity);
    }

    public override void OnActionReceived(ActionBuffers actions)
    {
        var actionZ = 2f * Mathf.Clamp(actions.ContinuousActions[0], -1f, 1f);
        var actionX = 2f * Mathf.Clamp(actions.ContinuousActions[1], -1f, 1f);

        if ((gameObject.transform.rotation.z < 0.25f && actionZ > 0f) ||
            (gameObject.transform.rotation.z > -.25f && actionZ < 0f))
        {
            gameObject.transform.Rotate(new Vector3(0,0,1), actionZ);
        }
        
        if ((gameObject.transform.rotation.x < 0.25f && actionX > 0f) ||
            (gameObject.transform.rotation.x > -.25f && actionX < 0f))
        {
            gameObject.transform.Rotate(new Vector3(1,0,0), actionX);
        }

        if ((ball.transform.position.y - gameObject.transform.position.y) < -3f ||
            Mathf.Abs(ball.transform.position.z - gameObject.transform.position.z) > 5f ||
            Mathf.Abs(ball.transform.position.x - gameObject.transform.position.x) > 5f)
        {
            SetReward(-1f);
            EndEpisode();
        }
        else
        {
            SetReward(.1f);
        }
    }

    public override void Heuristic(in ActionBuffers actionsOut)
    {
        var continuousActionsOut = actionsOut.ContinuousActions;
        continuousActionsOut[0] = -Input.GetAxis("Horizontal");
        continuousActionsOut[1] = Input.GetAxis("Vertical");
    }
    
}
