using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PostGameMenu : MonoBehaviour
{
    public Button questionnaireButton;
    public Button startButton;
    public Button userIdCopyButton;
    public TextMeshProUGUI userId;
    public TextMeshProUGUI logStatusText;

    private void Start()
    {
        UIManager.Instance.Disable();
        userId.text = GameManager.Instance.id.ToString();
        StartCoroutine(SetUserIdText());

        bool isLogSent = LogManager.Instance.FinalizeLog(true);
        logStatusText.text = isLogSent ? "Logs sent to the server successfully." : "Failed to send the logs to the server, please contact us so we can retrieve the logs from the local files with your help. Please still fill out the questionnaire below first.";
        logStatusText.color = isLogSent ? Color.green : Color.red;

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
            var link = "https://docs.google.com/forms/d/e/1FAIpQLSel45g3T_z8MNDJUIf_HXS6aggP8U7hX08ZRLdsoZJFZdR3_g/viewform";
            TextEditor textEditor = new()
            {
                text = link
            };
            textEditor.SelectAll();
            textEditor.Copy();
            Application.OpenURL(link);
            StartCoroutine(EnableStartButtonAfterDelay(4f));
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