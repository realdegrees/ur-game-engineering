using UnityEngine;

public class Spin : MonoBehaviour
{
    [Range(0, 100)]
    public float speed = 10f;
    private Rigidbody2D rb;
    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.constraints = RigidbodyConstraints2D.FreezePosition;
            rb.isKinematic = true;
        }
    }
    // Update is called once per frame
    void Update()
    {
        if (rb == null)
        {
            transform.Rotate(Vector3.forward * speed * Time.deltaTime);
        }
    }
    private void FixedUpdate()
    {
        if (rb != null)
        {
            rb.MoveRotation(rb.rotation + speed * Time.fixedDeltaTime);
        }
    }
}
