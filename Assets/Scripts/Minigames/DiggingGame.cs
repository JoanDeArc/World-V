using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DiggingGame : TrainingGame
{
    private float timer;
    private KeyCode lastKeyCode;
    private bool penaltyOn;
    private float penaltyAmount;
    private float timerPenalty;

    private float lowerAmount;

    private int thresholdLevel;
    private float[] safeThresholds;

    private bool animateThreshold;

    public Image ImageProgress;
    public Image ImagePenalty;
    public Image ImageSafeAnimation;
    public Image ImageSafe;

    public Text TextCountdown;

    public override string InfoString { get => "Alternate pressing the buttons as fast as you can."; }

    // Start is called before the first frame update
    void Start()
    {
    }

    public override IEnumerator StartGame(int difficultyLevel)
    {
        FindObjectOfType<Player>().MovementDisabled = true;
        this.difficultyLevel = difficultyLevel;
        GameCam.Priority = 11;

        yield return new WaitForSeconds(FindObjectOfType<CinemachineBrain>().GetComponent<CinemachineBrain>().m_DefaultBlend.m_Time);

        ImageProgress.fillAmount = 0;
        ImageSafeAnimation.fillAmount = 0;
        ImageSafe.fillAmount = 0;

        GameCanvas.gameObject.SetActive(true);

        thresholdLevel = 0;
        timer = 10;
        TextCountdown.text = timer.ToString("0.00") + "s";

        switch (difficultyLevel)
        {
            case 0:
                lowerAmount = 0.0001f;
                penaltyAmount = 0.2f;
                safeThresholds = new float[3] { 0.25f, 0.5f, 0.75f };
                break;
            case 1:
                lowerAmount = 0.0003f;
                penaltyAmount = 0.4f;
                safeThresholds = new float[1] { 0.5f };
                break;
            case 2:
                lowerAmount = 0.0007f;
                penaltyAmount = 0.5f;
                break;
        }

        PromptCanvas.GetComponentInChildren<Text>().text = "Dig quick!";
        PromptCanvas.gameObject.SetActive(true);
        yield return new WaitForSeconds(1);
        PromptCanvas.gameObject.SetActive(false);

        isPlaying = true;
    }

    public override IEnumerator EndGame()
    {
        isPlaying = false;

        ImageSafe.fillAmount = 1;
        animateThreshold = true;

        yield return new WaitForSeconds(2);

        GameCam.Priority = 0;
        FindObjectOfType<Player>().MovementDisabled = false;

        if (ImageProgress.fillAmount > 0.99f)
            Debug.Log("Game end - win");
        else
            Debug.Log("Game end - lose");

        GameCanvas.gameObject.SetActive(false);
        gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (isPlaying)
        {
            if (penaltyOn)
            {
                timerPenalty -= Time.deltaTime;
                if (timerPenalty <= 0)
                {
                    penaltyOn = false;
                    ImagePenalty.gameObject.SetActive(false);
                }
            }
            else
            {
                if (Input.GetKeyDown(KeyCode.Alpha1))
                    CheckInput(KeyCode.Alpha1);
                else if (Input.GetKeyDown(KeyCode.Alpha2))
                    CheckInput(KeyCode.Alpha2);
            }

            timer -= Time.deltaTime;
            TextCountdown.text = timer.ToString("0.00") + "s";
            if (timer <= 0)
            {
                timer = 0;
                StartCoroutine(EndGame());
            }

            LowerProgress();
        }
        if (animateThreshold)
            AnimateThreshold();
    }

    private void CheckInput(KeyCode keyCode)
    {
        if (keyCode == lastKeyCode)
        {
            penaltyOn = true;
            timerPenalty = penaltyAmount;
            ImagePenalty.gameObject.SetActive(true);
            lastKeyCode = KeyCode.None;
            return;
        }

        IncreaseProgress();

        lastKeyCode = keyCode;
    }

    private void IncreaseProgress()
    {
        ImageProgress.fillAmount += 0.02f;

        if (safeThresholds.Length > thresholdLevel)
        {
            if (ImageProgress.fillAmount >= safeThresholds[thresholdLevel])
            {
                ImageSafe.fillAmount = safeThresholds[thresholdLevel];
                thresholdLevel++;
                animateThreshold = true;
            }
        }

        if (ImageProgress.fillAmount == 1)
            StartCoroutine(EndGame());
    }

    private void LowerProgress()
    {
        if (ImageProgress.fillAmount > ImageSafe.fillAmount)
            ImageProgress.fillAmount -= lowerAmount;
    }

    private void AnimateThreshold()
    {
        if (ImageSafeAnimation.fillAmount < ImageSafe.fillAmount)
            ImageSafeAnimation.fillAmount += 0.005f;
        else
        {
            ImageSafeAnimation.fillAmount = ImageSafe.fillAmount;
            animateThreshold = false;
        }
    }
}
