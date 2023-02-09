using System.Collections;
using UnityEngine.UI;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    private static GameManager _instance; // singleton
    public static GameManager Instance { get { Init(); return _instance; } }

    public static float pacmanSpeed = 4f;
    public static float ghostSpeed = 4f;

    [SerializeField] private GameObject _pacman;
    public GameObject Pacman { get { return _pacman; } }

    [SerializeField] private GameObject[] _ghosts;

    [SerializeField] private Transform _doorExit;
    public Transform DoorExit { get { return _doorExit; } }

    [SerializeField] private Text _livesText;
    [SerializeField] private Text _scoreText;
    [SerializeField] private Text _pelletsText;

    private float[] _intervals = {7f, 20f, 7f, 20f, 5f, 20f, 5f, float.PositiveInfinity};
    private int _intervalIndex = 0;

    private int _lives = 3;
    private int _score = 0;
    private int _pelletsLeft = 240;

    private void Awake() { Init(); }

    private void Start()
    {
        StartCoroutine(SwitchStates());
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
        // stop and hide pacman
        _pacman.SetActive(false);

        // stop chasing
        foreach(GameObject ghost in _ghosts)
        {
            ghost.GetComponent<GhostBehavior>().enabled = false;
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

    private void OnLevelFinished()
    {
        _pacman.GetComponent<PacmanMovementArcade>().enabled = false;

        foreach (GameObject ghost in _ghosts)
        {
            ghost.GetComponent<GhostBehavior>().enabled = false;
        }

        print("You win!");
    }

    private void Restart()
    {
        _pacman.SetActive(true);
        _pacman.GetComponent<PacmanMovementArcade>().Reset();

        StopCoroutine(SwitchStates());
        
        foreach(GameObject ghost in _ghosts)
        {
            ghost.GetComponent<GhostBehavior>().enabled = true;
            ghost.GetComponent<GhostBehavior>().Reset();
        }

        StartCoroutine(SwitchStates());
    }

    private IEnumerator SwitchStates()
    {
        _intervalIndex = 0;

        while (_intervalIndex < _intervals.Length)
        {
            if (_intervalIndex % 2 == 0)
            {
                print($" {_intervalIndex} Scatter");
            }
            else
            {
                print($" {_intervalIndex} Chase");
            }

            foreach (GameObject ghost in _ghosts)
            {
                ghost.GetComponent<GhostBehavior>().SwitchState();
            }

            yield return new WaitForSeconds(_intervals[_intervalIndex]);

            _intervalIndex++;
        }

    }


}
