using System;
using Pathfinding;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PenaltyHandler : MonoBehaviour
{
    public uint PenaltyFactor = 1000000;
    public float CeilingPenaltyDistance = 8;
    public float CeilingPenaltyWidth = 3;
    public float allowedGapWidth = 4;
    public float softness = 3;
    private readonly uint airPenalty = 150;
    private readonly uint groundPenalty = 0;
    private readonly uint ceilingPenalty = 50;
    public void Scan()
    {
        AstarPath.active.Scan();
        AstarPath.active.AddWorkItem(new AstarWorkItem(ctx =>
                {
                    var gg = AstarPath.active.data.gridGraph;
                    for (int z = 0; z < gg.depth; z++)
                    {
                        for (int x = 0; x < gg.width; x++)
                        {
                            var node = gg.GetNode(x, z);
                            var pos = (Vector2)(Vector3)node.position;
                            var upHitDistance = gg.nodeSize * CeilingPenaltyDistance;
                            var downHitDistance = upHitDistance / 2f;

                            Collider2D self = Physics2D.OverlapPoint(pos, LayerMask.GetMask("Ground"));

                            if (self != null)
                            {
                                node.Walkable = false;
                                continue;
                            }

                            RaycastHit2D upHit = Physics2D.Raycast(pos, Vector2.up, upHitDistance, LayerMask.GetMask("Ground"));
                            RaycastHit2D downHit = Physics2D.Raycast(pos, Vector2.down, downHitDistance, LayerMask.GetMask("Ground"));
                            RaycastHit2D leftHit = Physics2D.Raycast(pos, Vector2.left, gg.nodeSize * allowedGapWidth, LayerMask.GetMask("Ground"));
                            RaycastHit2D rightHit = Physics2D.Raycast(pos, Vector2.right, gg.nodeSize * allowedGapWidth, LayerMask.GetMask("Ground"));


                            if (upHit.collider != null)
                            {
                                var normalizedHitDistance = Mathf.Clamp01(Vector2.Distance(upHit.point, pos) / upHitDistance);
                                node.Penalty = (uint)Mathf.RoundToInt(Mathf.Lerp(ceilingPenalty, airPenalty, normalizedHitDistance));
                            }
                            if (downHit.collider != null) // Overrides upHit Penalty                      
                            {
                                var normalizedHitDistance = Mathf.Clamp01(Vector2.Distance(downHit.point, pos) / downHitDistance);
                                node.Penalty = (uint)Mathf.RoundToInt(Mathf.Lerp(groundPenalty, airPenalty, normalizedHitDistance));
                            }

                            if (!downHit.collider && !upHit.collider)
                            {
                                node.Penalty = airPenalty;
                            }
                            if (leftHit.collider != null && rightHit.collider != null)
                            {
                                var distance = Vector2.Distance(leftHit.point, rightHit.point);
                                if (distance < allowedGapWidth)
                                {
                                    node.Penalty = (uint)Mathf.RoundToInt(airPenalty * 2f);
                                }
                            }
                            if (leftHit.collider && !rightHit.collider)
                            {
                                var hitDistance = Vector2.Distance(leftHit.point, pos);
                                if (hitDistance <= gg.nodeSize)
                                {
                                    node.Walkable = false;
                                }
                            }
                            if (rightHit.collider && !leftHit.collider)
                            {
                                var hitDistance = Vector2.Distance(rightHit.point, pos);
                                if (hitDistance <= gg.nodeSize)
                                {
                                    node.Walkable = false;
                                }
                            }
                            node.Penalty *= PenaltyFactor;
                        }
                    }


                    // Apply penalty multiplier based on surrounding nodes within a specified radius
                    float radius = 1.5f; // Example radius, you can adjust this value
                    for (int z = 0; z < gg.depth; z++)
                    {
                        for (int x = 0; x < gg.width; x++)
                        {
                            var node = gg.GetNode(x, z);
                            if (node.Penalty == 0) continue;
                            if (node.Walkable == false) continue;

                            int surroundingPenalty = 0;
                            int surroundingCount = 0;

                            // Check surrounding nodes within the radius
                            for (int dz = -Mathf.CeilToInt(radius); dz <= Mathf.CeilToInt(radius); dz++)
                            {
                                for (int dx = -Mathf.CeilToInt(radius); dx <= Mathf.CeilToInt(radius); dx++)
                                {
                                    if (dz == 0 && dx == 0) continue;

                                    int nx = x + dx;
                                    int nz = z + dz;

                                    if (nx >= 0 && nx < gg.width && nz >= 0 && nz < gg.depth)
                                    {
                                        var neighbor = gg.GetNode(nx, nz);
                                        float distance = Vector2.Distance(new Vector2(x, z), new Vector2(nx, nz));
                                        if (distance <= radius)
                                        {
                                            surroundingPenalty += (int)neighbor.Penalty;
                                            surroundingCount++;
                                        }
                                    }
                                }
                            }

                            if (surroundingCount > 0)
                            {
                                int averageSurroundingPenalty = surroundingPenalty / surroundingCount;
                                node.Penalty += (uint)averageSurroundingPenalty; // Apply a multiplier based on surrounding penalties
                            }
                        }
                    }

                    gg.GetNodes(node => gg.CalculateConnections((GridNodeBase)node));
                }));
    }
    void Start()
    {
        Scan();
        SceneManager.sceneLoaded += (a, b) =>
        {
            Scan();
        };
    }
}
