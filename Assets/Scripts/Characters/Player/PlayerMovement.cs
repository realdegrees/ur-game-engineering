using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField]
    [Range(0, 100)]
    private float moveSpeed = 50;

    private float horizontalInput;
    private Rigidbody2D rb;

    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("PlayerMovement Start called");
        rb = gameObject.GetComponent<Rigidbody2D>();
    }

    private void FixedUpdate()
    {
        Debug.Log("Horizontal Input: " + horizontalInput);
        Debug.Log("Move Speed: " + moveSpeed);
        Debug.Log("Velocity: " + rb.velocity);
        rb.velocity = new Vector2(horizontalInput * moveSpeed, rb.velocity.y);
    }

    public void Move(InputAction.CallbackContext context)
    {
        Debug.Log("Move called");
        horizontalInput = context.ReadValue<Vector2>().x;
    }
}
