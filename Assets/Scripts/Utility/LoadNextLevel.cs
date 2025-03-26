using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadNextLevel : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        LevelManager.Instance.NextLevel();
    }

    // Update is called once per frame
    void Update()
    {

    }
}
