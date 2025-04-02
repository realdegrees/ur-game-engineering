using System.Collections;
using UnityEngine;

public class IntroScript : MonoBehaviour
{
    public DialogueTrigger childDialogue;
    // Start is called before the first frame update
    void Start()
    {

        childDialogue.OnDeactivate.AddListener(() =>
        {
            StartCoroutine(NextLevel());
        });
    }

    // Update is called once per frame
    void Update()
    {

    }
    IEnumerator NextLevel()
    {
        yield return new WaitForSeconds(1f);
        LevelManager.Instance.NextLevel();
    }
}
