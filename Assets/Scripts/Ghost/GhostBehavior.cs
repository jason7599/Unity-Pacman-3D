using System.Collections;
using UnityEngine;

public enum GhostState { CHASE, SCATTER, FRIGHTENED }

public abstract class GhostBehavior : MonoBehaviour
{
    [SerializeField] private float _enterTime = 0f;
    [SerializeField] private Transform _home;
    [SerializeField] private LayerMask _wallLayer;

    private GhostState _state = GhostState.SCATTER;
    private GhostState _prevState = GhostState.SCATTER;

    private float _speed;
    private float _speedMultiplier = 1f;
    private Vector3 _startingPosition;
    private Vector3 _direction = Vector3.right;
    private Vector3 _target;
    private Rigidbody _rigidbody;
    private Vector3 _doorExitPos;
    private Vector3 _spawnPos;

    private Renderer _renderer;
    private Material _material;
    private Material _frightenedMaterial;

    protected PacmanMovementArcade _pacmanMovement;
    protected abstract Vector3 ChaseTarget(); // override for each ghost

    private void Start()
    {
        // initialize variables
        _speed = GameManager.ghostSpeed;
        _pacmanMovement = 
            GameManager.Instance.Pacman;
        _startingPosition = transform.position;
        _rigidbody = GetComponent<Rigidbody>();
        _doorExitPos = GameManager.Instance.DoorExit.position;
        _spawnPos = GameManager.Instance.Spawn.position;
        _renderer = GetComponent<Renderer>();
        _material = _renderer.material;
        _frightenedMaterial = 
            GameManager.Instance.FrightenedMaterial;

        // start initial routine
        StartCoroutine(InitRoutine());
    }

    private void FixedUpdate()
    {
        _rigidbody.MovePosition(_rigidbody.position + _direction * Time.fixedDeltaTime * _speed * _speedMultiplier);
    }

    private void SetDirection(Vector3 direction)
    {
        _direction = direction;
        _rigidbody.MoveRotation(Quaternion.LookRotation(direction));
    }

    private IEnumerator InitRoutine()
    {
        if (_enterTime != 0f)
        {
            yield return StartCoroutine(IdleHome(_enterTime));
        }

        yield return StartCoroutine(ExitHome());
    }

    private IEnumerator Respawn()
    {
        transform.position = _spawnPos;
        _state = _prevState;

        yield return StartCoroutine(ExitHome());
    }

    // bounce around in home
    private IEnumerator IdleHome(float idleTime)
    {
        SetDirection(Vector3.forward);

        float elapsed = 0f;
        while (elapsed < idleTime)
        {
            if (!CanMoveTo(transform.forward))
                SetDirection(-transform.forward);
            
            elapsed += Time.deltaTime;
            yield return null;
        }
    }

    private IEnumerator ExitHome()
    {
        // transform.position = new Vector3(_doorExitPos.x, transform.position.y, transform.position.z);
        SetDirection(Vector3.forward);
        _rigidbody.isKinematic = true;

        while (true)
        {
            float offset = _doorExitPos.z - transform.position.z;

            if (offset < 0f) break;

            transform.position += Vector3.forward * _speed * Time.deltaTime;
            yield return null;
        }

        // transform.position = new Vector3(_doorExitPos.x, transform.position.y, _doorExitPos.z);
        transform.position = _doorExitPos;
        SetDirection(Vector3.left);
        _rigidbody.isKinematic = false;

        yield return StartCoroutine(MoveRoutine());
    }

    private IEnumerator MoveRoutine()
    {
        while (true)
        {
            Vector3 nextDirection = GetBestDirection();

            // if change in direction
            if (nextDirection != transform.forward)
            {
                SetDirection(nextDirection);
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

    [SerializeField] private float halfExtents = 0.45f;
    [SerializeField] private float offset = 0.2f;

    private bool CanMoveTo(Vector3 direction)
    {
        RaycastHit hit;
        Physics.BoxCast(transform.position - direction * offset, Vector3.one * halfExtents, direction, out hit, Quaternion.identity, 1f, _wallLayer);
        return hit.collider == null;
    }

    private float DistanceToTargetInDirection(Vector3 direction)
    {
        Vector3 offset = _target - (_rigidbody.position + direction);
        return new Vector3(offset.x, 0f, offset.z).sqrMagnitude;
    }

    public void Reset()
    {
        transform.position = _startingPosition;
        _state = _prevState;

        StopAllCoroutines();

        StartCoroutine(InitRoutine());
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

        SetDirection(-_direction);
    }

    public void EnterFrightened()
    {
        _prevState = _state;
        _state = GhostState.FRIGHTENED;
        _speedMultiplier = 0.75f;
        _renderer.material = _frightenedMaterial;

        SetDirection(-transform.forward);
    }

    public void ExitFrightened()
    {
        if (_state == GhostState.FRIGHTENED)
        {
            _state = _prevState;
            _speedMultiplier = 1f;
            _renderer.material = _material;
        }
    }

    public void OnEaten()
    {
        ExitFrightened();
        
    }

    // private void OnDrawGizmos()
    // {
    //     if (_pacmanMovement != null)
    //     {
    //         Gizmos.color = Color.magenta;
    //         Gizmos.DrawRay(transform.position, _target - transform.position);
    //     }

    //     Vector3[] directions = new Vector3[] {transform.forward, transform.right, -transform.forward, -transform.right};

    //     foreach (Vector3 direction in directions)
    //     {
    //         RaycastHit hit;
    //         if (Physics.BoxCast(transform.position - direction * offset, Vector3.one * halfExtents, direction, out hit, Quaternion.identity, 1f, _wallLayer))
    //         {
    //             Gizmos.color = Color.red;
    //             Gizmos.DrawRay(transform.position - direction * offset, direction * hit.distance * 1.5f);
    //             Gizmos.DrawWireCube(transform.position + direction * hit.distance, Vector3.one * halfExtents * 2);
    //         }
    //         else
    //         {
    //             Gizmos.color = Color.green;
    //             Gizmos.DrawRay(transform.position - direction * offset, direction);
    //             Gizmos.DrawWireCube(transform.position + direction, Vector3.one * halfExtents * 2);
    //         }
    //     }
    // }

}
