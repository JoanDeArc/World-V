using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public class DialogueManager : MonoBehaviour
{
    public GameObject DialogueCanvas;
    public GameObject NamePanel;
    public Text NameText;
    public Text DialogueText;
    public GameObject TransformAnswers;


    public AudioSource TypingSound;

    private Queue<SentenceWrapper> sentences;

    private static Dialogue dialoguePickup;

    private Dialogue currentDialogue;

    private bool answersDisplayed;
    private bool gotAnswer;
    private string[] tempAnswers;
    private int answerCount;
    private int answerIndex;

    void Start()
    {
        sentences = new Queue<SentenceWrapper>();

        dialoguePickup = new Dialogue();
        dialoguePickup.Sentences = new SentenceWrapper[1];
    }


    public void StartDialogueAbout(ItemData item)
    {
        dialoguePickup.Sentences[0] = new SentenceWrapper(("",  item.About));

        StartDialogue(dialoguePickup);
    }

    public void StartDialogue(Item item, bool couldPickup)
    {
        if (couldPickup)
            dialoguePickup.Sentences[0] = new SentenceWrapper(("", "Picked up " + item.Name + "."));
        else
            dialoguePickup.Sentences[0] = new SentenceWrapper(("", "No space for " + item.Name + "."));

        StartDialogue(dialoguePickup);
    }

    public void StartDialogue(Dialogue dialogue)
    {
        FindObjectOfType<Player>().MovementDisabled = true;
        FindObjectOfType<Player>().IsInDialogue = true;

        DialogueCanvas.SetActive(true);
                TransformAnswers.SetActive(false);
        sentences.Clear();

        foreach (SentenceWrapper sentence in dialogue.Sentences)
            sentences.Enqueue(sentence);

        StepForwardDialogue();
    }

    
    public IEnumerator StartDialogueAnswer(DialogueWithAnswer dialogue, Action<int> answer)
    {
        currentDialogue = dialogue;
        answersDisplayed = false;
        gotAnswer = false;
        answerIndex = 0;

        for (int i = 0; i < 3; i++)
        {
            TransformAnswers.transform.GetChild(i).gameObject.SetActive(false);
        }

        StartDialogue(dialogue);

        while (!gotAnswer)
            yield return null;

        //Debug.Log("Returning answer index " + answerIndex + ".");
        answer(answerIndex);
    }

    public void StepForwardDialogue()
    {
        if (sentences.Count == 0)
        {
            if (!answersDisplayed && currentDialogue is DialogueWithAnswer)
                DisplayAnswers((currentDialogue as DialogueWithAnswer).Answers);
            else
                EndDialogue();
            return;
        }

        SentenceWrapper sentence = sentences.Dequeue();

        StopAllCoroutines();
        if (string.IsNullOrEmpty(sentence.Name))
            NamePanel.SetActive(false);
        else
        {
            NamePanel.SetActive(true);
            NameText.text = sentence.Name;
        }
        StartCoroutine(TypeSentence(sentence.Sentence));
    }

    private void DisplayAnswers(string[] answers)
    {

        DialogueText.text = "";
        TransformAnswers.SetActive(true);
        for (int i = 0; i < answers.Length; i++)
        {
            TransformAnswers.transform.GetChild(i).gameObject.SetActive(true);
            TransformAnswers.transform.GetChild(i).GetComponent<Text>().text = answers[i];
            TransformAnswers.transform.GetChild(i).GetComponent<Text>().color = new Color(0, 0, 0);
        }
        TransformAnswers.transform.GetChild(0).GetComponent<Text>().color = new Color(1, 0, 0);
        answerCount = answers.Length;

        answersDisplayed = true;
        return;
    }

    public void AddAnswerIndex(int amount)
    {
        if (!answersDisplayed)
            return;

        TransformAnswers.transform.GetChild(answerIndex).GetComponent<Text>().color = new Color(0, 0, 0);

        answerIndex += amount;
        if (answerIndex < 0)
            answerIndex = answerCount - 1;
        else if (answerIndex > answerCount - 1)
            answerIndex = 0;

        TransformAnswers.transform.GetChild(answerIndex).GetComponent<Text>().color = new Color(1, 0, 0);
    }

    IEnumerator TypeSentence (string sentence)
    {
        Debug.Log(sentence);
        TypingSound.Play();
        DialogueText.text = "";
        foreach (char letter in sentence.ToCharArray())
        {
            DialogueText.text += letter;

            yield return null;
        }
        TypingSound.Stop();
    }

    void EndDialogue()
    {
        DialogueCanvas.SetActive(false);
        gotAnswer = true;
        FindObjectOfType<Player>().MovementDisabled = false;
        FindObjectOfType<Player>().IsInDialogue = false;
    }
}
