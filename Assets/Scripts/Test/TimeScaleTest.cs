using UnityEngine;

public class TimeScaleTest : MonoBehaviour
{
    private Rigidbody _rigidbody;

    private void Start()
    {
        _rigidbody = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Jump();
        }
    }

    private void Jump()
    {
        if (_rigidbody.velocity.y > 0f)
            return;

        _rigidbody.AddForce(Vector3.up * 100f);
    }

}
