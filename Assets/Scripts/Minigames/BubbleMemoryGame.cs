using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BubbleMemoryGame : TrainingGame
{
    //private bool isPlaying;
    //private int difficultyLevel; // 0 = easy, 1 = normal, 2 = hard
    private int sequenceLength;
    private bool showingSequence;
    private bool enteringSequence;

    public ParticleSystem ParticleSystem;
    public Texture[] BubbleImages = new Texture[4];
    public AudioClip[] SoundEffects = new AudioClip[4];
    private AudioSource audioSource;

    //public Transform GameCanvas;
    //public Transform PromptCanvas;

    private float countdownTimer;
    private int sequenceIndex;
    private int[] sequenceIndices;
    private float[] sequenceIntervals;

    private Transform sequenceImages;
    private Transform inputImages;

    private float inputTimestamp;
    private int inputCounter;
    private int[] playerIndices;
    private float[] playerIntervals;

    public override string InfoString { get => "Remember the pattern and repeat it."; }


    //private CinemachineVirtualCamera cam;

    // Start is called before the first frame update
    void Start()
    {
        audioSource = GetComponentInChildren<AudioSource>();

        sequenceImages = GameCanvas.GetChild(0);
        inputImages = GameCanvas.GetChild(1);
    }

    private void RandomizeSequences()
    {
        
        switch(difficultyLevel)
        {
            case 0:
                sequenceLength = 4;
                break;
            case 1:
                sequenceLength = 5;
                break;
            case 2:
                sequenceLength = 7;
                break;
        }

        sequenceIndex = sequenceLength - 1;

        sequenceIndices = new int[sequenceLength];
        for (int i = 0; i < sequenceLength; i++)
            sequenceIndices[i] = Random.Range(0, 4);


        sequenceIntervals = new float[sequenceLength];
        sequenceIndices[0] = 0;
        for (int i = 1; i < sequenceLength; i++)
            sequenceIntervals[i] = Random.Range(3f, 5f);

        inputCounter = 0;
        playerIndices = new int[sequenceLength];
        playerIntervals = new float[sequenceLength];
    }

    public override IEnumerator StartGame(int difficultyLevel)
    {
        FindObjectOfType<Player>().MovementDisabled = true;
        this.difficultyLevel = difficultyLevel;
        GameCam.Priority = 11;
        RandomizeSequences();

        yield return new WaitForSeconds(FindObjectOfType<CinemachineBrain>().GetComponent<CinemachineBrain>().m_DefaultBlend.m_Time);

        GameCanvas.gameObject.SetActive(true);

        PromptCanvas.GetComponentInChildren<Text>().text = "Remember!";
        PromptCanvas.gameObject.SetActive(true);
        yield return new WaitForSeconds(1);
        PromptCanvas.gameObject.SetActive(false);

        isPlaying = true;
        showingSequence = true;
    }

    public override IEnumerator EndGame()
    {
        isPlaying = false;
        yield return new WaitForSeconds(2);
        enteringSequence = false;

        string debugstring1 = "Inputs: ";
        string debugstring2 = "Intervals: ";
        for (int i = 0; i < playerIndices.Length; i++)
        {
            debugstring1 += playerIndices[i] + " ";
            debugstring2 += playerIntervals[i] + " ";
        }
        debugstring1 += "  Answersheet: ";
        debugstring2 += "  Answersheet: ";
        for (int i = 0; i < playerIndices.Length; i++)
        {
            debugstring1 += sequenceIndices[i] + " ";
            debugstring2 += sequenceIntervals[i] + " ";
        }

        Debug.Log(debugstring1);

        bool match = true;
        for (int i = 0; i < sequenceLength; i++)
            if (sequenceIndices[i] != playerIndices[i])
                match = false;

        Debug.Log(match == true ? "Match!" : "Fail!");

        GameCanvas.gameObject.SetActive(false);

        GameCam.Priority = 0;
        FindObjectOfType<Player>().MovementDisabled = false;
        gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (isPlaying)
        {
            if (showingSequence)
            {
                if (countdownTimer > 0)
                    countdownTimer -= Time.deltaTime;
                else
                {
                    CreateBubble(sequenceIndices[sequenceIndex]);

                    countdownTimer = sequenceIntervals[sequenceIndex];
                    sequenceIndex--;
                }

                if (sequenceIndex == -1)
                {
                    showingSequence = false;
                    StartCoroutine(StartInputPhase());
                }
            }
            else if (enteringSequence)
            {
                if (Input.GetKeyDown(KeyCode.Alpha1))
                    RegisterUserInput(0);
                else if (Input.GetKeyDown(KeyCode.Alpha2))
                    RegisterUserInput(1);
                else if (Input.GetKeyDown(KeyCode.Alpha3))
                    RegisterUserInput(2);
                else if (Input.GetKeyDown(KeyCode.Alpha4))
                    RegisterUserInput(3);

                if (inputCounter == playerIndices.Length)
                {
                    StartCoroutine(EndGame());
                }
            }
        }
    }

    private IEnumerator PlaySoundAfterSec(int soundIndex, float seconds)
    {
        yield return new WaitForSeconds(seconds);
        audioSource.clip = SoundEffects[soundIndex];
        audioSource.Play();

        // Also add image
        sequenceImages.transform.GetChild(2 - sequenceIndex).GetComponent<RawImage>().texture = BubbleImages[soundIndex];
    }

    private IEnumerator StartInputPhase()
    {
        yield return new WaitForSeconds(6);
        PromptCanvas.GetComponentInChildren<Text>().text = "Repeat!";
        PromptCanvas.gameObject.SetActive(true);
        yield return new WaitForSeconds(3);
        PromptCanvas.gameObject.SetActive(false);

        enteringSequence = true;
    }

    private void CreateBubble(int index)
    {
        ParticleSystem ps = Instantiate(ParticleSystem, transform.position, Quaternion.identity, transform);

        ps.GetComponent<ParticleSystemRenderer>().material.mainTexture = BubbleImages[index];
        ps.gameObject.SetActive(true);

        StartCoroutine(PlaySoundAfterSec(index, ps.main.startLifetime.constant));
    }

    private void RegisterUserInput(int input)
    {
        CreateBubble(input);

        playerIndices[playerIndices.Length - 1 - inputCounter] = input;
        inputImages.GetChild(inputCounter).GetComponent<RawImage>().texture = BubbleImages[input];

        if (inputCounter > 0)
            playerIntervals[playerIntervals.Length - inputCounter] = Time.time - inputTimestamp;

        inputTimestamp = Time.time;
        inputCounter++;
    }
}
