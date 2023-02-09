using UnityEngine;

public class Warp : MonoBehaviour
{
    [SerializeField] private Transform _dest;

    private void OnCollisionEnter(Collision collision)
    {
        float y = collision.gameObject.transform.position.y;
        collision.gameObject.transform.position = new Vector3(_dest.position.x, y, _dest.position.z);
    }
}
