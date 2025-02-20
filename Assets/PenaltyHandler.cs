using Pathfinding;
using UnityEngine;

public class PenaltyUpdater : MonoBehaviour
{
    // The extra cost for nodes that are unsupported
    public int unsupportedPenalty = 100;

    void Start()
    {
        AstarPath.active.AddWorkItem(new AstarWorkItem(ctx =>
        {
            var gg = AstarPath.active.data.gridGraph;
            for (int z = 0; z < gg.depth; z++)
            {
                for (int x = 0; x < gg.width; x++)
                {
                    var node = gg.GetNode(x, z);
                    RaycastHit2D hit = Physics2D.CircleCast((Vector3)node.position, gg.nodeSize, Vector2.down, gg.nodeSize * 1.1f, LayerMask.GetMask("Ground"));
                    if (hit.collider == null)
                    {
                        Debug.Log("Air node at " + x + ", " + z + " penalized");
                        node.Penalty = 100000;
                    }
                }
            }

            gg.GetNodes(node => gg.CalculateConnections((GridNodeBase)node));
        }));
    }
}
