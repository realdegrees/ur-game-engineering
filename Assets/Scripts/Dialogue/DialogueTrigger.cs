using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueTrigger : MonoBehaviour
{
    [SerializeField] private TextAsset inkJSON;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.transform.root.CompareTag("Player"))
        {
            Debug.Log("Entered Dialogue Zone");
            DialogueManagerInk.GetInstance().EnterDialogueMode(inkJSON);
        }
    }

}
