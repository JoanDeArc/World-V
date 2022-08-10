using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Zoneswitch : MonoBehaviour
{
    public string ToScene;
    public int ToEntryPoint;
    private LevelManager levelManager;

    public void Start()
    {
        levelManager = FindObjectOfType<LevelManager>();
    }

    private void OnTriggerEnter(Collider other)
    {
        levelManager.SwitchScene(ToScene, ToEntryPoint);
    }
}
