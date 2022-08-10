using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LevelManager : MonoBehaviour
{
    public GameObject Player;
    public GameObject Companimon;


    Animator fadeAnimator;
    public Image fadeImage;

    private int entryPoint;

    // Start is called before the first frame update
    void Start()
    {
        DontDestroyOnLoad(this);

        //fadeAnimator = GameObject.Find("Fade Image").GetComponent<Animator>();
        //fadeImage = GameObject.Find("Fade Image").GetComponent<Image>();
    }

    void OnLevelWasLoaded()
    {
        fadeAnimator = GameObject.Find("Fade Image").GetComponent<Animator>();
        fadeImage = GameObject.Find("Fade Image").GetComponent<Image>();

        //fadeAnimator.SetBool("Fade", true);

        GameObject entry = GameObject.Find("Exitentry " + entryPoint);
        //Debug.Log(entry.transform.position);
        Player.transform.position = entry.transform.GetChild(0).transform.position;      
        Companimon.GetComponent<Companimon>().Warp(entry.transform.GetChild(1).transform.position);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SwitchScene(string sceneName, int entryPoint)
    {
        this.entryPoint = entryPoint;

        SceneManager.LoadScene(sceneName);
    }


    public void SwitchScene(string sceneName, float fadeSpeedMultiplier)
    {
        StartCoroutine(SwitchSceneFade(sceneName, fadeSpeedMultiplier));
    }

    private IEnumerator SwitchSceneFade(string sceneName, float speed)
    {
        fadeAnimator.SetBool("Fade", true);
        yield return new WaitUntil(() => fadeImage.color.a == 1);
        SceneManager.LoadScene(sceneName);
    }
}
