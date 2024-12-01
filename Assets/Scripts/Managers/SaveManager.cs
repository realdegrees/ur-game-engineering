using UnityEngine;
using Manager;

// This class is a singleton that manages the save state

public class SaveManager : Manager<SaveManager>
{
    protected override void Init()
    {
        Debug.Log("SaveManager Awake");
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
