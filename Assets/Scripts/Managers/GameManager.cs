using System.Collections;
using UnityEngine.UI;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    private static GameManager _instance; // singleton
    public static GameManager Instance { get { Init(); return _instance; } }

    public static float pacmanSpeed = 4f;
    public static float ghostSpeed = 4f;
    public float powerUpDuration = 10f;
    public bool debugMode = false;

    private PacmanMovementArcade _pacman;
    public PacmanMovementArcade Pacman { get { return _pacman; } }

    private GhostBehavior[] _ghosts;

    [SerializeField] private Transform _doorExit;
    public Transform DoorExit { get { return _doorExit; } }

    [SerializeField] private Transform _spawn;
    public Transform Spawn { get { return _spawn; } }

    [SerializeField] private Material _frightenedMaterial;
    public Material FrightenedMaterial { get { return _frightenedMaterial; } }

    [SerializeField] private Text _livesText;
    [SerializeField] private Text _scoreText;
    [SerializeField] private Text _pelletsText;
    [SerializeField] private Text _timeText;

    private float[] _intervals = {7f, 20f, 7f, 20f, 5f, 20f, 5f, float.PositiveInfinity};

    private int _lives = 3;
    private int _score = 0;
    private int _pelletsLeft = 244;

    private void Awake() 
    {
        Init();

        _pacman = GameObject.FindGameObjectWithTag("Player").GetComponent<PacmanMovementArcade>();

        GameObject[] gos = GameObject.FindGameObjectsWithTag("Ghost");
        _ghosts = new GhostBehavior[gos.Length];

        for (int i = 0; i < gos.Length; i++)
        {
            _ghosts[i] = gos[i].GetComponent<GhostBehavior>();
        }
    }

    private void Start()
    {
        StartCoroutine(StateRoutine());
    }

    private void Update()
    {

    }

    private static void Init()
    {
        if (_instance == null)
        {
            GameObject go = GameObject.Find("@GameManager");
            if (go == null)
            {
                go = new GameObject {name = "@GameManager"};
                go.AddComponent<GameManager>();
            }
            DontDestroyOnLoad(go);
            _instance = go.GetComponent<GameManager>();
        }
    }

    public void OnPacmanDeath()
    {
        _pacman.enabled = false;

        // stop chasing
        foreach(GhostBehavior ghost in _ghosts)
        {
            ghost.enabled = false;
        }

        _lives--;
        _livesText.text = $"Lives: {_lives}";

        if (_lives > 0) Invoke(nameof(Restart), 2f);
        else if (_pelletsLeft == 0) OnLevelFinished(); // for when pacman dies and eats the last pellet at the same time
        else _livesText.text = "Game Over!";
    }

    public void OnPelletEaten()
    {
        _score += 10;
        _scoreText.text = $"Score: {_score}";
        _pelletsText.text = $"Pellets Remaining: {--_pelletsLeft}";

        if (_pelletsLeft == 0) OnLevelFinished();
    }

    public void OnPowerUp()
    {
        _score += 50;
        _scoreText.text = $"Score: {_score}";
        _pelletsText.text = $"Pellets Remaining: {--_pelletsLeft}";

        if (_pelletsLeft == 0) OnLevelFinished();

        StartCoroutine(PowerUpRoutine());
    }

    private IEnumerator PowerUpRoutine()
    {
        print("powerup");
        _pacman.isPoweredUp = true;

        foreach (GhostBehavior ghost in _ghosts)
        {
            ghost.EnterFrightened();
        }

        yield return new WaitForSeconds(powerUpDuration);

        foreach (GhostBehavior ghost in _ghosts)
        {
            ghost.ExitFrightened();
        }
        
        _pacman.isPoweredUp = false;
        print("powerup end");
    }

    private void OnLevelFinished()
    {
        _pacman.enabled = false;

        foreach (GhostBehavior ghost in _ghosts)
        {
            ghost.enabled = false;
        }

        print("You win!");
    }

    private void Restart()
    {
        _pacman.enabled = true;
        _pacman.Reset();

        StopCoroutine(StateRoutine());
        
        foreach(GhostBehavior ghost in _ghosts)
        {
            ghost.enabled = true;
            ghost.Reset();
        }

        StartCoroutine(StateRoutine());
    }

    private IEnumerator StateRoutine()
    {
        int intervalIndex = 0;
        float time = 0f;

        while (true)
        {
            if (!_pacman.isPoweredUp)
            {
                time += Time.deltaTime;
                _timeText.text = $"Time: {time}";

                if (time > _intervals[intervalIndex])
                {
                    time = 0f;
                    intervalIndex++;

                    foreach (GhostBehavior ghost in _ghosts)
                    {
                        ghost.SwitchState();
                    }

                    print("State switch!");
                }
            }

            yield return null;
        }
    }


}
