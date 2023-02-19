using UnityEngine;

public class CastTest : MonoBehaviour
{
    public LayerMask _wallLayer;
    public float halfExtents = .45f;
    public float offset = .1f;

    private void OnDrawGizmos()
    {
        Vector3[] directions = new Vector3[] {transform.forward, transform.right, -transform.forward, -transform.right};

        foreach (Vector3 direction in directions)
        {
            // 좀 뒤에서 쏘자
            Vector3 center = transform.position - direction * offset;

            RaycastHit hit;
            if (Physics.BoxCast(center, Vector3.one * halfExtents, direction, out hit, Quaternion.identity, 1f, _wallLayer))
            {
                Gizmos.color = Color.red;
                Gizmos.DrawRay(center, direction * hit.distance * 1.5f);
                Gizmos.DrawWireCube(transform.position + direction * hit.distance, Vector3.one * halfExtents * 2);
            }
            else
            {
                Gizmos.color = Color.green;
                Gizmos.DrawRay(center, direction);
                Gizmos.DrawWireCube(transform.position + direction, Vector3.one * halfExtents * 2);
            }
        }
    }

}
