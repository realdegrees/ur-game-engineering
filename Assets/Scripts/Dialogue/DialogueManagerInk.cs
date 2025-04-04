using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Ink.Runtime;
using UnityEngine.EventSystems;
using Manager;
using Unity.VisualScripting;
using System;

public class DialogueManagerInk : Manager<DialogueManagerInk>
{
    [SerializeField] private GameObject[] choices;
    [SerializeField] private Image displayPortrait;
    [SerializeField] private Sprite playerPortrait;
    [SerializeField] private Sprite companionPortrait;
    [SerializeField] private GameObject portraitFrame;
    [SerializeField] private GameObject nameFrame;
    [SerializeField] private Button continueBtn;

    private Dictionary<string, Sprite> portraits;
    private TextMeshProUGUI displayNameText;
    private TextMeshProUGUI[] choicesText;
    // private Rigidbody2D rb;
    // private CharacterStateMachine stateMachine;

    public TMPro.TextMeshProUGUI dialogueText;
    public Animator animator;
    public float typingSpeed = 0.02f;
    public string currentDialogueChoice;

    private Story currentStory;
    private bool dialogueIsPlaying;

    public event Action OnDialogueEnd = delegate { };

    private const string SPEAKER_TAG = "speaker";
    private const string PORTRAIT_TAG = "portrait";


    private void Start()
    {
        //rb = GameObject.Find("Player").GetComponent<Rigidbody2D>();
        //stateMachine = GameObject.Find("Player").GetComponent<CharacterStateMachine>();
        dialogueIsPlaying = false;
        displayNameText = nameFrame.transform.Find("DisplayNameText").GetComponent<TextMeshProUGUI>();
        portraits = new Dictionary<string, Sprite>
        {
            { "player", playerPortrait },
            { "companion", companionPortrait }
        };

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

    public void EnterDialogueMode(TextAsset inkJSON)
    {
        // StartCoroutine(FreezePlayer());
        currentStory = new Story(inkJSON.text);
        dialogueIsPlaying = true;
        animator.Play("DialogueIn");

        ContinueStory();
    }

    // private IEnumerator FreezePlayer()
    // {
    //     while (!stateMachine.ground.connected)
    //     {
    //         yield return null;
    //     }
    //     rb.constraints = RigidbodyConstraints2D.FreezePosition;
    // }

    public void ExitDialogueMode()
    {
        if (animator.GetCurrentAnimatorStateInfo(0).IsName("DialogueInvisible"))
        {
            return;
        }
        // rb.constraints = RigidbodyConstraints2D.FreezeRotation;
        dialogueIsPlaying = false;
        animator.Play("DialogueOut");
        dialogueText.text = "";
        OnDialogueEnd.Invoke();
    }

    public void ContinueStory()
    {
        if (currentStory.canContinue)
        {
            dialogueText.text = currentStory.Continue();

            DisplayChoices();
            HandleTags(currentStory.currentTags);
            continueBtn.interactable = currentStory.currentChoices.Count == 0;
        }
        else
        {
            ExitDialogueMode();
        }
    }

    private void HandleTags(List<string> currentTags)
    {
        if (currentTags.Count == 0)
        {
            portraitFrame.SetActive(false);
            nameFrame.SetActive(false);
        }
        else
        {
            portraitFrame.SetActive(true);
            nameFrame.SetActive(true);
            foreach (string tag in currentTags)
            {
                string[] splitTag = tag.Split(':');
                if (splitTag.Length != 2)
                {
                    Debug.LogError("Tag could not be parsed: " + tag);
                }
                string tagKey = splitTag[0].Trim();
                string tagValue = splitTag[1].Trim();

                switch (tagKey)
                {
                    case SPEAKER_TAG:
                        displayNameText.text = tagValue;
                        break;
                    case PORTRAIT_TAG:
                        displayPortrait.sprite = GetPortrait(tagValue);
                        break;
                    default:
                        Debug.LogWarning("Tag came in but is not currently being handled: " + tag);
                        break;
                }
            }
        }
    }

    private Sprite GetPortrait(string tag)
    {
        if (portraits.TryGetValue(tag, out Sprite portrait))
        {
            return portrait;
        }
        else
        {
            Debug.LogWarning("Tag came in but is not currently being handled: " + tag);
            return null;
        }
    }

    private void DisplayChoices()
    {
        List<Choice> currentChoices = currentStory.currentChoices;

        continueBtn.interactable = currentChoices.Count == 0;

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
        LogManager.Instance.LogDialogueChoice(currentStory.currentText, currentStory.currentChoices[choiceIndex].text);
        currentDialogueChoice = currentStory.currentChoices[choiceIndex].text;
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