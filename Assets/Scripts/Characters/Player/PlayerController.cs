using UnityEngine;
using UnityEngine.InputSystem;

// ! While adding stuff keep in mind that this could also be used a base class for all entities that can move in the game
public class PlayerController : MonoBehaviour
{
    [Header("References")]
    public PlayerMovementConfig MovementConfig;
    [SerializeField] private Collider2D feetCollider;
    [SerializeField] private Collider2D bodyCollider;

    private Rigidbody2D rb;

    private Vector2 moveVelocity;
    private bool isFacingRight = true;

    private RaycastHit2D groundHit;
    private RaycastHit2D headHit;
    private bool isOnGround;
    private bool isHeadBlocked;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        FlipCheck();
    }

    private void FixedUpdate()
    {
        CheckCollisions();
        Debug.Log(InputManager.Instance.Movement);
        if (isOnGround) Move(MovementConfig.GroundAcceleration, MovementConfig.GroundDeceleration, InputManager.Instance.Movement);
        else Move(MovementConfig.AirAcceleration, MovementConfig.AirDeceleration, InputManager.Instance.Movement);
    }

    #region Movement

    public void Move(float acceleration, float deceleration, Vector2 moveInput)
    {
        if (moveInput != Vector2.zero)
        {
            moveVelocity = Vector2.Lerp(moveVelocity, moveInput * MovementConfig.MaxWalkSpeed, acceleration * Time.deltaTime);
        }
        else
        {
            moveVelocity = Vector2.Lerp(moveVelocity, Vector2.zero, deceleration * Time.deltaTime);
        }
        rb.velocity = new Vector2(moveVelocity.x, rb.velocity.y);
    }

    private void FlipCheck()
    {
        if (rb.velocity.x > 0 && !isFacingRight || rb.velocity.x < 0 && isFacingRight)
        {
            isFacingRight = !isFacingRight;
            transform.Rotate(0f, 180f, 0f);
        }
    }

    #endregion

    #region Collision Detection

    private void CheckCollisions()
    {
        IsOnGround();
        IsHeadBlocked();

        if (MovementConfig.DrawGizmos)
            DrawGizmos();
    }
    private void IsOnGround()
    {
        groundHit = Physics2D.CapsuleCast(feetCollider.bounds.center, feetCollider.bounds.size, CapsuleDirection2D.Horizontal, transform.eulerAngles.z, transform.TransformDirection(Vector2.down), MovementConfig.BottomRange, MovementConfig.GroundLayer);
        isOnGround = groundHit.collider != null;
    }

    private void IsHeadBlocked()
    {
        headHit = Physics2D.CapsuleCast(bodyCollider.bounds.center, bodyCollider.bounds.size, CapsuleDirection2D.Horizontal, transform.eulerAngles.z, transform.TransformDirection(Vector2.up), MovementConfig.TopRange, MovementConfig.GroundLayer);
        isHeadBlocked = headHit.collider != null;
    }

    #endregion

    #region Debug Visualization

    private void DrawGizmos()
    {
        if (isOnGround)
        {
            Debug.DrawRay(new Vector2(feetCollider.bounds.center.x, feetCollider.bounds.center.y), groundHit.point - (Vector2)feetCollider.bounds.center, Color.red);
        }
        if (isHeadBlocked)
        {
            Debug.DrawRay(new Vector2(bodyCollider.bounds.center.x, bodyCollider.bounds.center.y), headHit.point - (Vector2)bodyCollider.bounds.center, Color.red);
        }
    }

    #endregion
}
