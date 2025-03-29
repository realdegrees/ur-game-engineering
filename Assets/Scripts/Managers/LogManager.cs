using Manager;
using System;
using System.Collections.Generic;
using System.IO;
using Unity.VisualScripting;
using UnityEngine;

// Container for all logging data
[Serializable]
public class LogData
{
    public string startTime;
    public string finishTime;
    public string operatingSystem;
    public string deviceModel;
    public string processorType;
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
    private string logFilePath;
    private LogData logData;

    public void Start()
    {
        // Create log file path (using persistentDataPath ensures it works on all platforms)
        logFilePath = Path.Combine(Application.persistentDataPath, "logfile.json");

        // Initialize log data and record start time for the project
        logData = new LogData
        {
            startTime = DateTime.UtcNow.ToString("o"),
            operatingSystem = SystemInfo.operatingSystem,
            deviceModel = SystemInfo.deviceModel,
            processorType = SystemInfo.processorType,
            processorCount = SystemInfo.processorCount,
            systemMemorySize = SystemInfo.systemMemorySize,
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
        // Create a new LevelData object with start time
        LevelData level = new()
        {
            levelName = levelName,
            startTime = DateTime.UtcNow.ToString("o")
        };

        // Store it for later update (for simplicity, we add it now)
        logData.levels.Add(level);
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
    }

    // Call at the end of the project (or when you want to finalize the log)
    public void FinalizeLog()
    {
        logData.finishTime = DateTime.UtcNow.ToString("o");

        // Serialize the log data to JSON
        string json = JsonUtility.ToJson(logData, true);

#if !UNITY_EDITOR
        // Write the JSON string to the log file
        try
        {
            File.WriteAllText(logFilePath, json);
            Debug.Log("Log file written to: " + logFilePath);

            var id = SystemInfo.deviceUniqueIdentifier[..6];

            using var webClient = new System.Net.WebClient();
            webClient.Headers[System.Net.HttpRequestHeader.ContentType] = "application/json";
            webClient.Headers[System.Net.HttpRequestHeader.Authorization] = "Bearer ur-game-engineering-2025";
            webClient.UploadString("https://ge-log-api.realdegrees.dev/" + id, "POST", json);
            Debug.Log("Log file successfully posted to the server.");
        }
        catch (Exception e)
        {
            Debug.LogError("Failed to write log file: " + e.Message);
        }
#endif
    }
}
