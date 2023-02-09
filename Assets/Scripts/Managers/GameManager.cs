using System.Collections;
using UnityEngine.UI;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    private static GameManager _instance; // singleton
    public static GameManager Instance { get { Init(); return _instance; } }

    [SerializeField] private GameObject _pacman;
    public GameObject Pacman { get { return _pacman; } }

    [SerializeField] private GameObject[] _ghosts;

    [SerializeField] private Vector3 _doorExit;
    public Vector3 DoorExit { get { return _doorExit; } }

    [SerializeField] private Text _livesText;
    [SerializeField] private Text _scoreText;
    [SerializeField] private Text _pelletsText;

    private int _lives = 3;
    private int _score = 0;
    private int _pelletsLeft = 240;

    private void Awake() { Init(); }

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
        // stop pacman, and hide him
        _pacman.SetActive(false);

        // stop chasing
        foreach(GameObject ghost in _ghosts)
        {
            ghost.GetComponent<GhostBehavior>().enabled = false;
        }

        _lives--;
        _livesText.text = $"Lives: {_lives}";

        if (_lives > 0) StartCoroutine(Restart());
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

    private IEnumerator Restart()
    {
        yield return new WaitForSeconds(2f);

        _pacman.SetActive(true);
        _pacman.GetComponent<PacmanMovementArcade>().Reset();

        foreach(GameObject ghost in _ghosts)
        {
            ghost.GetComponent<GhostBehavior>().enabled = true;
            ghost.GetComponent<GhostBehavior>().Reset();
        }
    }


}
