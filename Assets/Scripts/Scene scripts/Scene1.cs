using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class Scene1 : MonoBehaviour
{
    public EventManager eventManager;

    public Canvas canvasTutorial;
    private Text tutorialText;

    public CombatAI companimonCombatAI, enimonCombatAI;
    bool attackTutorialShown, blockTutorialShown, ultiTutorialShown;

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(StartScene());

        tutorialText = canvasTutorial.transform.Find("Panel Blackout").Find("Panel").Find("Text").GetComponent<Text>();
        attackTutorialShown = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (!blockTutorialShown)
        {
            if (companimonCombatAI.Health < companimonCombatAI.MaxHealth * 0.9)
            {
                // Pause game and show tutorial
                StartCoroutine(ShowTutorial("Press R when your Companimon is about to get hit by an attack to gain extra Mega Charge!", 1f));
                blockTutorialShown = true;
            }
        }
        if (!attackTutorialShown)
        {
            
            if (enimonCombatAI.Health < enimonCombatAI.MaxHealth * 0.6)
            {
                // Pause game and show tutorial
                StartCoroutine(ShowTutorial("Press E when an attack hits the enemy to do extra damage. A perfectly timed attack will create a critical hit!", 1f));
                attackTutorialShown = true;
            }    
        }
        if (!ultiTutorialShown)
        {
            if (enimonCombatAI.Health < enimonCombatAI.MaxHealth * 0.25)
            {
                // Pause game and show tutorial
                StartCoroutine(ShowTutorial("Press T when your Mega Attack has been fully charged to unleash a powerful special attack!", 0));
                FindObjectOfType<BattleManager>().MegaAttackCharge = 1;
                FindObjectOfType<BattleManager>().MegaAttackSlider.value = 1;

                ultiTutorialShown = true;
            }
        }

        if (Time.timeScale == 0)
            if (Input.anyKeyDown)
            {
                canvasTutorial.gameObject.SetActive(false);
                Time.timeScale = 1;
            }
    }



    private IEnumerator ShowTutorial(string text, float time)
    {
        yield return new WaitForSeconds(time);
        tutorialText.text = text;
        canvasTutorial.gameObject.SetActive(true);
        Time.timeScale = 0;
        yield return null;
    }


    IEnumerator StartScene()
    {
        //FindObjectOfType<Player>().GetComponent<Animator>().Play("Character|PoseLib");
        //Image fadeImage = FindObjectOfType<LevelManager>().fadeImage;
        //yield return new WaitUntil(() => fadeImage.color.a == 0);
        yield return null;
        //eventManager.ExecuteEvent(0);
    }
}
