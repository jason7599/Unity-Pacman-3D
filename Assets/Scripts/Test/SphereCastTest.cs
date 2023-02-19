using UnityEngine;

public class SphereCastTest : MonoBehaviour
{
    public float radius = .45f;
    public float maxDistance = 1f;
    public LayerMask wall;

    private void OnDrawGizmos()
    {
        Vector3[] directions = new Vector3[] {transform.forward, transform.right, -transform.forward, -transform.right};
        Vector3 center = transform.position;

        foreach (Vector3 direction in directions)
        {
            RaycastHit hit;
            if (Physics.SphereCast(center, radius, direction, out hit, maxDistance, wall))
            {
                Gizmos.color = Color.red;
                Gizmos.DrawRay(center, direction * hit.distance);
                Gizmos.DrawWireSphere(center + direction * hit.distance, radius);
            }
            else
            {
                Gizmos.color = Color.green;
                Gizmos.DrawRay(center, direction * maxDistance);
                Gizmos.DrawWireSphere(center + direction * maxDistance, radius);
            }
        }
    }
}
