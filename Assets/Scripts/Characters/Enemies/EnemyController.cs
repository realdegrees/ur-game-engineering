using System;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    [HideInInspector]
    public EnemyStateMachine stateMachine;

    public event Action OnFlip = delegate { };

    public bool IsFacingRight { get; private set; } = true;
    private Vector2 pathDir;


    private void Start()
    {
        stateMachine = GetComponent<EnemyStateMachine>();
    }
    private void Update()
    {
        if (stateMachine.path != null) FlipCheck();
    }
    private void FixedUpdate()
    {
        pathDir = Vector2.zero;
        if (stateMachine.path != null && stateMachine.currentWaypoint < stateMachine.path.vectorPath.Count)
        {
            pathDir = ((Vector2)stateMachine.path.vectorPath[stateMachine.currentWaypoint] - stateMachine.rb.position).normalized;
        }
        HandleGravity();
        HandleMovement();
        HandleJump();
        HandleFall();
    }


    private void HandleGravity()
    {
        if (!stateMachine.ground.connected)
        {
            stateMachine.rb.AddForce(Physics2D.gravity * stateMachine.config.GravityMultiplier);
        }
    }

    private void FlipCheck()
    {
        if (pathDir.x < 0 && IsFacingRight)
        {
            IsFacingRight = false;
            transform.rotation = Quaternion.Euler(0f, 0, 0f);
            OnFlip.Invoke();
        }
        else if (pathDir.x > 0 && !IsFacingRight)
        {
            IsFacingRight = true;
            transform.rotation = Quaternion.Euler(0f, 180f, 0f);
            OnFlip.Invoke();
        }
    }

    private void HandleMovement()
    {
        if (stateMachine.path != null)
        {
            if (stateMachine.IsStateActive(EEnemyState.Moving, EEnemyState.Accelerating))
                return;

            stateMachine.EnterState(EEnemyState.Accelerating);
        }
        else if (stateMachine.IsStateActive(EEnemyState.Moving, EEnemyState.Accelerating))
        {
            stateMachine.EnterState(EEnemyState.Decelerating);
        }


    }
    private void HandleJump()
    {
        if (!stateMachine.ground.connected || stateMachine.path == null)
            return;

        Rigidbody2D rb = stateMachine.rb;
        EnemyMovementConfig config = stateMachine.config;
        var rayOrigin = new Vector2(rb.position.x + stateMachine.bodyCollider.bounds.size.x * 1.5f * Mathf.Sign(rb.velocity.x), stateMachine.bodyCollider.bounds.min.y);

        RaycastHit2D ledgeHit = Physics2D.Raycast(
            rayOrigin,
            -rb.transform.up,
            config.LedgeDepthThreshold,
            config.GroundLayer
        );

        var pathAngle = Vector2.Angle(Vector2.up, pathDir);

        bool isLedge = ledgeHit.collider == null;
        bool isWall = ledgeHit.point.y > stateMachine.ground.hit.point.y;
        bool wantsToGoDown = pathDir.y < 0;
        if (!wantsToGoDown && !stateMachine.IsStateActive(EEnemyState.Jumping, EEnemyState.Falling) && (isLedge || isWall || pathAngle < 10))
        {
            stateMachine.EnterState(EEnemyState.Jumping);
        }

    }
    private void HandleFall()
    {
        // TODO: might have to replace with or add a check for the fall state being active
        if (stateMachine.rb.velocity.y < 0 && !stateMachine.ground.connected && !stateMachine.IsStateActive(EEnemyState.Jumping, EEnemyState.Falling))
        {
            stateMachine.EnterState(EEnemyState.Falling);
        }
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.transform.root.CompareTag("Player"))
        {
            stateMachine.target = collision.transform.root;
        }
    }



    // private Seeker seeker;
    // private Path path;
    // private Rigidbody2D rb;

    // public Collider2D footCollider;
    // public float speed = 5f;
    // public float nextWaypointDistance = 3f;
    // public float jumpForce = 30f;
    // private int currentWaypoint = 0;
    // public float jumpDetectionDistance = 1f;

    // public float pathUpdateDelta = 2f;
    // private Transform target;


    // // Start is called before the first frame update
    // void Start()
    // {
    //     seeker = GetComponent<Seeker>();
    //     rb = GetComponent<Rigidbody2D>();

    //     StartCoroutine(UpdatePath());
    // }

    // // Update is called once per frame
    // void FixedUpdate()
    // {
    //     if (path != null)
    //         Move();
    //     else
    //     {
    //         // decelerate horizontal
    //         rb.velocity = new Vector2(rb.velocity.x * .9f, rb.velocity.y);
    //     }
    // }

    // private void HandleJump(Vector2 horizontalDirection)
    // {
    //     Vector2 source = footCollider.transform.position;
    //     RaycastHit2D wallHit = Physics2D.Raycast(source, horizontalDirection, jumpDetectionDistance, LayerMask.GetMask("Ground"));
    //     RaycastHit2D holeHit = Physics2D.Raycast(source + horizontalDirection * jumpDetectionDistance, Vector2.down, jumpDetectionDistance, LayerMask.GetMask("Ground"));

    //     Debug.DrawRay(source, horizontalDirection * jumpDetectionDistance, Color.red);
    //     Debug.DrawRay(source + horizontalDirection * jumpDetectionDistance, Vector2.down * jumpDetectionDistance, Color.red);

    //     if (wallHit.collider != null || holeHit.collider == null)
    //     {
    //         rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
    //     }
    // }

    // private void Move()
    // {
    //     if (currentWaypoint >= path.vectorPath.Count)
    //     {
    //         path = null;
    //         return;
    //     }

    //     Vector2 direction = ((Vector2)path.vectorPath[currentWaypoint] - rb.position).normalized;
    //     Vector2 horizontalDirection = new Vector2(direction.x, 0).normalized;
    //     Vector2 horizontalVelocity = new(rb.velocity.x, 0);
    //     Vector2 force = (1 + Vector2.Distance(horizontalVelocity, horizontalDirection)) * speed * Time.deltaTime * direction;

    //     // HandleJump(horizontalDirection);
    //     rb.AddForce(force);

    //     float distance = Vector2.Distance(rb.position, path.vectorPath[currentWaypoint]);
    //     if (distance < nextWaypointDistance)
    //     {
    //         currentWaypoint++;
    //     }
    // }

    // void OnTriggerEnter2D(Collider2D collision)
    // {
    //     if (collision.transform.root.CompareTag("Player"))
    //     {
    //         target = collision.transform.root;
    //     }
    // }

    // IEnumerator UpdatePath()
    // {
    //     while (true)
    //     {
    //         if (seeker.IsDone())
    //         {
    //             Debug.Log("Calulating new path");
    //             seeker.StartPath(rb.position, target.position, p =>
    //             {
    //                 if (!p.error)
    //                 {
    //                     path = p;
    //                     currentWaypoint = 0;
    //                 }
    //             });
    //         }
    //         yield return new WaitForSeconds(pathUpdateDelta);
    //     }
    // }
}
