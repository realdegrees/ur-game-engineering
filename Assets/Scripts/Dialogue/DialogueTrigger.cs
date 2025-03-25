using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class DialogueTrigger : EditorZone<DialogueTrigger>
{
    public TextAsset inkJSON;

    protected override void Start()
    {
        base.Start();
        OnActivate.AddListener(() => DialogueManagerInk.Instance.EnterDialogueMode(inkJSON));
        OnDeactivate.AddListener(() => DialogueManagerInk.Instance.ExitDialogueMode());
    }
}
