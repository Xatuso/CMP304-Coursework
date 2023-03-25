using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Http.Headers;
using UnityEngine;


public class Level : MonoBehaviour
{
    private const float CAMERA_ORTHO_SIZE = 50f;
    private const float PIPE_WIDTH = 7.8f;
    private const float PIPE_HEAD_HEIGHT = 3.75f;
    private const float PIPE_MOVE_SPEED = 30f;
    private const float PIPE_DESTROY_POS = -100f;
    private const float PIPE_SPAWN_POS = 100f;
    private const float BIRD_X_POS = 0f;
    private const float GROUND_DESTORY_POS = -200f;

    private static Level instance;
    [SerializeField] private BirdControls bird;

    public static Level GetInstace()
    {
        return instance;
    }

    public event EventHandler OnPipePassed;

    private List<Transform> groundList;
    private List<Pipe> pipeList;
    private int spawnCount;
    private int pipesPassedCount;
    private float spawnTimer;
    private float maxSpawnTimer;
    private float gapSize;
    private STATE state;

    public enum DIFFICULTY
    {
        EASY,
        MEDIUM,
        HARD,
        INSANE,
    }

    private enum STATE
    {
        WAITINGTOSTART,
        PLAYING,
        BIRDDEAD,
    }

    private void Awake()
    {
        instance = this;
        pipeList = new List<Pipe>();
        SpawnGround();
        maxSpawnTimer = 1f;
        spawnTimer = 2f;
        SetDifficulty(DIFFICULTY.EASY);
        state = STATE.PLAYING;
    }

    private void Update()
    {
        if (state == STATE.PLAYING)
        {
            PipeMovement();
            GroundMovement();
            SpawnPipe();
        }
    }

    private void SpawnGround()
    {
        groundList = new List<Transform>();
        Transform groundTransform;
        float groundY = -47.5f;
        float groundWidth = 192f;
        groundTransform = Instantiate(GameAssets.getInstance().prefabGround, new Vector3(0, groundY, 0), Quaternion.identity);
        groundList.Add((groundTransform));
        groundTransform = Instantiate(GameAssets.getInstance().prefabGround, new Vector3(groundWidth, groundY, 0), Quaternion.identity);
        groundList.Add((groundTransform));
        groundTransform = Instantiate(GameAssets.getInstance().prefabGround, new Vector3(groundWidth * 2, groundY, 0), Quaternion.identity);
        groundList.Add((groundTransform));
    }

    private void GroundMovement()
    {
        foreach (Transform groundTransform in groundList)
        {
            groundTransform.position += new Vector3(-1, 0, 0) * PIPE_MOVE_SPEED * Time.deltaTime;
            if (groundTransform.position.x < GROUND_DESTORY_POS)
                //ground passed the left side, move it to the right again
            {
                float rightMostXPos = -100f;
                for (int i = 0; i < groundList.Count; ++i)
                {
                    if (groundList[i].position.x > rightMostXPos)
                    {
                        rightMostXPos = groundList[i].position.x;
                    }
                }

                float groundWidth = 192;
                groundTransform.position = new Vector3(rightMostXPos + groundWidth, groundTransform.position.y, groundTransform.position.z);
            }
        }
    }

    private void SpawnPipe()
    {
        spawnTimer -= Time.deltaTime;
        if (spawnTimer < 0)
        {
            spawnTimer += maxSpawnTimer;

            float heightLimit = 10f;
            float minHeight = gapSize * .5f + heightLimit;
            float totalHeight = CAMERA_ORTHO_SIZE * 2f;
            float maxHeight = totalHeight - gapSize * .5f - heightLimit;

            float height = UnityEngine.Random.Range(minHeight, maxHeight);

            CreateGapPipes(height, gapSize, PIPE_SPAWN_POS);
        }
    }

    private void PipeMovement()
    {
        for (int i = 0; i < pipeList.Count; ++i)
        {
            Pipe pipe = pipeList[i];
            bool isToRight = pipe.GetXPos() > BIRD_X_POS;
            pipe.Move();
            if(isToRight && pipe.GetXPos() <= BIRD_X_POS && pipe.IsBottom())
            {
                //pipe passed the bird
                ++pipesPassedCount;
                OnPipePassed?.Invoke(this, EventArgs.Empty);
            }
            if (pipe.GetXPos() < PIPE_DESTROY_POS)
            {
                pipe.DestroyPipe();
                pipeList.Remove(pipe);
                --i;
            }
        }
    }

    private void SetDifficulty(DIFFICULTY difficulty)
    {
        switch (difficulty)
        {
            case DIFFICULTY.EASY:
                gapSize = 50.0f;
                maxSpawnTimer = 1.2f;
                break;
            case DIFFICULTY.MEDIUM:
                gapSize = 40.0f;
                maxSpawnTimer = 1.1f;
                break;
            case DIFFICULTY.HARD:
                gapSize = 33.0f;
                maxSpawnTimer = 1.0f;
                break;
            case DIFFICULTY.INSANE:
                gapSize = 24.0f;
                maxSpawnTimer = 0.8f;
                break;
        }
    }

