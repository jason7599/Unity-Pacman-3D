using UnityEngine;

public class BlinkyBehavior : GhostBehavior
{
    protected override Vector3 Target { get { return _pacmanMovement.Position; } }
}
