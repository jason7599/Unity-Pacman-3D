using UnityEngine;

public enum MovementMode
{
    ARCADE, DEBUG
}

public class PacmanMovementArcade : MonoBehaviour
{
    [SerializeField] private MovementMode _movementMode = MovementMode.ARCADE;

    private float _speed;

    private Vector3 _startingPosition;
    private Vector3 _direction = Vector3.left;
    private Vector3 _nextDirection = Vector3.zero;

    public Vector3 Direction { get { return _direction; } }
    public Vector3 Position { get { return transform.position; } }
    
    public bool isPoweredUp = false;

    private Rigidbody _rigidbody;
    private Camera _cam;

    private LayerMask _wallLayer;

    private void Start()
    {
        _speed = GameManager.pacmanSpeed;

        _startingPosition = transform.position;
        _direction = transform.forward;
        _nextDirection = Vector3.zero;

        _rigidbody = GetComponent<Rigidbody>();
        _cam = Camera.main;

        _wallLayer = LayerMask.GetMask("Wall");
    }

    private void OnCollisionEnter(Collision collision)
    {
        GameObject go = collision.gameObject;

        if (go.CompareTag("Ghost")) // ghost
        {
            if (isPoweredUp)
            {
                go.GetComponent<GhostBehavior>().Reset();
            }
            else
            {
                _cam.transform.LookAt(go.transform);
                GameManager.Instance.OnPacmanDeath();
            }
        }
    }

    private void OnTriggerEnter(Collider collider)
    {
        GameObject go = collider.gameObject;

        if (go.CompareTag("Pellet"))
        {
            Destroy(go);
            GameManager.Instance.OnPelletEaten();
        }
        else if (go.CompareTag("PowerPellet"))
        {
            Destroy(go);
            GameManager.Instance.OnPowerUp();
        }
    }

    private void Update()
    {
        if (_movementMode == MovementMode.ARCADE) MoveArcade();
        else MoveDebug();
    }

    private void MoveArcade()
    {
        if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow))
            _nextDirection = transform.forward;

        else if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow))
            _nextDirection = -transform.right;

        else if (Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow))
            _nextDirection = -transform.forward;

        else if (Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow))
            _nextDirection = transform.right;
        
        // if _nextDirection is set, and can move to that direction, change direction
        if (_nextDirection != Vector3.zero)
        {
            if (CanMoveTo(_nextDirection))
            {
                _direction = _nextDirection;
                _nextDirection = Vector3.zero;
                _rigidbody.MoveRotation(Quaternion.LookRotation(_direction));
            }
        }

        _rigidbody.MovePosition(_rigidbody.position + _direction * _speed * Time.deltaTime);

        _cam.transform.localRotation = Quaternion.Slerp(_cam.transform.localRotation, Quaternion.LookRotation(_direction), Time.deltaTime * 10f);
        _cam.transform.position = transform.position;
    }

    private void MoveDebug()
    {
        if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow))
            _nextDirection = Vector3.forward;

        else if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow))
            _nextDirection = Vector3.left;

        else if (Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow))
            _nextDirection = Vector3.back;

        else if (Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow))
            _nextDirection = Vector3.right;
        
        // if _nextDirection is set, and can move to that direction, change direction
        if (_nextDirection != Vector3.zero)
        {
            if (CanMoveTo(_nextDirection))
            {
                _direction = _nextDirection;
                _nextDirection = Vector3.zero;
                _rigidbody.MoveRotation(Quaternion.LookRotation(_direction));
            }
        }

        _rigidbody.MovePosition(_rigidbody.position + _direction * _speed * Time.deltaTime);

        _cam.transform.localRotation = Quaternion.Slerp(_cam.transform.localRotation, Quaternion.LookRotation(_direction), Time.deltaTime * 10f);
        _cam.transform.position = transform.position;
    }

    private bool CanMoveTo(Vector3 direction)
    {
        RaycastHit hit;
        Physics.BoxCast(transform.position, Vector3.one * 0.45f, direction, out hit, Quaternion.identity, 1f, _wallLayer);
        return hit.collider == null;
    }
    
    public void Reset()
    {
        transform.position = _startingPosition;
        transform.rotation = Quaternion.LookRotation(Vector3.left);

        _direction = Vector3.left;
        _nextDirection = Vector3.zero;
    }


}
