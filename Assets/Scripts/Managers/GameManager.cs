using System.Collections;
using Manager;
using UnityEngine;

// This class is a singleton that manages the game state

public class GameManager : Manager<GameManager>
{
    private string scenario = null;
    [HideInInspector] public string id;
    [HideInInspector] public string Scenario => scenario;


    [Header("Debug")]
    public string overrideScenarioName = "";
    public bool UseRandomUserId = false; // Set this to true to use a random user ID

    // Start is called before the first frame update
    protected override void Awake()
    {
        id = UseRandomUserId ? "RandomId-" + System.Guid.NewGuid().ToString("N")[..8] : SystemInfo.deviceUniqueIdentifier[..8];
        if (overrideScenarioName != "")
        {
            scenario = overrideScenarioName;
        }
        else
        {
            StartCoroutine(FetchScenario());
        }
        base.Awake();
    }

    private IEnumerator FetchScenario()
    {
        using var webClient = new System.Net.WebClient();
        webClient.Headers[System.Net.HttpRequestHeader.ContentType] = "application/json";
        webClient.Headers[System.Net.HttpRequestHeader.Authorization] = "Bearer ur-game-engineering-2025";
        webClient.Encoding = System.Text.Encoding.UTF8;

        var downloadTask = System.Threading.Tasks.Task.Run(() =>
            webClient.DownloadString("https://ge-log-api.realdegrees.dev/scenario/" + id)
        );

        downloadTask.GetAwaiter().OnCompleted(() =>
        {
            if (downloadTask.IsCompletedSuccessfully)
            {
                scenario = downloadTask.Result;
            }
            else
            {
                scenario = Random.value > 0.5f ? "A" : "B"; // Default scenario if the request fails or times out
            }
        });

        yield return new WaitForSeconds(3f);
        if (downloadTask.IsCompleted)
        {
            yield return null;
        }
        else
        {
            downloadTask.Dispose();
            scenario = Random.value > 0.5f ? "A" : "B"; // Default scenario if the request fails or times out

            yield return null;
        }

    }
}
