using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class GameController : MonoBehaviour
{
    private enum Direction
    { None, Left, Right, Up, Down }

    private Direction swipeDirection = Direction.None;
    private Direction swipeEndDirection = Direction.None;

    private Board board;
    private Spawner spawner;
    private Shape activeShape;
    private float dropInterval = 1f;
    private float dropIntervalModded;
    private float timeToDrop;

    private bool gameOver = false;
    public GameObject gameOverPanel;
    //float timeToNextKey;
    //[Range(0.02f, 1f)]
    //public float keyRepeatRate = .25F;

    private float timeToNextKeyLeftRight;

    [Range(0.02f, 1f)]
    public float keyRepeatRateLeftRight = .25f;

    private float timeToNextKeyDown;

    [Range(0.02f, 1f)]
    public float keyRepeatRateDown = .25f;

    private float timeToNextKeyRotate;

    [Range(0.02f, 1f)]
    public float keyRepeatRateRotate = .25f;

    private SoundManager soundManager;
    private ScoreManager scoreManager;

    public IconToggle rotIconToggle;
    private bool clockwise = true;

    private Ghost ghost;

    private Holder holder;

    public bool isPaused = false;
    public GameObject pausePanel;

    public ParticlePlayer gameOverFX;

    private void OnEnable()
    {
        TouchController.SwipeEvent += SwipeHandler;
        TouchController.SwipeEndEvent += SwipeEndHandler;
    }

    private void OnDisable()
    {
        TouchController.SwipeEvent -= SwipeHandler;
        TouchController.SwipeEndEvent -= SwipeEndHandler;
    }

    // Use this for initialization
    private void Start()
    {
        board = GameObject.FindWithTag("Board").GetComponent<Board>();
        spawner = GameObject.FindWithTag("Spawner").GetComponent<Spawner>();
        soundManager = GameObject.FindObjectOfType<SoundManager>();
        scoreManager = GameObject.FindObjectOfType<ScoreManager>();
        ghost = GameObject.FindObjectOfType<Ghost>();
        holder = GameObject.FindObjectOfType<Holder>();
        if (spawner)
        {
            if (activeShape == null)
            {
                activeShape = spawner.SpawnShape();
            }
            spawner.transform.position = VectorF.Round(spawner.transform.position);
        }
        else if (!spawner)
        {
            Debug.LogWarning("Spawner object is not found.");
        }

        if (!soundManager)
            Debug.LogWarning("WARNING: There is no sound Manager.");

        if (!board)
            Debug.LogWarning("Board object is not found.");

        if (!scoreManager)
            Debug.LogWarning("No scoreManager found.");

        timeToNextKeyLeftRight = Time.time + keyRepeatRateLeftRight;
        timeToNextKeyDown = Time.time + keyRepeatRateDown;
        timeToNextKeyRotate = Time.time + keyRepeatRateRotate;

        if (gameOverPanel)
        {
            gameOverPanel.SetActive(false);
        }

        if (pausePanel)
        {
            pausePanel.SetActive(false);
        }

        dropIntervalModded = dropInterval;
    }

    private void Update()
    {
        if (!spawner || !board || !activeShape || gameOver || !soundManager || !scoreManager)
        {
            Debug.Log("Either spawner, board , SoundManger or shape is empty.");
            return;
        }

        PlayerInput();
    }

    private void LateUpdate()
    {
        if (ghost)
        {
            ghost.DrawGhost(activeShape, board);
        }
    }

    private void MoveLeft()
    {
        activeShape.MoveLeft();
        timeToNextKeyLeftRight = Time.time + keyRepeatRateLeftRight;
        if (!board.IsValidPosition(activeShape))
        {
            activeShape.MoveRight();
            PlaySound(soundManager.erroSound, 0.5f);
        }
        else
        {
            PlaySound(soundManager.moveSound, 0.5f);
        }
    }

    private void MoveRight()
    {
        activeShape.MoveRight();
        timeToNextKeyLeftRight = Time.time + keyRepeatRateLeftRight;
        if (!board.IsValidPosition(activeShape))
        {
            activeShape.MoveLeft();
            PlaySound(soundManager.erroSound, 0.5f);
        }
        else
        {
            PlaySound(soundManager.moveSound, 0.5f);
        }
    }

    private void Rotate()
    {
        activeShape.Rotate(clockwise);
        timeToNextKeyRotate = Time.time + keyRepeatRateRotate;
        if (!board.IsValidPosition(activeShape))
        {
            activeShape.Rotate(!clockwise);
            PlaySound(soundManager.erroSound, 0.5f);
        }
        else
        {
            PlaySound(soundManager.moveSound, 0.5f);
        }
    }

    private void MoveDown()
    {
        timeToDrop = Time.time + dropIntervalModded;
        timeToNextKeyDown = Time.time + keyRepeatRateDown;
        activeShape.MoveDown();
        if (!board.IsValidPosition(activeShape))
        {
            if (board.IsOverLimit(activeShape))
            {
                GameOver();
            }
            else
            {
                LandShape();
            }
        }
    }

    private void PlayerInput()
    {
        // Debug.Log("Working");

        if ((Input.GetButton("MoveLeft") && (Time.time > timeToNextKeyLeftRight)) || Input.GetButtonDown("MoveLeft"))
        {
            MoveLeft();
        }
        else if ((Input.GetButton("MoveRight") && (Time.time > timeToNextKeyLeftRight)) || Input.GetButtonDown("MoveRight"))
        {
            MoveRight();
        }
        else if ((Input.GetButtonDown("Rotate") && (Time.time > timeToNextKeyRotate)) || Input.GetButtonDown("Rotate"))
        {
            Rotate();
        }
        else if ((Input.GetButton("MoveDown") && (Time.time > timeToNextKeyDown)) || (Time.time > timeToDrop))
        {
            MoveDown();
        }
        else if ((swipeDirection == Direction.Right && Time.time > timeToNextKeyLeftRight) || swipeEndDirection == Direction.Right)
        {
            MoveRight();

            swipeDirection = Direction.None;
            swipeEndDirection = Direction.None;
        }
        else if ((swipeDirection == Direction.Left && Time.time > timeToNextKeyLeftRight) || swipeEndDirection == Direction.Left)
        {
            MoveLeft();

            swipeDirection = Direction.None;
            swipeEndDirection = Direction.None;
        }
        else if (swipeEndDirection == Direction.Up)
        {
            Rotate();
            swipeEndDirection = Direction.None;
        }
        else if (swipeDirection == Direction.Down && Time.time > timeToNextKeyDown)
        {
            MoveDown();
            swipeDirection = Direction.None;
        }
    }

    private void PlaySound(AudioClip audioClip, float volMul)
    {
        if (audioClip && soundManager.fxEnabled)
        {
            AudioSource.PlayClipAtPoint(audioClip, Camera.main.transform.position, Mathf.Clamp(soundManager.fxVolume * volMul, 0.05F, 1.0F));
        }
    }

    private void GameOver()
    {
        activeShape.MoveUp();

        StartCoroutine("GameOverRoutine");
        gameOver = true;

        PlaySound(soundManager.gameOverSound, 5.0f);
        PlaySound(soundManager.gameOverVocalClip, 5.0f);

        Debug.LogWarning("Game Over");
    }

    private IEnumerator GameOverRoutine()
    {
        if (gameOverFX)
            gameOverFX.Play();

        yield return new WaitForSeconds(0.25f);

        if (gameOverPanel)
            gameOverPanel.SetActive(true);
    }

    private void LandShape()
    {
        timeToNextKeyLeftRight = Time.time;
        timeToNextKeyDown = Time.time;
        timeToNextKeyRotate = Time.time;

        //timeToNextKey = Time.time;

        activeShape.MoveUp();
        board.StoreShapeInGrid(activeShape);

        activeShape.LandShapeFX();

        if (ghost)
        {
            ghost.Reset();
        }

        if (holder)
        {
            holder.canRelease = true;
        }

        activeShape = spawner.SpawnShape();

        //board.ClearAllRows();
        board.StartCoroutine("ClearAllRows");

        PlaySound(soundManager.dropSound, 0.5f);
        if (board.completedRows > 0)
        {
            scoreManager.ScoreLines(board.completedRows);

            if (scoreManager.didLevelUp)
            {
                PlaySound(soundManager.levelUpVocalClip, 1);

                dropIntervalModded = Mathf.Clamp(dropInterval - (((float)scoreManager.level - 1)) * 0.1f, 0.05f, 1f);
            }
            else
            {
                if (board.completedRows > 1)
                {
                    PlaySound(soundManager.GetRandomClip(soundManager.vocalClips), 1.0f);
                }
                PlaySound(soundManager.clearRowSound, 0.8f);
            }
        }
    }

    public void ToggleRotDirection()
    {
        clockwise = !clockwise;
        if (rotIconToggle)
        {
            rotIconToggle.ToggleIcon(clockwise);
        }
    }

    public void TogglePause()
    {
        if (gameOver)
            return;

        isPaused = !isPaused;

        if (pausePanel)
        {
            pausePanel.SetActive(isPaused);
            if (soundManager)
            {
                soundManager.musicSource.volume = isPaused ? soundManager.musicVolume * 0.25f : soundManager.musicVolume;
            }

            Time.timeScale = isPaused ? 0 : 1;
        }
    }

    public void RestartGmae()
    {
        Debug.Log("Restarted Game");
        Time.timeScale = 1;
        Application.LoadLevel(Application.loadedLevel);
    }

    public void Hold()
    {
        // if the holder is empty...
        if (!holder.heldShape)
        {
            // catch the current active Shape
            holder.Catch(activeShape);

            // spawn a new Shape
            activeShape = spawner.SpawnShape();

            // play a sound
            PlaySound(soundManager.holdSound, 1.0f);
        }
        // if the holder is not empty and can releases
        else if (holder.canRelease)
        {
            // set our active Shape to a temporary Shape
            Shape shape = activeShape;

            // release the currently heldShape
            activeShape = holder.Release();

            // move the released Shape back to the spawner position
            activeShape.transform.position = spawner.transform.position;

            // catch the temporary Shape
            holder.Catch(shape);

            // play a sound
            PlaySound(soundManager.holdSound, 1.0f);
        }
        // the holder is not empty but cannot release yet
        else
        {
            //Debug.LogWarning("HOLDER WARNING:  Wait for cool down");

            // play an error sound
            PlaySound(soundManager.erroSound, 1.0f);
        }

        // reset the Ghost every time we tap the Hold button
        if (ghost)
        {
            ghost.Reset();
        }
    }

    private void SwipeHandler(Vector2 swipeMovement)
    {
        swipeDirection = GetDirection(swipeMovement);
    }

    private void SwipeEndHandler(Vector2 swipeEnd)
    {
        swipeEndDirection = GetDirection(swipeEnd);
    }

    private Direction GetDirection(Vector2 swipeMovement)
    {
        Direction swipeDir = Direction.None;

        //horizontal
        if (Mathf.Abs(swipeMovement.x) > Mathf.Abs(swipeMovement.y))
        {
            swipeDir = (swipeMovement.x >= 0) ? Direction.Right : Direction.Left;
        }
        //vertical
        else
        {
            swipeDir = (swipeMovement.y >= 0) ? Direction.Up : Direction.Down;
        }

        return swipeDir;
    }
}