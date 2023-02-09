using System.Collections;
using UnityEngine;

public abstract class GhostBehavior : MonoBehaviour
{
    [SerializeField] private float _speed = 3f;
    [SerializeField] private float _enterTime = 0f;

    protected PacmanMovementArcade _pacmanMovement;
    protected abstract Vector3 Target { get; }

    private Vector3 _startingPosition;
    private Vector3 _startingDirection;
    private Vector3 _direction = Vector3.zero;

    private Rigidbody _rigidbody;
    private LayerMask _wallLayer;

    private void Start()
    {
        _pacmanMovement = 
            GameManager.Instance.Pacman.GetComponent<PacmanMovementArcade>();

        _startingPosition = transform.position;
        _startingDirection = transform.forward;

        _rigidbody = GetComponent<Rigidbody>();

        _wallLayer = LayerMask.GetMask("Wall");
        
        StartCoroutine(EnterRoutine());
    }

    private void Update()
    {
        _rigidbody.MovePosition(_rigidbody.position + _direction * Time.deltaTime * _speed);
    }

    private IEnumerator EnterRoutine()
    {
        yield return new WaitForSeconds(_enterTime);
        
        _rigidbody.position = GameManager.Instance.DoorExit;
        _direction = _startingDirection;

        StartCoroutine(ChangeDirection());
    }

    private IEnumerator ChangeDirection()
    {
        while (true)
        {
            Vector3 nextDirection = ChooseDirection();

            if (nextDirection != transform.forward)
            {
                _direction = nextDirection;
                _rigidbody.MoveRotation(Quaternion.LookRotation(_direction));
                
                yield return new WaitForSeconds(0.25f); // to prevent turning 180 degrees at once
            }
            else 
            {
                yield return null;
            }
        }
    }

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

    private bool CanMoveTo(Vector3 direction)
    {
        RaycastHit hit;
        Physics.BoxCast(transform.position, Vector3.one * 0.45f, direction, out hit, Quaternion.identity, 1f, _wallLayer);
        return hit.collider == null;
    }

    private float DistanceInDirection(Vector3 direction)
    {
        Vector3 offset = Target - (_rigidbody.position + direction);
        return new Vector3(offset.x, 0f, offset.z).sqrMagnitude;
    }

    public void Reset()
    {
        transform.position = _startingPosition;
        transform.rotation = Quaternion.LookRotation(_startingDirection);
        _direction = Vector3.zero;

        StopAllCoroutines();
        StartCoroutine(EnterRoutine());
    }

    // private void OnDrawGizmos()
    // {
    //     Gizmos.color = Color.red;
    //     Gizmos.DrawRay(transform.position, Target - transform.position);
    // }

}
