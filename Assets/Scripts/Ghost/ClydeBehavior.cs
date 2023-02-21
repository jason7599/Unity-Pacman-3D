using UnityEngine;

public class ClydeBehavior : GhostBehavior
{
    [SerializeField] private float _chaseRadius = 64f;

    protected override Vector3 ChaseTarget()
    {
        if ((_pacmanMovement.Position - transform.position).sqrMagnitude > _chaseRadius)
        {
            return _pacmanMovement.Position;
        }
        else
        {
            return _home.position;
        }
    }
}
