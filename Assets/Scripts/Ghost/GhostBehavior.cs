using System.Collections;
using UnityEngine;

public enum GhostState { CHASE, SCATTER, FRIGHTENED }

public abstract class GhostBehavior : MonoBehaviour
{
    [SerializeField] private float _enterTime = 0f;
    [SerializeField] private Transform _home;
    [SerializeField] private LayerMask _wallLayer;

    private float _speed;

    private GhostState _state = GhostState.CHASE;

    private Vector3 _startingPosition;
    private Vector3 _direction = Vector3.right;
    private Vector3 _target;

    private Rigidbody _rigidbody;

    protected PacmanMovementArcade _pacmanMovement;

    protected abstract Vector3 ChaseTarget(); // override for each ghost

    private void Start()
    {
        // initialize variables
        _speed = GameManager.ghostSpeed;

        _pacmanMovement = 
            GameManager.Instance.Pacman.GetComponent<PacmanMovementArcade>();

        _startingPosition = transform.position;

        _rigidbody = GetComponent<Rigidbody>();

        // start initial routine
        StartCoroutine(EnterRoutine());
    }

    private void Update()
    {
        _rigidbody.MovePosition(_rigidbody.position + _direction * Time.deltaTime * _speed);
    }

    private IEnumerator EnterRoutine()
    {
        // wait in ghost house
        yield return new WaitForSeconds(_enterTime);
        
        // TODO: find a way to do this more smoothly
        _rigidbody.position = GameManager.Instance.DoorExit.position;
        _direction = Vector3.left;
        _rigidbody.MoveRotation(Quaternion.LookRotation(_direction));

        // start making decisions dynamically
        StartCoroutine(MoveRoutine());
    }

    private IEnumerator MoveRoutine()
    {
        while (true)
        {
            Vector3 nextDirection = GetBestDirection();

            // if change in direction
            if (nextDirection != transform.forward)
            {
                _direction = nextDirection;
                _rigidbody.MoveRotation(Quaternion.LookRotation(_direction));

                // to prevent turning 180
                yield return new WaitForSeconds(0.25f);
            }
            else 
            {
                yield return null;
            }
        }
    }

    // choose best direction for current _target
    private Vector3 GetBestDirection()
    {
        Vector3 nextDirection = transform.forward;
        float minDistance = float.PositiveInfinity;

        if (_state == GhostState.CHASE)
        {
            _target = ChaseTarget();
        }
        else if (_state == GhostState.SCATTER)
        {
            _target = _home.position;
        }

        if (CanMoveTo(transform.forward))
        {
            minDistance = DistanceToTargetInDirection(transform.forward);
        }

        if (CanMoveTo(transform.right))
        {
            float distance = DistanceToTargetInDirection(transform.right);
            if (distance < minDistance)
            {
                nextDirection = transform.right;
                minDistance = distance;
            }
        }

        if (CanMoveTo(-transform.right))
        {
            float distance = DistanceToTargetInDirection(-transform.right);
            if (distance < minDistance)
            {
                nextDirection = -transform.right;
            }
        }

        return nextDirection;
    }

    public float halfExtents = 0.45f;
    public float offset = 0.2f;

    // same as pacman's
    private bool CanMoveTo(Vector3 direction)
    {
        RaycastHit hit;
        Physics.BoxCast(transform.position - direction * offset, Vector3.one * halfExtents, direction, out hit, Quaternion.identity, 1f, _wallLayer);
        return hit.collider == null;
    }

    private void OnDrawGizmos()
    {
        if (_pacmanMovement != null)
        {
            Gizmos.color = Color.magenta;
            Gizmos.DrawRay(transform.position, _target - transform.position);
        }

        Vector3[] directions = new Vector3[] {transform.forward, transform.right, -transform.forward, -transform.right};

        foreach (Vector3 direction in directions)
        {
            RaycastHit hit;
            if (Physics.BoxCast(transform.position - direction * offset, Vector3.one * halfExtents, direction, out hit, Quaternion.identity, 1f, _wallLayer))
            {
                Gizmos.color = Color.red;
                Gizmos.DrawRay(transform.position - direction * offset, direction * hit.distance * 1.5f);
                Gizmos.DrawWireCube(transform.position + direction * hit.distance, Vector3.one * halfExtents * 2);
            }
            else
            {
                Gizmos.color = Color.green;
                Gizmos.DrawRay(transform.position - direction * offset, direction);
                Gizmos.DrawWireCube(transform.position + direction, Vector3.one * halfExtents * 2);
            }
        }
    }

    private float DistanceToTargetInDirection(Vector3 direction)
    {
        Vector3 offset = _target - (_rigidbody.position + direction);
        return new Vector3(offset.x, 0f, offset.z).sqrMagnitude;
    }

    public void Reset()
    {
        transform.position = _startingPosition;
        _direction = Vector3.right;
        _state = GhostState.CHASE;

        StopAllCoroutines();
        StartCoroutine(EnterRoutine());
    }

    public void SwitchState()
    {
        if (_state == GhostState.CHASE)
        {
            _state = GhostState.SCATTER;
        }
        else if (_state == GhostState.SCATTER)
        {
            _state = GhostState.CHASE;
        }

        _direction = -_direction;
        _rigidbody.MoveRotation(Quaternion.LookRotation(_direction)); // 이 한 줄 안써서 ㅅㅂ 개고생했네
    }

    public void OnEaten()
    {
        
    }

}
