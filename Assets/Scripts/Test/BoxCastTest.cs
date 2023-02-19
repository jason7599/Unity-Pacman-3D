using UnityEngine;

public class BoxCastTest : MonoBehaviour
{
    public float boxSize = .45f;
    public float maxDistance = 1f;
    public LayerMask wall;

    private void OnDrawGizmos()
    {
        Vector3[] directions = new Vector3[] {transform.forward, transform.right, -transform.forward, -transform.right};
        Vector3 center = transform.position;

        foreach (Vector3 direction in directions)
        {
            
            RaycastHit hit;
            if (Physics.BoxCast(center, Vector3.one * boxSize * .5f, direction, out hit, Quaternion.identity, maxDistance, wall))
            {
                Gizmos.color = Color.red;
                Gizmos.DrawRay(center, direction * hit.distance);
                Gizmos.DrawWireCube(center + direction * hit.distance, Vector3.one * boxSize);
            }
            else
            {
                Gizmos.color = Color.green;
                Gizmos.DrawRay(center, direction * maxDistance);
                Gizmos.DrawWireCube(center + direction * maxDistance, Vector3.one * boxSize);
            }
        }
    }
}
