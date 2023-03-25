using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameAssets : MonoBehaviour
{
    public static GameAssets instance;

    public static GameAssets getInstance()
    {
        return instance;
    }

    private void Awake()
    {
        instance = this;
    }

    public Sprite pipeTopSprite;
    public Transform prefabPipeBody;
    public Transform prefabPipeHead;
    public Transform prefabGround;
    public Transform pipeCheckpoint;
}
