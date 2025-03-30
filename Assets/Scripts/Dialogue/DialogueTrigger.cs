using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class DialogueTrigger : EditorZone<DialogueTrigger>
{
    public TextAsset inkJSON;
    public new readonly bool deactivateOnExit = false;

    private void Deactivate()
    {
        DialogueManagerInk.Instance.OnDialogueEnd -= Deactivate;
        OnDeactivate.Invoke();
    }
    protected override void Start()
    {
        base.Start();
        OnActivate.AddListener(() =>
        {
            DialogueManagerInk.Instance.EnterDialogueMode(inkJSON);
            DialogueManagerInk.Instance.OnDialogueEnd += Deactivate;
        });
    }
}
