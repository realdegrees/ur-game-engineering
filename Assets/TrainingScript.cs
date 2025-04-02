using System.Collections;
using UnityEngine;

public class TrainingScript : MonoBehaviour
{
    private string scenario;
    public GameObject companion;
    public GameObject companionSciptedTarget;
    public DialogueTrigger onBatKilledDialogue;
    public DialogueTrigger onAfterCompanionCryDialogue;
    public DialogueTrigger batDialogue;
    // Start is called before the first frame update
    void Start()
    {
        scenario = GameManager.Instance.Scenario;
        onBatKilledDialogue.OnDeactivate.AddListener(() =>
        {
            LevelManager.Instance.NextLevel();
        });
        onAfterCompanionCryDialogue.OnDeactivate.AddListener(() =>
        {
            LevelManager.Instance.NextLevel();
        });
        batDialogue.OnDeactivate.AddListener(() =>
        {
            var sm = companion.GetComponent<NPCStateMachine>();
            sm.SetTarget(companionSciptedTarget.transform);
            if (scenario == "B")
            {
                StartCoroutine(NextLevel());
            }
        });
    }

    IEnumerator NextLevel()
    {
        yield return new WaitForSeconds(3f);
        onAfterCompanionCryDialogue.OnActivate.Invoke(companion);
    }

    // Update is called once per frame
    void Update()
    {
        if (companionSciptedTarget == null)
        {
            enabled = false;
            onBatKilledDialogue.OnActivate.Invoke(companion);
        }
    }
}
