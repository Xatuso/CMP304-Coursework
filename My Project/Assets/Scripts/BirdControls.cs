using System;
using System.Collections;
using System.Collections.Generic;
using Unity.MLAgents.Actuators;
using UnityEngine;

public class BirdControls : MonoBehaviour
{
    private enum STATE
    {
        WAITINGTOSTART,
        PLAYING,
        DEAD,
    }
    public event EventHandler onDied;
    public event EventHandler onStartedPlaying;

    private Rigidbody2D rigidbody;
    private const float JUMP_FORCE = 100f;
    private STATE state;
    public static BirdControls instance;

    public static BirdControls GetInstance()
    {
        return instance;
    }

    private void Awake()
    {
        instance = this;
        rigidbody = GetComponent<Rigidbody2D>();
        rigidbody.bodyType = RigidbodyType2D.Static;
    }

    private void Update()
    {
        switch (state)
        {
            case STATE.WAITINGTOSTART:
                if (Input.GetKeyDown(KeyCode.Space) || Input.GetMouseButtonDown(0))
                //if(TextInput() || autoStart)
                {
                    Jump();
                    state = STATE.PLAYING;
                    rigidbody.bodyType = RigidbodyType2D.Dynamic;
                    if(onStartedPlaying != null) onStartedPlaying(this, EventArgs.Empty);
                }
                break;
            case STATE.PLAYING:
                if (Input.GetKeyDown(KeyCode.Space) || Input.GetMouseButtonDown(0))
                //if(TextInput())
                {
                    Jump();
                }
                break;
            case STATE.DEAD:
                break;
        }
        Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.forward) * 100, Color.yellow);
    }

    public void Jump()
    {
        rigidbody.velocity = Vector2.up * JUMP_FORCE;
    }

    private bool TextInput()
    {
        return false;
    }

    //private void OnTriggerEnter2D(Collider2D collider)
    //{
    //    rigidbody.bodyType = RigidbodyType2D.Static;
    //    state = STATE.DEAD;
    //    if(onDied != null) onDied(this,EventArgs.Empty);
    //}

    public void Reset()
    {
        rigidbody.velocity = Vector2.zero;
        rigidbody.bodyType = RigidbodyType2D.Static;
        transform.position = Vector3.zero;
    }

    public float GetVelocityY()
    {
        return rigidbody.velocity.y;
    }
}
