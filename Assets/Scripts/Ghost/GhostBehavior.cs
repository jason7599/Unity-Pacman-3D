using System.Collections;
using UnityEngine;

public enum GhostState { CHASE, SCATTER, FLEE }

public abstract class GhostBehavior : MonoBehaviour
{
    [SerializeField] private float _enterTime = 0f;
    [SerializeField] private Transform _home;

    private float _speed;

    private GhostState _state = GhostState.CHASE;
    private GhostState _prevState;

    private Vector3 _startingPosition;
    private Vector3 _direction = Vector3.right;
    private Vector3 _target;

    private Rigidbody _rigidbody;
    private LayerMask _wallLayer;

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

        _wallLayer = LayerMask.GetMask("Wall");
        
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

        // start making decisions dynamically
        StartCoroutine(ChangeDirection());
    }

    private IEnumerator ChangeDirection()
    {
        while (true)
        {
            UpdateTarget();

            Vector3 nextDirection = ChooseDirection();

            if (nextDirection != transform.forward)
            {
                _direction = nextDirection;
                _rigidbody.MoveRotation(Quaternion.LookRotation(_direction));
                
                // to prevent turning 180 degrees at once
                // TODO: not sure if this is enough, I think I witnessed blinky doing a 180
                yield return new WaitForSeconds(0.25f); 
            }
            else 
            {
                yield return null;
            }

        }
    }

    // choose best direction for current _target
    private Vector3 ChooseDirection()
    {
        Vector3 nextDirection = transform.forward;
        float minDistance = float.PositiveInfinity;

        if (CanMoveTo(transform.forward))
            minDistance = DistanceInDirection(transform.forward);

        if (CanMoveTo(transform.right))
        {
            float distance = DistanceInDirection(transform.right);
            if (distance < minDistance)
            {
                nextDirection = transform.right;
                minDistance = distance;
            }
        }

        if (CanMoveTo(-transform.right))
        {
            float distance = DistanceInDirection(-transform.right);
            if (distance < minDistance)
            {
                nextDirection = -transform.right;
                minDistance = distance;
            }
        }

        return nextDirection;
    }

    // same as pacman's
    private bool CanMoveTo(Vector3 direction)
    {
        RaycastHit hit;
        Physics.BoxCast(transform.position, Vector3.one * 0.45f, direction, out hit, Quaternion.identity, 1f, _wallLayer);
        return hit.collider == null;
    }

    // Set target based on current state
    private void UpdateTarget()
    {
        if (_state == GhostState.CHASE) _target = ChaseTarget();
        else if (_state == GhostState.SCATTER) _target = _home.position;
    }

    private float DistanceInDirection(Vector3 direction)
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
        if (_state == GhostState.CHASE) _state = GhostState.SCATTER;
        else if (_state == GhostState.SCATTER) _state = GhostState.CHASE;

        _direction = -_direction;
    }

}
