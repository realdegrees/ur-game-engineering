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
        OnActivate.AddListener(() =>
        {
            if (target != null)
            {
                var player = PlayerController.Instance.gameObject;
                if (keepXPosition) player.transform.position = new Vector3(target.position.x, player.transform.position.y, player.transform.position.z);
                if (keepYPosition) player.transform.position = new Vector3(player.transform.position.x, target.position.y, player.transform.position.z);
                else player.transform.position = target.position;
            }
        });
    }
    #endregion
}
