using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Ink.Runtime;
using UnityEngine.EventSystems;

public class DialogueManagerInk : MonoBehaviour
{
    [SerializeField] private GameObject[] choices;
    private TextMeshProUGUI[] choicesText;

    //public Image characterIcon;
    //public TMPro.TextMeshProUGUI characterName;
    public TMPro.TextMeshProUGUI dialogueText;
    public Animator animator;
    public float typingSpeed = 0.02f;

    private Story currentStory;
    private bool dialogueIsPlaying;

    private static DialogueManagerInk instance;

    private void Awake()
    {
        if (instance != null)
         {
            Debug.Log("DialogueManager already exists in the scene");
         }
         instance = this;
    }

    private void Start()
    {
        dialogueIsPlaying = false;

        choicesText = new TextMeshProUGUI[choices.Length];
        int index = 0;
        foreach (GameObject choice in choices)
        {
            choicesText[index] = choice.GetComponentInChildren<TextMeshProUGUI>();
            index++;
        }
    }

    private void Update()
    {
        if (!dialogueIsPlaying)
        {
            return;
        }
    }

    public static DialogueManagerInk GetInstance()
    {
        return instance;
    }

    public void EnterDialogueMode(TextAsset inkJSON)
    {
        currentStory = new Story(inkJSON.text);
        dialogueIsPlaying = true;
        animator.SetTrigger("Enter");
       // dialoguePanel.SetActive(true);

        ContinueStory();
    }

    private void ExitDialogueMode()
    {
        dialogueIsPlaying = false;
        //dialoguePanel.SetActive(false);
        animator.SetTrigger("Exit");
        dialogueText.text = "";
    }

    public void ContinueStory()
    {
        if (currentStory.canContinue)
        {
            dialogueText.text = currentStory.Continue();
            DisplayChoices();
            //characterIcon.sprite = currentLine.character.icon;
            //characterName.text = currentLine.character.name;
        }
        else
        {
            ExitDialogueMode();
        }
    }

    private void DisplayChoices()
    {
        List<Choice> currentChoices = currentStory.currentChoices;

        if (currentChoices.Count > choices.Length)
        {
            Debug.Log("More choices were given than UI can support: " + currentChoices.Count);
        }

        int index = 0;
        foreach (Choice choice in currentChoices)
        {
            choices[index].gameObject.SetActive(true);
            choicesText[index].text = choice.text;
            index++;
        }

        for (int i = index; i < choices.Length; i++)
        {
            choices[i].gameObject.SetActive(false);
        }

        StartCoroutine(SelectFirstChoice());
    }

    private IEnumerator SelectFirstChoice()
    {
        EventSystem.current.SetSelectedGameObject(null);
        yield return new WaitForEndOfFrame();
        EventSystem.current.SetSelectedGameObject(choices[0].gameObject);
    }

    public void MakeChoice(int choiceIndex)
    {
        currentStory.ChooseChoiceIndex(choiceIndex);
        ContinueStory();
    }

    // IEnumerator TypeSentence(DialogueLine dialogueLine)
    // {
    //     dialogueBody.text = "";
    //     foreach (char letter in dialogueLine.line.ToCharArray())
    //     {
    //         dialogueBody.text += letter; 
    //         yield return new WaitForSeconds(typingSpeed);
    //     }
    // }
}