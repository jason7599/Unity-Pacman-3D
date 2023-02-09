using UnityEngine;

public class BlinkyBehavior : GhostBehavior
{
    protected override Vector3 ChaseTarget()
    { 
        return _pacmanMovement.Position;
    }

}
