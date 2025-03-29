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
        userId.text = SystemInfo.deviceUniqueIdentifier[..6];
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
            LevelManager.Instance.NextLevel();
        });
    }
    IEnumerator EnableStartButtonAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        startButton.gameObject.SetActive(true);
    }
}