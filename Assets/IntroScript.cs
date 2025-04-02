using UnityEngine;

public class IntroScript : MonoBehaviour
{
    public DialogueTrigger childDialogue;
    // Start is called before the first frame update
    void Start()
    {

        childDialogue.OnDeactivate.AddListener(() =>
        {
            LevelManager.Instance.NextLevel();
        });
    }

    // Update is called once per frame
    void Update()
    {

    }
}
