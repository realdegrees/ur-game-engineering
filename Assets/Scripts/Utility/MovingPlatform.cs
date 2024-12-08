using System.Collections;
using System.Collections.Generic;
using UnityEditor.Callbacks;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class MovingPlatform : MonoBehaviour
{
    public Transform target;
    [Range(.1f, 2)]
    public float speed;
    public AnimationCurve curve = AnimationCurve.Linear(0f, 0f, 1f, 1f);
    public bool debug = false;

    private Vector2 initialPosition;
    private Vector2 targetPosition;
    private Rigidbody2D rb;

    void Awake()
    {
        initialPosition = transform.position;
        targetPosition = target.position;
        TryGetComponent(out rb);
    }

    void FixedUpdate()
    {
        float time = Mathf.PingPong(Time.time * speed, 1f);
        float curveValue = curve.Evaluate(time);
        Vector2 newPosition = Vector2.Lerp(initialPosition, targetPosition, curveValue);
        Vector2 velocity = newPosition - rb.position;
        rb.velocity = velocity;
    }

    void OnDrawGizmos()
    {
        if (!debug) return;

        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(targetPosition, Vector3.one);
        Gizmos.DrawWireCube(initialPosition, Vector3.one);
    }
}
