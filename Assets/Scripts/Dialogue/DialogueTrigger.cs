using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class DialogueTrigger : EditorZone<DialogueTrigger>
{
    public TextAsset inkJSON;
    public new readonly bool deactivateOnExit = false;

    private void DoDeactivate()
    {
        DialogueManagerInk.Instance.OnDialogueEnd -= DoDeactivate;
        OnDeactivate.Invoke();
    }
    protected override void Start()
    {
        base.Start();
        OnActivate.AddListener((go) =>
        {
            DialogueManagerInk.Instance.EnterDialogueMode(inkJSON);
            DialogueManagerInk.Instance.OnDialogueEnd += DoDeactivate;
        });
    }
}
