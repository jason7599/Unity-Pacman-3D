using UnityEngine;

public class PinkyBehavior : GhostBehavior
{
    protected override Vector3 ChaseTarget()
    {
        Vector3 offset = _pacmanMovement.Direction;
        if (offset == Vector3.forward) offset += Vector3.left;
        return _pacmanMovement.Position + offset * 2f;
    }
}
