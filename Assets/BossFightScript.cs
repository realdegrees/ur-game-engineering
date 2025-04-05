using System.Collections;
using UnityEngine;

public class BossFightScript : MonoBehaviour
{
    public DialogueTrigger entryDialogue;
    public DialogueTrigger onKingKilledDialogue;

    public GameObject boss;
    public GameObject companion;

    private NPCStateMachine bossStateMachine;
    private NPCStateMachine companionStateMachine;
    private NPCStats bossStats;
    private NPCStats companionStats;

    public CharacterStateMachine playerStateMachine;
    public NextLevelZone playerRunAwayZone;

    private string dialogueChoice;

    // Start is called before the first frame update
    void Start()
    {
        bossStateMachine = boss.GetComponent<NPCStateMachine>();
        companionStateMachine = companion.GetComponent<NPCStateMachine>();
        bossStats = boss.GetComponent<NPCStats>();
        companionStats = companion.GetComponent<NPCStats>();

        entryDialogue.OnDeactivate.AddListener(() =>
        {
            dialogueChoice = DialogueManagerInk.Instance.currentDialogueChoice;

            Debug.Log("Dialogue choice: " + dialogueChoice.ToString());
            if (dialogueChoice == "ATTACK")
            {
                Debug.Log("Define attack logic here");
                // Case 1: Player chooses to fight
                bossStateMachine.SetTarget(playerStateMachine.transform);
                companionStateMachine.SetTarget(playerStateMachine.transform);
            }
            if (dialogueChoice == "RUN AWAY" || dialogueChoice == "SPARE") // TODO spare is just a placeholder
            {
                LevelManager.Instance.NextLevel();
            }
        });

        onKingKilledDialogue.OnDeactivate.AddListener(() =>
        {
            dialogueChoice = DialogueManagerInk.Instance.currentDialogueChoice;
            if (dialogueChoice == "ATTACK")
            {
                Debug.Log("Define attack logic here");
                // Case 1: Player chooses to fight
                bossStateMachine.SetTarget(playerStateMachine.transform);
                companionStateMachine.SetTarget(playerStateMachine.transform);
            }
            if (dialogueChoice == "RUN AWAY" || dialogueChoice == "SPARE") // TODO spare is just a placeholder
            {
                LevelManager.Instance.NextLevel();
            }
        });
    }
    void Update()
    {
        if (bossStats.GetHealth() <= 0 && companionStats.GetHealth() > 0)
        {
            onKingKilledDialogue.OnActivate.Invoke(playerStateMachine.gameObject);
        }
        if (bossStateMachine == null && companionStateMachine == null)
        {
            LevelManager.Instance.NextLevel();
        }
    }
}
