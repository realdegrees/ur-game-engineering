using Pathfinding;
using UnityEngine;

public class PenaltyHandler : MonoBehaviour
{
    public uint PenaltyFactor = 1000000;
    public float CeilingPenaltyDistance = 8;
    public float CeilingPenaltyWidth = 3;
    public float allowedGapWidth = 10;
    public float gapClosingOffset = 3;
    private readonly uint airPenalty = 190;
    private readonly uint groundPenalty = 15;
    private readonly uint ceilingPenalty = 210;
    public void Scan()
    {
        AstarPath.active.AddWorkItem(new AstarWorkItem(ctx =>
                {
                    var gg = AstarPath.active.data.gridGraph;
                    for (int z = 0; z < gg.depth; z++)
                    {
                        for (int x = 0; x < gg.width; x++)
                        {
                            var node = gg.GetNode(x, z);
                            var pos = (Vector2)(Vector3)node.position;
                            var upCheckBox = CeilingPenaltyWidth * gg.nodeSize * Vector2.one;
                            var downCheckBox = 0.5f * gg.nodeSize * Vector2.one;
                            var upHitDistance = gg.nodeSize * CeilingPenaltyDistance;
                            var downHitDistance = upHitDistance / 1.5f;

                            Collider2D self = Physics2D.OverlapBox(pos, gg.nodeSize * Vector2.one, 0, LayerMask.GetMask("Ground"));

                            if (self != null)
                            {
                                node.Walkable = false;
                            }

                            RaycastHit2D upHit = Physics2D.BoxCast(pos + new Vector2(0, upCheckBox.y / 2), upCheckBox, 0, Vector2.up, upHitDistance, LayerMask.GetMask("Ground"));
                            RaycastHit2D downHit = Physics2D.BoxCast(pos, downCheckBox, 0, Vector2.down, downHitDistance, LayerMask.GetMask("Ground"));
                            RaycastHit2D leftHit = Physics2D.Raycast(pos - new Vector2(0, gapClosingOffset * gg.nodeSize), Vector2.left, allowedGapWidth, LayerMask.GetMask("Ground"));
                            RaycastHit2D rightHit = Physics2D.Raycast(pos - new Vector2(0, gapClosingOffset * gg.nodeSize), Vector2.right, allowedGapWidth, LayerMask.GetMask("Ground"));

                            if (upHit.collider != null)
                            {
                                node.Penalty = ceilingPenalty;
                            }
                            if (downHit.collider != null) // Overrides upHit Penalty                      
                            {
                                var normalizedHitDistance = Mathf.Clamp01(Vector2.Distance(downHit.point, pos) / downHitDistance);
                                node.Penalty = groundPenalty - (uint)(groundPenalty * (1 - normalizedHitDistance));
                                node.Walkable = true;
                            }

                            if (upHit.collider == null && downHit.collider == null)
                            {
                                var isOverGap = leftHit.collider != null && rightHit.collider != null;
                                node.Penalty = isOverGap ? groundPenalty : airPenalty;
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
    }
}
