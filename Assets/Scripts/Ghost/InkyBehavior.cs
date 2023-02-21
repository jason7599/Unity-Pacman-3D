using UnityEngine;

public class InkyBehavior : GhostBehavior
{
    [SerializeField] private Transform _blinky;

    protected override Vector3 ChaseTarget()
    {
        Vector3 offset = _pacmanMovement.Direction;
        if (offset == Vector3.forward) offset += Vector3.left;

        Vector3 pivot = _pacmanMovement.Position + offset;

        // return pivot - (_blinky.position - pivot);
        return 2 * pivot - _blinky.position;
    }

}
