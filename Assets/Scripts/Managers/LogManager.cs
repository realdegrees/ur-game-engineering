using Manager;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;

// Container for all logging data
[Serializable]
public class LogData
{
    public string id;
    public string scenario;
    public bool foundEasterEgg;
    public string startTime;
    public string finishTime;
    public string operatingSystem;
    public string deviceModel;
    public string processorType;
    public string graphicsCard;
    public int processorCount;
    public int systemMemorySize; // in MB
    public List<LevelData> levels;
    public List<DialogueChoice> dialogueChoices;
}

// Data structure for individual level logs
[Serializable]
public class LevelData
{
    public string levelName;
    public string startTime;
    public string finishTime;
    public float duration; // in seconds
}

// Data structure for dialogue choice logs
[Serializable]
public class DialogueChoice
{
    public string question;
    public string choice;
    public string timestamp;
}

public class LogManager : Manager<LogManager>
{
    private LogData logData;
    [Header("Debug")]
    public bool SendLogsToServerInEditor = false; // Set to true to send logs in the editor
    private bool logSentToServer = false; // Flag to check if the log has been finalized

    protected override void Awake()
    {
        base.Awake();
        // Create log file path (using persistentDataPath ensures it works on all platforms)
        // Initialize log data and record start time for the project
        StartCoroutine(InitLogData());
    }

    IEnumerator InitLogData()
    {
        yield return new WaitUntil(() => GameManager.Instance != null && GameManager.Instance.id != null && GameManager.Instance.Scenario != null);
        logData = new LogData
        {
            id = GameManager.Instance.id,
            scenario = GameManager.Instance.Scenario,
            startTime = DateTime.UtcNow.ToString("o"),
            operatingSystem = SystemInfo.operatingSystem,
            deviceModel = SystemInfo.deviceModel,
            processorType = SystemInfo.processorType,
            processorCount = SystemInfo.processorCount,
            systemMemorySize = SystemInfo.systemMemorySize,
            graphicsCard = SystemInfo.graphicsDeviceName,
            levels = new List<LevelData>(),
            dialogueChoices = new List<DialogueChoice>()
        };
    }

    private void OnApplicationQuit()
    {
        // Finalize the log when the object is destroyed (e.g., when the game ends)
        FinalizeLog();
        base.OnDestroy();
    }

    // Call when a level (scene) starts
    public void LogLevelStart(string levelName)
    {
        StartCoroutine(LogLevelStartCoroutine(levelName));
    }

    IEnumerator LogLevelStartCoroutine(string levelName)
    {
        yield return new WaitUntil(() => logData != null && logData.levels != null);
        // Check if the level already exists in the log
        if (!logData.levels.Exists(l => l.levelName == levelName))
        {
            // Create a new LevelData object with start time
            LevelData level = new()
            {
                levelName = levelName,
                startTime = DateTime.UtcNow.ToString("o")
            };

            // Store it for later update (for simplicity, we add it now)
            logData.levels.Add(level);
            Debug.Log("[LOG] Level Started -> " + levelName);
        }


    }

    // Call when a level ends; assumes the last level started is ending
    public void LogLevelEnd(string levelName)
    {
        // Find the level data entry for the given level name
        LevelData level = logData.levels.Find(l => l.levelName == levelName && string.IsNullOrEmpty(l.finishTime));
        if (level != null)
        {
            level.finishTime = DateTime.UtcNow.ToString("o");

            // Calculate duration based on parsed DateTime values (error handling omitted for brevity)
            DateTime start = DateTime.Parse(level.startTime);
            DateTime finish = DateTime.Parse(level.finishTime);
            level.duration = (float)(finish - start).TotalSeconds;
            Debug.Log("[LOG] Level Ended -> " + levelName);
        }
        else
        {
            Debug.LogWarning("LogLevelEnd called with a level name that wasn't started: " + levelName);
        }
    }

    // Log a dialogue choice with its question and chosen answer
    public void LogDialogueChoice(string question, string choice)
    {
        DialogueChoice dialogue = new()
        {
            question = question,
            choice = choice,
            timestamp = DateTime.UtcNow.ToString("o")
        };
        logData.dialogueChoices.Add(dialogue);
        Debug.Log("[LOG] Dialogue Selected -> " + question + ": " + choice);
    }

    public void LogEasterEgg()
    {
        logData.foundEasterEgg = true;
    }


    private bool UploadLog(string id, string log)
    {
        try
        {
            using var webClient = new System.Net.WebClient();
            webClient.Headers[System.Net.HttpRequestHeader.ContentType] = "application/json";
            webClient.Headers[System.Net.HttpRequestHeader.Authorization] = "Bearer ur-game-engineering-2025";
            webClient.UploadString("https://ge-log-api.realdegrees.dev/log", "POST", log);
            Debug.Log("[LOG] Log uploaded successfully.");
            return true;
        }
        catch (Exception e)
        {
            Debug.LogError("[LOG] Failed to send logfile to the server: " + e.Message);
            return false;
        }
    }

    // Call at the end of the project (or when you want to finalize the log)
    public bool FinalizeLog(bool finished = false)
    {
        logData.finishTime = finished ? DateTime.UtcNow.ToString("o") : "";

        // Serialize the log data to JSON
        string json = JsonUtility.ToJson(logData, true);
        var fullId = GameManager.Instance.Scenario + "-" + GameManager.Instance.id;
        var logFilePath = Path.Combine(Application.persistentDataPath, fullId + ".json");
        File.WriteAllText(logFilePath, json);
        Debug.Log("[LOG] Log file saved to: " + logFilePath);
        if (SendLogsToServerInEditor && !logSentToServer)
        {
            logSentToServer = true; // Prevent multiple uploads

            return UploadLog(fullId, json);
        }

#if !UNITY_EDITOR // ? This is used to force the upload in non-editor mode even if the variable is disabled
        if (!SendLogsToServerInEditor && !logSentToServer)
        {
            logSentToServer = true; // Prevent multiple uploads
            
            return UploadLog(fullId, json);
        }
#endif
        return false;
    }
}