    private DIFFICULTY GetDifficulty()
    {
        if (spawnCount >= 30)
        {
            return DIFFICULTY.INSANE;
        }
        if (spawnCount >= 20)
        {
            return DIFFICULTY.HARD;
        }
        if (spawnCount >= 10)
        {
            return DIFFICULTY.MEDIUM;
        }

        return DIFFICULTY.EASY;
    }

    private void CreateGapPipes(float gapY, float gapSize, float xPos)
    {
        Pipe pipeBottom = CreatePipe(gapY - gapSize *.5f, xPos, true);
        Pipe pipeTop = CreatePipe(CAMERA_ORTHO_SIZE*2f - gapY - gapSize*.5f, xPos, false);
        ++spawnCount;
        SetDifficulty(GetDifficulty());
    }

    private Pipe CreatePipe(float height, float xPos, bool isBottom)
    {
        //pipe head
        Transform pipeHead = Instantiate(GameAssets.getInstance().prefabPipeHead);
        float pipeHeadYPos;
        if (isBottom)
        {
            pipeHeadYPos = -CAMERA_ORTHO_SIZE + height - PIPE_HEAD_HEIGHT * .5f;
        }
        else
        {
            {
                pipeHeadYPos = CAMERA_ORTHO_SIZE - height + PIPE_HEAD_HEIGHT * .5f;
            }
        }
        pipeHead.position = new Vector3(xPos, pipeHeadYPos);

        //pipe body
        Transform pipeBody = Instantiate(GameAssets.getInstance().prefabPipeBody);
        float pipeBodyYPos;
        if (isBottom)
        {
            pipeBodyYPos = -CAMERA_ORTHO_SIZE;
        }
        else
        {
            pipeBodyYPos = CAMERA_ORTHO_SIZE;
            pipeBody.localScale = new Vector3(1, -1, 1);
        }
        pipeBody.position = new Vector3(xPos, pipeBodyYPos);
        
        SpriteRenderer pipeBodySpriteRenderer = pipeBody.GetComponent<SpriteRenderer>();
        pipeBodySpriteRenderer.size = new Vector2(PIPE_WIDTH, height);

        BoxCollider2D pipeBodyBoxCollider = pipeBody.GetComponent<BoxCollider2D>();
        pipeBodyBoxCollider.size = new Vector2(PIPE_WIDTH, height);
        pipeBodyBoxCollider.offset = new Vector2(0f, height * 0.5f);

        Pipe newPipe = new Pipe(pipeHead, pipeBody, isBottom);
        pipeList.Add(newPipe);
        if (isBottom)
        {
            Transform pipeCheckpoint = Instantiate(GameAssets.getInstance().pipeCheckpoint);
            pipeCheckpoint.localScale = new Vector3(.1f, gapSize);
            pipeCheckpoint.parent = pipeBody;
            pipeCheckpoint.localPosition = new Vector3(0, height + gapSize * .5f);
        }

        return newPipe;
    }

    public int GetPipeSpawned()
    {
        return spawnCount;
    }

    public int GetPipesPassed()
    {
        return pipesPassedCount;
    }

    public void reset()
    {
        foreach (Pipe pipe in pipeList)
        {
            pipe.DestroyPipe();
        }
        pipeList.Clear();
        pipesPassedCount = 0;
        spawnCount = 0;
        SetDifficulty(DIFFICULTY.EASY);
        maxSpawnTimer = 0f;
        //bird.Reset();
    }


    //private class for controlling the pipe head and body as one entity
    public class Pipe
    {
        private Transform pipeHeadTransform;
        public Transform pipeBodyTransform;
        private bool isBottom;

        public Pipe(Transform pipeHeadTransform, Transform pipeBodyTransform, bool isBottom)
        {
            this.pipeHeadTransform = pipeHeadTransform;
            this.pipeBodyTransform = pipeBodyTransform;
            this.isBottom = isBottom;
        }

        public void Move()
        {
            pipeHeadTransform.position += new Vector3(-1, 0, 0) * PIPE_MOVE_SPEED * Time.deltaTime;
            pipeBodyTransform.position += new Vector3(-1, 0, 0) * PIPE_MOVE_SPEED * Time.deltaTime;
        }

        public float GetXPos()
        {
            return pipeHeadTransform.position.x;
        }

        public bool IsBottom()
        {
            return isBottom;
        }

        public void DestroyPipe()
        {
            Destroy(pipeHeadTransform.gameObject);
            Destroy(pipeBodyTransform.gameObject);
        }
    }
}
