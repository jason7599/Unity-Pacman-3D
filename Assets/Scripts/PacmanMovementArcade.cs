using UnityEngine;

public class PacmanMovementArcade : MonoBehaviour
{
    [SerializeField] private float _speed = 4f;

    private Vector3 _startingPosition;
    private Vector3 _startingDirection;

    private Vector3 _direction = Vector3.left;
    private Vector3 _nextDirection = Vector3.zero;

    public Vector3 Direction { get { return _direction; } }
    public Vector3 Position { get { return transform.position; } }

    private Rigidbody _rigidbody;
    private Camera _cam;

    private LayerMask _wallLayer;

    private void Awake()
    {
        _startingPosition = transform.position;
        _startingDirection = transform.forward;

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
            _cam.transform.LookAt(go.transform);
            GameManager.Instance.OnPacmanDeath();
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
    }

    private void Update()
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

    private bool CanMoveTo(Vector3 direction)
    {
        RaycastHit hit;
        Physics.BoxCast(transform.position, Vector3.one * 0.45f, direction, out hit, Quaternion.identity, 1f, _wallLayer);
        return hit.collider == null;
    }
    
    public void Reset()
    {
        transform.position = _startingPosition;
        transform.rotation = Quaternion.LookRotation(_startingDirection);

        _direction = _startingDirection;
        _nextDirection = Vector3.zero;
    }


}
