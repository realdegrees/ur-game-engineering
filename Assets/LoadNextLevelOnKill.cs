using System.Collections.Generic;
using UnityEngine;

public class LoadNextLevelOnKill : MonoBehaviour
{
    public List<NPCStats> requiredKills = new();
    private int kills = 0;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        kills = 0;
        foreach (NPCStats npc in requiredKills)
        {
            if (npc == null)
            {
                kills++;
            }
        }
        if (kills == requiredKills.Count)
        {
            LevelManager.Instance.NextLevel();
        }
    }
}
