using Manager;
using UnityEngine;

// This class is a singleton that manages the game state

public class GameManager : Manager<GameManager>
{
    protected override void Init()
    {
        Debug.Log("GameManager Awake");
    }
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
}
