using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

public class TeleportZone : EditorZone<TeleportZone>
{
    public Transform target;
    public bool keepXPosition = false;
    public bool keepYPosition = false;

    #region Zone Events

    protected override void Awake()
    {
        base.Awake();
        OnActivate.AddListener((go) =>
        {
            if (target != null)
            {
                if (keepXPosition) go.transform.position = new Vector3(target.position.x, go.transform.position.y, go.transform.position.z);
                if (keepYPosition) go.transform.position = new Vector3(go.transform.position.x, target.position.y, go.transform.position.z);
                else go.transform.position = target.position;
            }
        });
    }
    #endregion
}
