using System;
using UnityEngine;
using UnityEngine.InputSystem;

// ! While adding stuff keep in mind that this could also be used a base class for all entities that can move in the game
public class PlayerController : MonoBehaviour
{
    [Header("References")]
    public PlayerMovementConfig MovementConfig;
    [SerializeField] private Collider2D feetCollider;
    [SerializeField] private Collider2D bodyCollider;
    [SerializeField] private Collider2D headCollider;

    private Rigidbody2D rb;

    private Vector2 moveVelocity;
    private bool isFacingRight = true;

    private RaycastHit2D groundHit;
    private RaycastHit2D headHit;
    private bool isOnGround;
    private bool isHeadBlocked;

    public float VerticalVelocity { get; private set; }
    private bool isJumping;
    private bool isFastFalling;
    private bool isFalling;
    private float fastFallTime;
    private float fastFallReleaseSpeed;
    private int numberOfJumpsUsed;

    private float apexPoint;
    private float timePastApexThreshold;
    private bool isPastApexThreshold;

    private float jumpBufferTimer;
    private bool jumpReleaseDuringBuffer;

    private float coyoteTimer;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        UpdateTimers();
        FlipCheck();
        JumpCheck();
    }

    private void FixedUpdate()
    {
        CheckCollisions();
        Jump();

        if (isOnGround) Move(MovementConfig.GroundAcceleration, MovementConfig.GroundDeceleration, InputManager.Instance.Movement);
        else Move(MovementConfig.AirAcceleration, MovementConfig.AirDeceleration, InputManager.Instance.Movement);
    }

    #region Movement

    public void Move(float acceleration, float deceleration, Vector2 moveInput)
    {
        if (moveInput != Vector2.zero)
        {
            moveVelocity = Vector2.Lerp(moveVelocity, moveInput * MovementConfig.MaxWalkSpeed, acceleration * Time.fixedDeltaTime);
        }
        else
        {
            moveVelocity = Vector2.Lerp(moveVelocity, Vector2.zero, deceleration * Time.fixedDeltaTime);
        }
        rb.velocity = new Vector2(moveVelocity.x, rb.velocity.y);
    }

    private void FlipCheck()
    {
        if (rb.velocity.x > 5f && !isFacingRight || rb.velocity.x < -5f && isFacingRight)
        {
            isFacingRight = !isFacingRight;
            transform.Rotate(0f, 180f, 0f);
        }
    }

    #endregion

    #region Jumping

    private void JumpCheck()
    {
        if (InputManager.Instance.JumpPressed)
        {
            jumpBufferTimer = MovementConfig.JumpBufferTime;
            jumpReleaseDuringBuffer = false;
        }
        if (InputManager.Instance.JumpReleased)
        {
            if (jumpBufferTimer > 0f)
            {
                jumpReleaseDuringBuffer = true;
            }
            if (isJumping && VerticalVelocity > 0f)
            {
                isFastFalling = true;
                if (isPastApexThreshold)
                {
                    isPastApexThreshold = false;
                    fastFallTime = MovementConfig.TimeForUpwardsCancel;
                    VerticalVelocity = 0f;
                }
                else
                {
                    fastFallReleaseSpeed = VerticalVelocity;
                }
            }
        }

        // Jump with fall buffering and coyote time
        if (jumpBufferTimer > 0f && !isJumping && (isOnGround || coyoteTimer > 0f))
        {
            InitiateJump(1);

            if (jumpReleaseDuringBuffer)
            {
                isFastFalling = true;
                fastFallReleaseSpeed = VerticalVelocity;
            }
        }

        // Double Jump
        else if (jumpBufferTimer > 0f && isJumping && numberOfJumpsUsed < MovementConfig.NumberOfJumpsAllowed)
        {
            isFastFalling = false;
            InitiateJump(1);
        }
        // Air jump after coyote time
        else if (jumpBufferTimer > 0f && isFalling && numberOfJumpsUsed < MovementConfig.NumberOfJumpsAllowed - 1)
        {
            InitiateJump(2);
            isFastFalling = false;
        }
        // Landing
        if ((isJumping || isFalling) && isOnGround && VerticalVelocity <= 0f)
        {
            Debug.Log("Landed");
            isJumping = false;
            isFastFalling = false;
            isFalling = false;
            fastFallTime = 0f;
            isPastApexThreshold = false;
            numberOfJumpsUsed = 0;
            coyoteTimer = MovementConfig.JumpCoyoteTime;
            VerticalVelocity = Physics2D.gravity.y; ;
        }
    }

    private void InitiateJump(int cost)
    {
        Debug.Log("Jumped");
        if (!isJumping)
        {
            isJumping = true;
        }
        jumpBufferTimer = 0f;
        numberOfJumpsUsed += cost;
        VerticalVelocity = MovementConfig.InitialJumpVelocity;
    }
    private void Jump()
    {
        if (isJumping)
        {
            // Check head collision
            if (isHeadBlocked)
            {
                isFastFalling = true;
            }

            // Handle Gravity when ascending
            if (VerticalVelocity >= 0f)
            {
                apexPoint = Mathf.InverseLerp(MovementConfig.InitialJumpVelocity, 0f, VerticalVelocity);

                // Gravity when ascending after apex
                if (apexPoint >= MovementConfig.ApexThreshold)
                {
                    if (!isPastApexThreshold)
                    {
                        isPastApexThreshold = true;
                        timePastApexThreshold = 0f;
                    }

                    if (isPastApexThreshold)
                    {
                        timePastApexThreshold += Time.fixedDeltaTime;
                        if (timePastApexThreshold < MovementConfig.ApexHangTime)
                        {
                            VerticalVelocity = 0f;
                        }
                        else
                        {
                            VerticalVelocity = -.1f;
                        }
                    }
                }
                // Gravity when ascending before apex
                else
                {
                    VerticalVelocity += MovementConfig.Gravity * Time.fixedDeltaTime;
                    if (isPastApexThreshold)
                    {
                        isPastApexThreshold = false;
                    }
                }
            }
            // Handle Gravity when descending
            else if (!isFastFalling)
            {
                VerticalVelocity += MovementConfig.Gravity * MovementConfig.GravityOnReleaseMultiplier * Time.fixedDeltaTime;
            }
            else if (VerticalVelocity < 0f)
            {
                if (!isFalling)
                {
                    isFalling = true;
                }
            }
        }

        // Cut Jump
        if (isFastFalling)
        {
            if (fastFallTime >= MovementConfig.TimeForUpwardsCancel)
            {
                VerticalVelocity += MovementConfig.Gravity * MovementConfig.GravityOnReleaseMultiplier * Time.fixedDeltaTime;
            }
            else if (fastFallTime < MovementConfig.TimeForUpwardsCancel)
            {
                VerticalVelocity = Mathf.Lerp(fastFallReleaseSpeed, 0f, fastFallTime / MovementConfig.TimeForUpwardsCancel);
            }
            fastFallTime += Time.fixedDeltaTime;
        }

        // Gravity without jumping
        if (!isOnGround && !isJumping)
        {
            if (!isFalling)
            {
                isFalling = true;
            }
            VerticalVelocity += MovementConfig.Gravity * Time.fixedDeltaTime;
        }

        // Clamp vertical velocity
        VerticalVelocity = Mathf.Clamp(VerticalVelocity, -MovementConfig.MaxFallSpeed, 50f);
        rb.velocity = new Vector2(rb.velocity.x, VerticalVelocity);
    }

    #endregion

    #region Collision Detection

    private void CheckCollisions()
    {
        IsOnGround();
        IsHeadBlocked();
    }
    private void IsOnGround()
    {
        groundHit = Physics2D.CapsuleCast(feetCollider.bounds.center, feetCollider.bounds.size, CapsuleDirection2D.Horizontal, 0f, Vector2.down, MovementConfig.BottomRange, MovementConfig.GroundLayer);
        isOnGround = groundHit.collider != null;
    }

    private void IsHeadBlocked()
    {

        headHit = Physics2D.CapsuleCast(headCollider.bounds.center, headCollider.bounds.size, CapsuleDirection2D.Horizontal, 0f, Vector2.up, MovementConfig.TopRange, MovementConfig.GroundLayer);
        isHeadBlocked = headHit.collider != null;
        if (isHeadBlocked)
        {
            Vector2 headToHit = headHit.point - (Vector2)headCollider.bounds.center;
            float angle = Vector2.Angle(Vector2.up, headToHit);
            Debug.Log("Angle: " + angle);
            isHeadBlocked = angle <= 25f;
        }

    }

    #endregion

    #region Debug Visualization

    private void OnDrawGizmos()
    {
        if (isOnGround)
        {
            Debug.DrawRay(new Vector2(feetCollider.bounds.center.x, feetCollider.bounds.center.y), groundHit.point - (Vector2)feetCollider.bounds.center, Color.red);
        }
        if (isHeadBlocked)
        {
            Debug.DrawRay(new Vector2(headCollider.bounds.center.x, headCollider.bounds.center.y), headHit.point - (Vector2)headCollider.bounds.center, Color.red);
        }
        if (MovementConfig.ShowJumpArc)
            DrawJumpArc(MovementConfig.MaxWalkSpeed, Color.white);
    }

    private void DrawJumpArc(float moveSpeed, Color gizmoColor)
    {
        Vector2 startPosition = new(feetCollider.bounds.center.x, feetCollider.bounds.min.y);
        Vector2 previousPosition = startPosition;

        float speed = isFacingRight ? moveSpeed : -moveSpeed;

        Vector2 velocity = new(speed, MovementConfig.InitialJumpVelocity);

        Gizmos.color = gizmoColor;

        float timeStep = 2 * MovementConfig.TimeTillJumpApex / MovementConfig.ArcResolution; // time step for the simulation
                                                                                             // float totalTime = (2 * MovementConfig.TimeTillJumpApex) + MovementConfig.ApexHangTime; // total time of the arc including hang time

        for (int i = 0; i < MovementConfig.VisualizationSteps; i++)
        {
            float simulationTime = i * timeStep;
            Vector2 displacement;
            Vector2 drawPoint;

            if (simulationTime < MovementConfig.TimeTillJumpApex) // Ascending
            {
                displacement = velocity * simulationTime + simulationTime * simulationTime * 0.5f * new Vector2(0f, MovementConfig.Gravity);
            }
            else if (simulationTime < MovementConfig.TimeTillJumpApex + MovementConfig.ApexHangTime) // Apex hang time
            {
                float apexTime = simulationTime - MovementConfig.TimeTillJumpApex;
                displacement = velocity * MovementConfig.TimeTillJumpApex + 0.5f * MovementConfig.TimeTillJumpApex * MovementConfig.TimeTillJumpApex * new Vector2(0f, MovementConfig.Gravity);
                displacement += new Vector2(speed, 0) * apexTime; // No vertical movement during hang time
            }
            else // Descending
            {
                float descendTime = simulationTime - (MovementConfig.TimeTillJumpApex + MovementConfig.ApexHangTime);
                displacement = velocity * MovementConfig.TimeTillJumpApex + 0.5f * MovementConfig.TimeTillJumpApex * MovementConfig.TimeTillJumpApex * new Vector2(0f, MovementConfig.Gravity);
                displacement += new Vector2(speed, 0f) * MovementConfig.ApexHangTime; // Horizontal movement during hang time
                displacement += new Vector2(speed, 0f) * descendTime + 0.5f * descendTime * descendTime * new Vector2(0f, MovementConfig.Gravity);
            }

            drawPoint = startPosition + displacement;

            if (MovementConfig.StopOnCollision)
            {
                RaycastHit2D hit = Physics2D.Raycast(previousPosition, drawPoint - previousPosition, Vector2.Distance(previousPosition, drawPoint), MovementConfig.GroundLayer);
                if (hit.collider != null)
                {
                    // If a hit is detected, stop drawing the arc at the hit point
                    Gizmos.DrawLine(previousPosition, hit.point);
                    break;
                }
            }

            Gizmos.DrawLine(previousPosition, drawPoint);
            previousPosition = drawPoint;
        }
    }


    #endregion

    #region Timers

    private void UpdateTimers()
    {
        if (jumpBufferTimer > 0f)
            jumpBufferTimer -= Time.deltaTime;

        if (!isOnGround && coyoteTimer > 0f)
        {
            coyoteTimer -= Time.deltaTime;
            Debug.Log(coyoteTimer);
        }
    }
    #endregion
}
