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

    private Vector2 movementVector;
    private Vector2 desiredMovementVector;
    private bool isFacingRight = true;

    private RaycastHit2D groundHit;
    private RaycastHit2D headHit;
    private bool isOnGround;
    private bool isOnSlope;
    private bool isOnUpSlope;
    private float slopeAngle;

    private float groundAngle;

    private bool isHeadBlocked;
    private Collider2D connectedGround;
    private Collider2D connectedCeiling;

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

    private Bounds playerBounds;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        UpdateTimers();
        FlipCheck();
        if (Math.Abs(groundAngle) <= MovementConfig.MaxSlopeAngle) JumpCheck();
    }

    private void FixedUpdate()
    {
        // ! TODO: probably not the smartest way to build these bounds, maybe use a composite collider?
        playerBounds = headCollider.bounds;
        playerBounds.Encapsulate(bodyCollider.bounds);
        playerBounds.Encapsulate(feetCollider.bounds);

        CheckCollisions();
        Jump();
        Move();
    }


    #region Movement

    public void Move()
    {
        // Set acceleration and deceleration based on whether the player is on the ground or in the air
        float acceleration = isOnGround ? MovementConfig.GroundAcceleration : MovementConfig.AirAcceleration;
        float deceleration = isOnGround ? MovementConfig.GroundDeceleration : MovementConfig.AirDeceleration;

        movementVector = rb.velocity; // use the actual velocity as base for calculations

        // Accelerate
        if (InputManager.Instance.Movement != 0)
        {
            desiredMovementVector = CalculateDesiredMovementVector();
            desiredMovementVector = ApplySlopeModifiers(desiredMovementVector, acceleration);
            movementVector = Vector2.Lerp(movementVector, desiredMovementVector, acceleration * Time.fixedDeltaTime); // Lerp the current velocity to the desired one based on the acceleration
        }

        // Decelerate
        else
        {
            movementVector = Vector2.Lerp(movementVector, Vector2.zero, deceleration * Time.fixedDeltaTime);
        }

        rb.velocity = movementVector;
    }

    private Vector2 ApplySlopeModifiers(Vector2 movementVector, float acceleration)
    {

        if (isOnSlope)
        {
            var wantsToWalkUp = desiredMovementVector.y > 0;
            var canWalkUp = slopeAngle <= MovementConfig.MaxSlopeAngle;
            var slopeSpeedMultiplier = isOnUpSlope ? MovementConfig.UpSlopeSpeedMultiplier : MovementConfig.DownSlopeSpeedMultiplier;
            if (wantsToWalkUp) // If the player wants to walk up a slope that is too steep, don't allow it
            {
                if (canWalkUp)
                {
                    var slowdown = 1 - (slopeAngle / MovementConfig.MaxSlopeAngle);
                    var slopeAngleSlowdownMultiplier = Mathf.Lerp(1, slowdown, MovementConfig.SlopeSteepnessFactor);
                    movementVector *= slopeAngleSlowdownMultiplier * slopeSpeedMultiplier; // Lerp the current velocity to the desired one based on the acceleration
                }
                else
                {
                    var rejectionForce = Mathf.Pow(MovementConfig.SlopeRejectionForce, 2); // Adjust the curve so that 0.5 becomes 0.25
                    movementVector = Vector2.Lerp(movementVector, -movementVector, rejectionForce + 0.5f); // Lerp the current velocity to the opposite direction based on the acceleration
                // TODO: maybe trigger an animation or sound effect to indicate that the player can't walk up the slope
            }

            }
            else
            {
                var speedup = 1 + (slopeAngle / MovementConfig.MaxSlopeAngle);
                var slopeAngleSpeedupMultiplier = Mathf.Lerp(1, speedup, MovementConfig.SlopeSteepnessFactor);
                movementVector *= slopeAngleSpeedupMultiplier * slopeSpeedMultiplier;
            }
        }
        if (!isOnSlope) movementVector.y = rb.velocity.y; // If not on slope, preserve the y velocity
        // Apply the slope effect multiplier to the velocity
        return movementVector;

    }

    private Vector2 CalculateDesiredMovementVector()
    {
        // calculate the direction the player should move in based on the input and the normal of the ground the player is standing on
        var desiredDirection = GetDesiredDirection();


        var velocity = MovementConfig.MaxWalkSpeed * desiredDirection; // Construct the desired movement vector based on the maximum walk speed, slope effect multiplier and the desired direction
        return velocity;
    }

    private Vector2 GetDesiredDirection()
    {
        return -InputManager.Instance.Movement * Vector2.Perpendicular(groundHit.normal).normalized;
    }

    private void FlipCheck()
    {
        if (rb.velocity.x > MovementConfig.TurnThreshold && !isFacingRight || rb.velocity.x < -MovementConfig.TurnThreshold && isFacingRight)
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
            isFastFalling = false;
            InitiateJump(2);
        }
        // Landing
        if ((isJumping || isFalling) && isOnGround && VerticalVelocity <= 0f)
        {
            isJumping = false;
            isFastFalling = false;
            isFalling = false;
            fastFallTime = 0f;
            isPastApexThreshold = false;
            numberOfJumpsUsed = 0;
            coyoteTimer = MovementConfig.JumpCoyoteTime;
            VerticalVelocity = 0f;
        }
    }

    private void InitiateJump(int cost)
    {
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
            if (isHeadBlocked) isFastFalling = true;
            ApplyJumpGravity();
        }

        // Cut Jump
        if (isFastFalling)
        {
            HandleCutJump();
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

        if (!isOnGround || isJumping)
        {
            // Clamp vertical velocity
            VerticalVelocity = Mathf.Clamp(VerticalVelocity, -MovementConfig.MaxFallSpeed, 50f);
            rb.velocity = new Vector2(rb.velocity.x, VerticalVelocity);
        }
    }

    private void HandleCutJump()
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

    private void ApplyJumpGravity()
    {
        // Handle Gravity when ascending
        if (VerticalVelocity >= 0f)
        {
            apexPoint = Mathf.InverseLerp(MovementConfig.InitialJumpVelocity, 0f, VerticalVelocity);

            // Gravity when ascending after apex (manages hang time)
            if (apexPoint > MovementConfig.ApexThreshold)
            {
                HandleApex();
            }
            // Gravity when ascending normally during jump
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
            VerticalVelocity += MovementConfig.Gravity * Time.fixedDeltaTime;
        }
        else if (VerticalVelocity < 0f)
        {
            if (!isFalling)
            {
                isFalling = true;
            }
        }
    }

    private void HandleApex()
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

    #endregion

    #region Collision Detection

    private void CheckCollisions()
    {
        IsOnGround();
        IsHeadBlocked();
    }
    private void IsOnGround()
    {
        var checkDirection = -transform.up;
        groundHit = Physics2D.CapsuleCast(feetCollider.bounds.center, feetCollider.bounds.size, CapsuleDirection2D.Horizontal, 0f, checkDirection, MovementConfig.BottomRange, MovementConfig.GroundLayer);
        isOnGround = groundHit.collider != null;
        isOnSlope = false;
        isOnUpSlope = false;
        groundAngle = 0;
        if (isOnGround)
        {
            float angle = Vector2.Angle(-checkDirection, groundHit.normal);
            groundAngle = Mathf.RoundToInt(angle);
            isOnSlope = angle > 2f;
            isOnUpSlope = isOnSlope && (isFacingRight ? groundHit.normal.x < 0 : groundHit.normal.x > 0);
            slopeAngle = Vector2.Angle(Vector2.up, groundHit.normal); // TODO: change Vector2.up to the inverse of the gravity vector
        }
        else
        {
            groundHit.normal = Vector2.up;
        }
        connectedGround = isOnGround ? groundHit.collider : null;
    }

    private void IsHeadBlocked()
    {
        var checkDirection = transform.up;
        headHit = Physics2D.CapsuleCast(headCollider.bounds.center, headCollider.bounds.size, CapsuleDirection2D.Horizontal, 0f, checkDirection, MovementConfig.TopRange, MovementConfig.GroundLayer);
        isHeadBlocked = headHit.collider != null;
        if (isHeadBlocked)
        {
            Vector2 headToHit = headHit.point - (Vector2)headCollider.bounds.center;
            float angle = Vector2.Angle(Vector2.up, headToHit);
            isHeadBlocked = angle <= 25f;
            connectedCeiling = isHeadBlocked ? headHit.collider : null;
        }
        else
        {
            headHit.normal = Vector2.down;
        }
    }

    #endregion

    #region Debug Visualization

    private void OnDrawGizmos()
    {
        if (MovementConfig.CollisionGizmos)
            DrawCollisionGizmos();
        if (MovementConfig.VelocityGizmos)
            DrawVelocityGizmos();
        if (MovementConfig.Show)
            DrawJumpArc();
    }

    private void DrawCollisionGizmos()
    {
        Gizmos.color = Color.blue;
        if (isOnGround)
        {
            Gizmos.DrawRay(new Vector2(feetCollider.bounds.center.x, feetCollider.bounds.center.y), groundHit.point - (Vector2)feetCollider.bounds.center);
        }
        if (isHeadBlocked)
        {
            Gizmos.DrawRay(new Vector2(headCollider.bounds.center.x, headCollider.bounds.center.y), headHit.point - (Vector2)headCollider.bounds.center);
        }
    }
    private void DrawVelocityGizmos()
    {
        Debug.DrawRay(playerBounds.center, movementVector, Color.green);
        Gizmos.color = Color.red;
        Gizmos.DrawCube((Vector2)playerBounds.center + desiredMovementVector, Vector3.one * 0.1f);
        Gizmos.DrawRay(playerBounds.center, desiredMovementVector.normalized * 1);

    }

    private class JumpArcData
    {
        public Vector2 startPosition;
        public Vector2 velocity;
        public float timeStep;
        public Color color;
        public Vector2? targetPosition;
    }
    private readonly JumpArcData jumpArcData = new();

    private void DrawJumpArc()
    {
        float speed = isFacingRight ? MovementConfig.MaxWalkSpeed : -MovementConfig.MaxWalkSpeed;

        if (isOnGround)
        {
            jumpArcData.startPosition = playerBounds.center;
            jumpArcData.velocity = new(speed, MovementConfig.InitialJumpVelocity);
            jumpArcData.timeStep = 2 * MovementConfig.TimeTillJumpApex / MovementConfig.ArcResolution;
            jumpArcData.color = Color.white;
        }
        else
        {
            if (!isJumping) return;
            jumpArcData.color = Color.gray;
        }
        Vector2 previousPosition = jumpArcData.startPosition;

        for (int i = 0; i < MovementConfig.VisualizationSteps; i++)
        {
            float simulationTime = i * jumpArcData.timeStep;
            Vector2 displacement;
            Vector2 drawPoint;

            if (simulationTime < MovementConfig.TimeTillJumpApex) // Ascending
            {
                displacement = jumpArcData.velocity * simulationTime + simulationTime * simulationTime * 0.5f * new Vector2(0f, MovementConfig.Gravity);
            }
            else if (simulationTime < MovementConfig.TimeTillJumpApex + MovementConfig.ApexHangTime) // Apex hang time
            {
                float apexTime = simulationTime - MovementConfig.TimeTillJumpApex;
                displacement = jumpArcData.velocity * MovementConfig.TimeTillJumpApex + 0.5f * MovementConfig.TimeTillJumpApex * MovementConfig.TimeTillJumpApex * new Vector2(0f, MovementConfig.Gravity);
                displacement += new Vector2(speed, 0) * apexTime; // No vertical movement during hang time
            }
            else // Descending
            {
                float descendTime = simulationTime - (MovementConfig.TimeTillJumpApex + MovementConfig.ApexHangTime);
                displacement = jumpArcData.velocity * MovementConfig.TimeTillJumpApex + 0.5f * MovementConfig.TimeTillJumpApex * MovementConfig.TimeTillJumpApex * new Vector2(0f, MovementConfig.Gravity);
                displacement += new Vector2(speed, 0f) * MovementConfig.ApexHangTime; // Horizontal movement during hang time
                displacement += new Vector2(speed, 0f) * descendTime + 0.5f * descendTime * descendTime * new Vector2(0f, MovementConfig.Gravity);
            }

            drawPoint = jumpArcData.startPosition + displacement;


            float checkDistance = Vector2.Distance(previousPosition, drawPoint);
            Vector2 checkDirection = drawPoint - previousPosition;
            RaycastHit2D check = Physics2D.BoxCast(previousPosition, playerBounds.size, 0, checkDirection, checkDistance, MovementConfig.GroundLayer);
            var isConnectedGround = check.collider == connectedGround;
            var isValidTarget = !isConnectedGround || (isConnectedGround && (Vector2.Distance(check.centroid, playerBounds.center) > playerBounds.size.y));


            if (check.collider != null && isValidTarget)
            {

                Gizmos.color = Color.red;
                Gizmos.DrawLine(check.centroid, playerBounds.center);

                jumpArcData.targetPosition ??= check.centroid;
                Gizmos.color = Color.green;
                Gizmos.DrawSphere(check.centroid, 0.1f);
                Gizmos.DrawSphere(check.point, 0.1f);
                break;
            }
            else
            {
                jumpArcData.targetPosition = null;
            }


            // Draw only if the index is even to create a striped effect
            if (i % 2 == 0)
            {
                Gizmos.color = jumpArcData.color;
                Gizmos.DrawLine(previousPosition, drawPoint);
            }
            previousPosition = drawPoint;
        }

        if (jumpArcData.targetPosition.Value != null)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireCube(jumpArcData.targetPosition.Value, playerBounds.size);
            Gizmos.DrawSphere(jumpArcData.startPosition, .05f);
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
        }
    }
    #endregion
}
