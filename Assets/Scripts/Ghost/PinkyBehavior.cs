using UnityEngine;

public class PinkyBehavior : GhostBehavior
{
    protected override Vector3 Target 
    {
        get
        {
            return _pacmanMovement.Position + _pacmanMovement.Direction * 2f;
        }
    }
}
