using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;
using Unity.VisualScripting;

public class BirdAgentScript : Agent
{
    [SerializeField] private Level level;
    [SerializeField] Rigidbody2D rigidbody;
    private float jumpForce = 100f;

    private Vector2 startingPos;

    public override void Initialize()
    {
        startingPos = transform.position;
    }
    public override void OnEpisodeBegin()
    {
        transform.position = startingPos;
        rigidbody.velocity = Vector2.zero;
        level.reset();
    }

    private void Start()
    {
        level.OnPipePassed += Level_OnPipePassed;
    }

    //it gets a small reward for staying alive, incentivizing the ai to perform actions which keeps it alive
    public override void OnActionReceived(ActionBuffers actions)
    {
        AddReward(0.1f);
        if (actions.DiscreteActions[0] != 1)
        {
            return;
        }
        else
        {
            Jump();
        }
    }
    public void Jump()
    {
        rigidbody.velocity = Vector2.up * jumpForce;
    }

    //gets a reward when it passes a pipe
    private void Level_OnPipePassed(object sender, System.EventArgs e)
    {
        AddReward((1f));
    }

    //if it collides with something (ceiling/pipe/floor), it gets a negative reward and the episode ends
    private void OnTriggerEnter2D(Collider2D other)
    {
        AddReward(-1f);
        EndEpisode();
    }
}
