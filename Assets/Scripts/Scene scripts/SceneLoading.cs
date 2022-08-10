using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneLoading : MonoBehaviour
{
    public string ToScene;
    public int ToEntryPoint;
    LevelManager levelManager;

    // Start is called before the first frame update
    void Start()
    {
        levelManager = FindObjectOfType<LevelManager>();

        levelManager.SwitchScene(ToScene, ToEntryPoint);
    }

    // Update is called once per frame
    void Update()
    {
    }
}
