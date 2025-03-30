using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Menu : MonoBehaviour
{
    public Button questionnaireButton;
    public Button startButton;
    public Button userIdCopyButton;
    public TextMeshProUGUI userId;

    private void Start()
    {
        UIManager.Instance.Disable();
        userId.text = "Generating ID...";
        StartCoroutine(SetUserIdText());

        userIdCopyButton.onClick.AddListener(() =>
        {
            TextEditor textEditor = new()
            {
                text = userId.text
            };
            textEditor.SelectAll();
            textEditor.Copy();
        });

        questionnaireButton.onClick.AddListener(() =>
        {
            var link = "https://docs.google.com/forms/d/e/1FAIpQLScWHVgTQgy8hpYqxSTQT-GOtWGT8U4Ox-9NBmQFKJpNuyzIHA";
            TextEditor textEditor = new()
            {
                text = link
            };
            textEditor.SelectAll();
            textEditor.Copy();
            Application.OpenURL(link);
            StartCoroutine(EnableStartButtonAfterDelay(5f));
        });

        startButton.onClick.AddListener(() =>
        {
            UIManager.Instance.Enable();
            LevelManager.Instance.NextLevel();
        });
    }
    IEnumerator SetUserIdText()
    {
        yield return new WaitUntil(() => GameManager.Instance && GameManager.Instance.Scenario != null);
        userId.text = GameManager.Instance.Scenario + "-" + GameManager.Instance.id;
    }
    IEnumerator EnableStartButtonAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        yield return new WaitUntil(() => GameManager.Instance && GameManager.Instance.Scenario != null);
        startButton.gameObject.SetActive(true);
    }
}