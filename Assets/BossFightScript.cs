using System.Collections;
using UnityEngine;

public class BossFightScript : MonoBehaviour
{
    public DialogueTrigger entryDialogue;
    public NPCStateMachine bossStateMachine;
    public NPCStateMachine companionStateMachine;
    public NextLevelZone playerRunAwayZone;

    private string dialogueChoice;

    // Start is called before the first frame update
    void Start()
    {
        entryDialogue.OnDeactivate.AddListener(() =>
        {
            dialogueChoice = DialogueManagerInk.Instance.currentDialogueChoice;

            if (dialogueChoice == "ATTACK")
            {
                Debug.Log("Define attack logic here");
                // Case 1: Player chooses to fight
                // bossStateMachine.SetTarget(PlayerController.Instance.transform);
                // companionStateMachine.SetTarget(PlayerController.Instance.transform);
            }
            if (dialogueChoice == "RUN AWAY")
            {
                Debug.Log("Define running logic here");
                // Case 2: Player chooses to run
                // playerRunAwayZone.gameObject.SetActive(true); // Activate the nextlevelzone at the entrance
                // StartCoroutine(MovePlayerToExit()); // Forcefully move the player towards the zone to end the level
            }
        });
    }
    void Update()
    {
        if (bossStateMachine == null && companionStateMachine != null)
        {
            // TODO Logic for when boss dies first (start another dialogue, offering the player a choice between spare, run and fight)
        }
        if (bossStateMachine == null && companionStateMachine == null)
        {
            // TODO Logic for when both die (finish game) 
        }
    }

    IEnumerator MovePlayerToExit()
    {
        // PlayerController.Instance.DisableMovement();
        Rigidbody playerRigidbody = PlayerController.Instance.GetComponent<Rigidbody>();
        Vector3 startPosition = PlayerController.Instance.transform.position;
        Vector3 targetPosition = playerRunAwayZone.transform.position;
        float elapsedTime = 0f;
        float duration = 5f;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            Vector3 direction = (targetPosition - startPosition).normalized;
            float force = Vector3.Distance(startPosition, targetPosition) / duration;
            playerRigidbody.AddForce(direction * force, ForceMode.VelocityChange);
            yield return null;
        }

        PlayerController.Instance.transform.position = targetPosition;
        LevelManager.Instance.NextLevel();
    }
}
