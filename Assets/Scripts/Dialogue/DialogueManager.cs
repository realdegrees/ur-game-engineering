using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DialogueManager : MonoBehaviour
{

    public static DialogueManager Instance;

    public Image characterIcon;
    public TMPro.TextMeshProUGUI characterName;
    public TMPro.TextMeshProUGUI dialogueBody;
    public Animator animator;

    public bool isDialogueActive = false;
    public float typingSpeed = 0.05f;

    private Queue<DialogueLine> lines = new Queue<DialogueLine>();

    void Start()
    {
        if (Instance == null) {
            Instance = this;
        }
    }

    public void StartDialogue(Dialogue dialogue)
    {
        isDialogueActive = true;
        animator.SetTrigger("Enter");
        lines.Clear();

        foreach (DialogueLine dialogueLine in dialogue.dialogueLines)
        {
            lines.Enqueue(dialogueLine);
        }
        DisplayNextDialogueLine();
    }

    public void DisplayNextDialogueLine()
    {
        if (lines.Count == 0)
        {
            EndDialogue();
            return;
        }
        DialogueLine currentLine = lines.Dequeue();
        characterIcon.sprite = currentLine.character.icon;
        characterName.text = currentLine.character.name;
        StopAllCoroutines();
        StartCoroutine(TypeSentence(currentLine));
    }

    IEnumerator TypeSentence(DialogueLine dialogueLine)
    {
        dialogueBody.text = "";
        foreach (char letter in dialogueLine.line.ToCharArray())
        {
            dialogueBody.text += letter; 
            yield return new WaitForSeconds(typingSpeed);
        }
    }

    void EndDialogue()
    {
        isDialogueActive = false;
        animator.SetTrigger("Exit");
    }
}
